using Godot;
using Godot.Collections;
using System;

class SynchronizedScenePath: Godot.Object {
    public string GUID;
    public NodePath Parent;
    public string ScenePath;
}

public class ServerPeer: Node {
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

    public T SpawnSynchronizedScene<T>(NodePath parent, string scenePath) where T: Node {
        var nodeGuid = _GenerateGUID();
        var parentNode = GetNode(parent);
        var childNode = GD.Load<PackedScene>(scenePath).Instance<T>();
        childNode.Name = nodeGuid;
        parentNode.AddChild(childNode);

        _SynchronizedScenePaths.Add(nodeGuid, new SynchronizedScenePath() {
            GUID = nodeGuid,
            Parent = parent,
            ScenePath = scenePath
        });
        _SynchronizedNodes.Add(nodeGuid, childNode);

        // Send command to all connected clients
        _RPC.Client.SpawnSynchronizedSceneBroadcast(parent, scenePath, nodeGuid);

        return childNode;
    }

    public override void _Ready()
    {
        GetTree().Connect("network_peer_connected", this, nameof(_PeerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(_PeerDisconnected));

        var peer = new NetworkedMultiplayerENet();
        peer.CreateServer(ServerPort, MaxPlayers);
        GetTree().NetworkPeer = peer;

        _RPC = GetNode<RPCService>("/root/RPCService");
        _Logger.DebugM("_Ready", "Server is ready.");
    }

    private void _PeerConnected(int peerId) {
        _Logger.DebugM("_PeerConnected", $"Peer {peerId} connected.");

        // Spawn existing scenes
        foreach (var kv in _SynchronizedScenePaths) {
            var syncScenePath = kv.Value;
            _RPC.Client.SpawnSynchronizedSceneTo(peerId, syncScenePath.Parent, syncScenePath.ScenePath, syncScenePath.GUID);
        }
    }

    private void _PeerDisconnected(int peerId) {
        _Logger.DebugM("_PeerDisconnected", $"Peer {peerId} disconnected.");
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
}