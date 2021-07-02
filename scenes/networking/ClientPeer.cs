using Godot;

public class ClientPeer: Node {
    [Signal]
    public delegate void ConnectedToServer();

    public string ServerAddress;
    public int ServerPort;

    private Logging _Logger;

    public ClientPeer() {
        _Logger = Logging.GetLogger("ClientPeer");
        Name = "ClientPeer";
    }

    public override void _Ready()
    {
        GetTree().Connect("network_peer_connected", this, nameof(_PeerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(_PeerDisconnected));
        GetTree().Connect("connected_to_server", this, nameof(_ConnectedToServer));
        GetTree().Connect("connection_failed", this, nameof(_ConnectionFailed));
        GetTree().Connect("server_disconnected", this, nameof(_ServerDisconnected));

        var peer = new NetworkedMultiplayerENet();
        peer.CreateClient(ServerAddress, ServerPort);
        GetTree().NetworkPeer = peer;

        _Logger.DebugM("_Ready", "Client is ready.");
    }

    private void _PeerConnected(int peerId) {
        _Logger.DebugM("_PeerConnected", $"Peer {peerId} connected.");
    }

    private void _PeerDisconnected(int peerId) {
        _Logger.DebugM("_PeerDisconnected", $"Peer {peerId} disconnected.");
    }

    private void _ConnectedToServer() {
        _Logger.InfoM("_ConnectedToServer", "Connected to server.");

        var rpc = GetNode<RPCService>("/root/RPCService");
        rpc.Server.Ping();

        EmitSignal(nameof(ConnectedToServer));
    }

    private void _ConnectionFailed() {
        _Logger.ErrorM("_ConnectedToServer", "Connection failed.");
    }

    private void _ServerDisconnected() {
        _Logger.ErrorM("_ConnectedToServer", "Server disconnected.");
        QueueFree();
    }

    public override void _ExitTree()
    {
        GetTree().NetworkPeer = null;
    }
}