using Godot;
using Godot.Collections;

public class ServerRPC: Node {
    public Logging _Logger;
    private RPCService _Service;

    public ServerRPC() {
        _Logger = Logging.GetLogger("ServerRPC");
        Name = "ServerRPC";
    }

    public void LinkService(RPCService service) {
        _Service = service;
    }

    public void Ping() {
        var myId = GetTree().GetNetworkUniqueId();
        _Logger.DebugMN(myId, "Ping", "Sending ping to server.");
        RpcId(1, nameof(_Ping));
    }

    public void SendInput(Dictionary<string, float> input) {
        RpcUnreliableId(1, nameof(_SendInput), input);
    }

    public void AskPlayerScores() {
        RpcId(1, nameof(_AskPlayerScores));
    }

    [Master]
    private void _SendInput(Dictionary<string, float> input) {
        var peerId = GetTree().GetRpcSenderId();
        _Service.SyncInput.UpdatePeerInput(peerId, input);
    }

    [Master]
    private void _AskPlayerScores() {
        var peerId = GetTree().GetRpcSenderId();
        _Service.Client.ReceivePlayerScores(new Dictionary<string, int> {});
    }

    [Master]
    private void _Ping() {
        var myId = GetTree().GetNetworkUniqueId();
        var peerId = GetTree().GetRpcSenderId();

        _Logger.DebugMN(myId, "_Ping", $"Ping request received from peer {peerId}.");
        _Service.Client.Pong(peerId);
    }
}