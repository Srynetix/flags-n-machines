using Godot;

public class ServerRPC: Node {
    public Logging _Logger;
    private ClientRPC _ClientRPC;

    public ServerRPC() {
        _Logger = Logging.GetLogger("ServerRPC");
        Name = "ServerRPC";
    }

    public void LinkClientRPC(ClientRPC client) {
        _ClientRPC = client;
    }

    public void Ping() {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "Ping", "Sending ping to server.");
        RpcId(1, nameof(_Ping));
    }

    [Master]
    private void _Ping() {
        var myId = GetTree().GetNetworkUniqueId();
        var peerId = GetTree().GetRpcSenderId();

        _Logger.DebugMN(myId, "_Ping", $"Ping request received from peer {peerId}.");
        _ClientRPC.Pong(peerId);
    }
}