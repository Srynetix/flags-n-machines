using Godot;
using Godot.Collections;

public class ClientRPC: Node {
    public Logging _Logger;
    private ServerRPC _ServerRPC;

    public ClientRPC() {
        _Logger = Logging.GetLogger("ClientRPC");
        Name = "ClientRPC";
    }

    public void LinkServerRPC(ServerRPC server) {
        _ServerRPC = server;
    }

    public void Pong(int clientId) {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "Pong", $"Sending pong to client {clientId}.");
        RpcId(clientId, nameof(_Pong));
    }

    public void SpawnSynchronizedSceneTo(int clientId, NodePath parent, string scenePath, string guid) {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "SpawnSynchronizedSceneTo", $"Sending scene spawn '{scenePath}' at parent '{parent}' with guid '{guid}' to client '{clientId}'.");
        RpcId(clientId, nameof(_SpawnSynchronizedScene), parent, scenePath, guid);
    }

    public void SpawnSynchronizedSceneBroadcast(NodePath parent, string scenePath, string guid) {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "SpawnSynchronizedSceneBroadcast", $"Sending scene spawn '{scenePath}' at parent '{parent}' with guid '{guid}' to all clients.");
        Rpc(nameof(_SpawnSynchronizedScene), parent, scenePath, guid);
    }

    public void SynchronizeNodeBroadcast(NodePath path, Dictionary<string, object> data) {
        RpcUnreliable(nameof(_SynchronizeNode), path, data);
    }

    [Remote]
    private void _Pong() {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "_Pong", "Pong received from server!");
    }

    [Remote]
    private void _SpawnSynchronizedScene(NodePath parent, string scenePath, string guid) {
        var parentNode = GetNode(parent);
        var childNode = GD.Load<PackedScene>(scenePath).Instance();
        childNode.Name = guid;
        parentNode.AddChild(childNode);

        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "_SpawnSynchronizedScene", $"Spawned scene '{scenePath}' at parent '{parent}' with GUID '{guid}'.");
    }

    [Remote]
    private void _SynchronizeNode(NodePath path, Dictionary<string, object> data) {
        var node = GetNode(path);
        if (node is ISynchronizable syncNode) {
            syncNode._NetworkReceive(data);
        }
    }
}