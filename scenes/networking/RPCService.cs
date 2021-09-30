using Godot;

public class RPCService : Node
{
    public ClientRPC Client;
    public ServerRPC Server;
    public SyncInput SyncInput;

    public RPCService()
    {
        Name = "RPCService";
    }

    public static RPCService GetInstance(SceneTree tree)
    {
        return tree.Root.GetNode<RPCService>("RPCService");
    }

    public override void _Ready()
    {
        Client = new ClientRPC();
        AddChild(Client);

        Server = new ServerRPC();
        AddChild(Server);

        SyncInput = new SyncInput();
        AddChild(SyncInput);

        Client.LinkService(this);
        Server.LinkService(this);
    }
}