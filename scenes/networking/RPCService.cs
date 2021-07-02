using Godot;

public class RPCService: Node {
    public ClientRPC Client;
    public ServerRPC Server;

    public RPCService() {
        Name = "RPCService";
    }

    public override void _Ready()
    {
        Client = new ClientRPC();
        AddChild(Client);

        Server = new ServerRPC();
        AddChild(Server);

        Client.LinkServerRPC(Server);
        Server.LinkClientRPC(Client);
    }
}