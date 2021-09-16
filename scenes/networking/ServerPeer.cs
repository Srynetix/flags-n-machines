using Godot;
using System;
using System.Collections.Generic;

class SynchronizedScenePath: Godot.Object {
    public string GUID;
    public string Name;
    public NodePath Parent;
    public string ScenePath;
    public int OwnerPeerId;
    public Dictionary<NodePath, int> MasterConfiguration;
}

public class ServerPeer: Node {
    [Signal]
    public delegate void PeerConnected(int peerId);
    [Signal]
    public delegate void PeerDisconnected(int peerId);

    public int ServerPort;
    public int MaxPlayers;
    private Dictionary<int, string> _Players = new Dictionary<int, string>();

    private RPCService _RPC;
    private Logging _Logger;
    private Dictionary<string, SynchronizedScenePath> _SynchronizedScenePaths = new Dictionary<string, SynchronizedScenePath>();
    private Dictionary<string, Node> _SynchronizedNodes = new Dictionary<string, Node>();

    public ServerPeer() {
        _Logger = Logging.GetLogger("ServerPeer");
        Name = "ServerPeer";
    }

    public Dictionary<int, string> GetPlayers() {
        return _Players;
    }

    public Node SpawnSynchronizedScene(NodePath parent, string scenePath, int ownerPeerId = 1, Dictionary<NodePath, int> masterConfiguration = null) {
        return SpawnSynchronizedScene<Node>(parent, scenePath, ownerPeerId, masterConfiguration);
    }

    public T SpawnSynchronizedScene<T>(NodePath parent, string scenePath, int ownerPeerId = 1, Dictionary<NodePath, int> masterConfiguration = null) where T: Node {
        var nodeGuid = _GenerateGUID();
        var parentNode = GetNode(parent);
        var childNode = GD.Load<PackedScene>(scenePath).Instance<T>();
        var name = childNode.Name;
        childNode.Name = GenerateNetworkName(childNode.Name, nodeGuid);
        childNode.SetNetworkMaster(ownerPeerId);
        parentNode.AddChild(childNode);

        if (masterConfiguration != null) {
            foreach (var kv in masterConfiguration) {
                childNode.GetNode(kv.Key).SetNetworkMaster(kv.Value);
            }
        }

        _SynchronizedScenePaths.Add(nodeGuid, new SynchronizedScenePath() {
            GUID = nodeGuid,
            Name = name,
            Parent = parent,
            ScenePath = scenePath,
            OwnerPeerId = ownerPeerId,
            MasterConfiguration = masterConfiguration,
        });
        _SynchronizedNodes.Add(nodeGuid, childNode);

        // Send command to all connected clients
        _RPC.Client.SpawnSynchronizedSceneBroadcast(parent, name, scenePath, nodeGuid, ownerPeerId, masterConfiguration);

        return childNode;
    }

    public Node SpawnSynchronizedNamedScene(NodePath parent, string scenePath, string sceneName, int ownerPeerId = 1, Dictionary<NodePath, int> masterConfiguration = null) {
        return SpawnSynchronizedNamedScene<Node>(parent, scenePath, sceneName, ownerPeerId, masterConfiguration);
    }

    public T SpawnSynchronizedNamedScene<T>(NodePath parent, string scenePath, string sceneName, int ownerPeerId = 1, Dictionary<NodePath, int> masterConfiguration = null) where T: Node {
        var parentNode = GetNode(parent);
        var childNode = GD.Load<PackedScene>(scenePath).Instance<T>();
        childNode.Name = sceneName;
        childNode.SetNetworkMaster(ownerPeerId);
        parentNode.AddChild(childNode);

        if (masterConfiguration != null) {
            foreach (var kv in masterConfiguration) {
                childNode.GetNode(kv.Key).SetNetworkMaster(kv.Value);
            }
        }

        _SynchronizedScenePaths.Add(sceneName, new SynchronizedScenePath() {
            GUID = "",
            Name = sceneName,
            Parent = parent,
            ScenePath = scenePath,
            OwnerPeerId = ownerPeerId,
            MasterConfiguration = masterConfiguration,
        });
        _SynchronizedNodes.Add(sceneName, childNode);

        // Send command to all connected clients
        _RPC.Client.SpawnSynchronizedSceneBroadcast(parent, sceneName, scenePath, "", ownerPeerId, masterConfiguration);

        return childNode;
    }

    public Node SpawnSynchronizedSceneMapped(NodePath parent, string name, string serverScenePath, string clientScenePath, int ownerPeerId = 1, Dictionary<NodePath, int> masterConfiguration = null) {
        return SpawnSynchronizedSceneMapped<Node>(parent, name, serverScenePath, clientScenePath, ownerPeerId, masterConfiguration);
    }

