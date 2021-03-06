using Godot;
using Godot.Collections;

public class ClientRPC: Node {
    [Signal]
    public delegate void SpawnedFromServer(Node node);
    [Signal]
    public delegate void RemovedFromServer(Node node);

    public Logging _Logger;
    private RPCService _Service;

    public ClientRPC() {
        _Logger = Logging.GetLogger("ClientRPC");
        Name = "ClientRPC";
    }

    public void LinkService(RPCService service) {
        _Service = service;
    }

    public void Pong(int peerId) {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "Pong", $"Sending pong to peer {peerId}.");
        RpcId(peerId, nameof(_Pong));
    }

    public void SpawnSynchronizedSceneOn(int peerId, NodePath parent, string scenePath, string guid, int ownerPeerId, Dictionary<NodePath, int> masterConfiguration) {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "SpawnSynchronizedSceneTo", $"Sending scene spawn '{scenePath}' at parent '{parent}' with guid '{guid}' and owner '{ownerPeerId}' to peer '{peerId}'.");
        RpcId(peerId, nameof(_SpawnSynchronizedScene), parent, scenePath, guid, ownerPeerId, masterConfiguration);
    }

    public void SpawnSynchronizedSceneBroadcast(NodePath parent, string scenePath, string guid, int ownerPeerId, Dictionary<NodePath, int> masterConfiguration) {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "SpawnSynchronizedSceneBroadcast", $"Sending scene spawn '{scenePath}' at parent '{parent}' with guid '{guid}' and owner '{ownerPeerId}' to all peers.");
        Rpc(nameof(_SpawnSynchronizedScene), parent, scenePath, guid, ownerPeerId, masterConfiguration);
    }

    public void SynchronizeNodeBroadcast(NodePath path, Dictionary<string, object> data) {
        RpcUnreliable(nameof(_SynchronizeNode), path, data);
    }

    public void RemoveSynchronizedNodeBroadcast(NodePath path) {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "RemoveSynchronizedNodeBroadcast", $"Will remove node '{path}' on all peers.");
        Rpc(nameof(_RemoveSynchronizedNode), path);
    }

    public void RemoveSynchronizedNodeOn(int peerId, NodePath path) {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "RemoveSynchronizedNodeBroadcast", $"Will remove node '{path}' on peer {peerId}.");
        RpcId(peerId, nameof(_RemoveSynchronizedNode), path);
    }

    [Remote]
    private void _Pong() {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "_Pong", "Pong received from server!");
    }

    [Remote]
    private void _SpawnSynchronizedScene(NodePath parent, string scenePath, string guid, int ownerPeerId, Dictionary<NodePath, int> masterConfiguration) {
        var parentNode = GetNode(parent);
        var childNode = GD.Load<PackedScene>(scenePath).Instance();
        childNode.Name = guid;
        childNode.SetNetworkMaster(ownerPeerId);
        parentNode.AddChild(childNode);

        foreach (var kv in masterConfiguration) {
            childNode.GetNode(kv.Key).SetNetworkMaster(kv.Value);
        }

        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "_SpawnSynchronizedScene", $"Spawned scene '{scenePath}' at parent '{parent}' with guid '{guid}' and owner '{ownerPeerId}'.");

        EmitSignal(nameof(SpawnedFromServer), childNode);
    }

    [Remote]
    private void _SynchronizeNode(NodePath path, Dictionary<string, object> data) {
        var node = GetNode(path);
        if (node is ISynchronizable syncNode) {
            syncNode._NetworkReceive(data);
        }
    }

    [Remote]
    private void _RemoveSynchronizedNode(NodePath path) {
        var myId = GetTree().GetNetworkUniqueId();
        var node = GetNodeOrNull(path);
        if (node != null) {
            node.QueueFree();
            _Logger.DebugMN(myId, "_RemoveSynchronizedNode", $"Removed node {path}.");
        }

        EmitSignal(nameof(RemovedFromServer), node);
    }
}