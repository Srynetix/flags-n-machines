using Godot;
using Godot.Collections;
using System;

class SynchronizedScenePath: Godot.Object {
    public string GUID;
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

    private RPCService _RPC;
    private Logging _Logger;
    private Dictionary<string, SynchronizedScenePath> _SynchronizedScenePaths = new Dictionary<string, SynchronizedScenePath>();
    private Dictionary<string, Node> _SynchronizedNodes = new Dictionary<string, Node>();

    public ServerPeer() {
        _Logger = Logging.GetLogger("ServerPeer");
        Name = "ServerPeer";
    }

    public T SpawnSynchronizedScene<T>(NodePath parent, string scenePath, int ownerPeerId = 1, Dictionary<NodePath, int> masterConfiguration = null) where T: Node {
        var nodeGuid = _GenerateGUID();
        var parentNode = GetNode(parent);
        var childNode = GD.Load<PackedScene>(scenePath).Instance<T>();
        childNode.Name = nodeGuid;
        childNode.SetNetworkMaster(ownerPeerId);
        parentNode.AddChild(childNode);

        if (masterConfiguration != null) {
            foreach (var kv in masterConfiguration) {
                childNode.GetNode(kv.Key).SetNetworkMaster(kv.Value);
            }
        }

        _SynchronizedScenePaths.Add(nodeGuid, new SynchronizedScenePath() {
            GUID = nodeGuid,
            Parent = parent,
            ScenePath = scenePath,
            OwnerPeerId = ownerPeerId,
            MasterConfiguration = masterConfiguration,
        });
        _SynchronizedNodes.Add(nodeGuid, childNode);

        // Send command to all connected clients
        _RPC.Client.SpawnSynchronizedSceneBroadcast(parent, scenePath, nodeGuid, ownerPeerId, masterConfiguration);

        return childNode;
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
        GetTree().NetworkPeer = peer;

        _RPC = RPCService.GetInstance(GetTree());
        _Logger.DebugM("_Ready", "Server is ready.");
    }

    private void _PeerConnected(int peerId) {
        _Logger.DebugM("_PeerConnected", $"Peer {peerId} connected.");
        _RPC.SyncInput.CreatePeerInput(peerId);

        // Spawn existing scenes
        foreach (var kv in _SynchronizedScenePaths) {
            var syncScenePath = kv.Value;
            _RPC.Client.SpawnSynchronizedSceneOn(peerId, syncScenePath.Parent, syncScenePath.ScenePath, syncScenePath.GUID, syncScenePath.OwnerPeerId, syncScenePath.MasterConfiguration);
        }

        EmitSignal(nameof(PeerConnected), peerId);
    }

    private void _PeerDisconnected(int peerId) {
        _Logger.DebugM("_PeerDisconnected", $"Peer {peerId} disconnected.");
        _RPC.SyncInput.RemovePeerInput(peerId);

        EmitSignal(nameof(PeerDisconnected), peerId);
    }

    private string _GenerateGUID() {
        return Guid.NewGuid().ToString();
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