    public T SpawnSynchronizedSceneMapped<T>(NodePath parent, string name, string serverScenePath, string clientScenePath, int ownerPeerId = 1, Dictionary<NodePath, int> masterConfiguration = null) where T: Node {
        var nodeGuid = _GenerateGUID();
        var parentNode = GetNode(parent);
        var serverChildNode = GD.Load<PackedScene>(serverScenePath).Instance<T>();
        serverChildNode.Name = GenerateNetworkName(name, nodeGuid);
        serverChildNode.SetNetworkMaster(ownerPeerId);
        parentNode.AddChild(serverChildNode);

        if (masterConfiguration != null) {
            foreach (var kv in masterConfiguration) {
                serverChildNode.GetNode(kv.Key).SetNetworkMaster(kv.Value);
            }
        }

        _SynchronizedScenePaths.Add(nodeGuid, new SynchronizedScenePath() {
            GUID = nodeGuid,
            Name = name,
            Parent = parent,
            ScenePath = clientScenePath,
            OwnerPeerId = ownerPeerId,
            MasterConfiguration = masterConfiguration
        });
        _SynchronizedNodes.Add(nodeGuid, serverChildNode);

        // Send command to all connected clients
        _RPC.Client.SpawnSynchronizedSceneBroadcast(parent, name, clientScenePath, nodeGuid, ownerPeerId, masterConfiguration);

        return serverChildNode;
    }

    public void RemoveSynchronizedNode(Node node) {
        var guid = node.Name;
        var syncNode = _SynchronizedNodes[guid];
        _SynchronizedNodes.Remove(guid);
        _SynchronizedScenePaths.Remove(guid);

        _RPC.Client.RemoveSynchronizedNodeBroadcast(node.GetPath());
        node.QueueFree();
    }

    public override void _Ready()
    {
        GetTree().Connect("network_peer_connected", this, nameof(_PeerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(_PeerDisconnected));

        var peer = new NetworkedMultiplayerENet();
        peer.CreateServer(ServerPort, MaxPlayers);
        peer.AllowObjectDecoding = true;
        GetTree().NetworkPeer = peer;

        _RPC = RPCService.GetInstance(GetTree());
        _Logger.DebugM("_Ready", $"Server started on port '{ServerPort}' for '{MaxPlayers}' players.");
    }

    private void _PeerConnected(int peerId) {
        _Logger.DebugM("_PeerConnected", $"Peer {peerId} connected.");
        _RPC.SyncInput.CreatePeerInput(peerId);
        _Players[peerId] = "Player";

        _SpawnExistingScenes(peerId);
        EmitSignal(nameof(PeerConnected), peerId);
    }

    private void _SpawnExistingScenes(int peerId) {
        _Logger.DebugM("_SpawnExistingScenes", $"Spawning existing scenes on peer {peerId}.");

        // Spawn existing scenes
        foreach (var kv in _SynchronizedScenePaths) {
            var syncScenePath = kv.Value;
            _RPC.Client.SpawnSynchronizedSceneOn(peerId, syncScenePath.Parent, syncScenePath.Name, syncScenePath.ScenePath, syncScenePath.GUID, syncScenePath.OwnerPeerId, syncScenePath.MasterConfiguration);
        }
    }

    private void _PeerDisconnected(int peerId) {
        _Logger.DebugM("_PeerDisconnected", $"Peer {peerId} disconnected.");
        _RPC.SyncInput.RemovePeerInput(peerId);
        _Players.Remove(peerId);

        EmitSignal(nameof(PeerDisconnected), peerId);
    }

    private static string _GenerateGUID() {
        return Guid.NewGuid().ToString();
    }

    public static string GenerateNetworkName(string name, string guid) {
        if (guid != "") {
            return $"{name}#{guid}";
        } else {
            return name;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        foreach (var kv in _SynchronizedNodes) {
            var node = kv.Value;
            if (node is ISynchronizable syncNode) {
                var data = syncNode._NetworkSend();
                if (data != null && data.Keys.Count > 0) {
                    _RPC.Client.SynchronizeNodeBroadcast(node.GetPath(), data);
                }
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey) {
            if (eventKey.Pressed) {
                if (eventKey.Scancode == (int)KeyList.F4) {
                    GetTree().Root.PrintTreePretty();
                }

                else if (eventKey.Scancode == (int)KeyList.F5) {
                    GD.Print(RPCService.GetInstance(GetTree()).SyncInput._PlayerInputs);
                }
            }
        }
    }
}