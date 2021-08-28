using Godot;

public class ListenServerPeer: Node {
    public int ServerPort;
    public int MaxPlayers;

    private Logging _Logger;
    private SceneTree _SceneTree;
    private ServerPeer _Server;

    public ListenServerPeer() {
        _Logger = Logging.GetLogger("ListenServerPeer");
        Name = "ListenServerPeer";
    }

    public Node GetServerRoot() {
        return _SceneTree.Root;
    }

    public SceneTree GetServerTree() {
        return _SceneTree;
    }

    public ServerPeer GetServer() {
        return _Server;
    }

    public void PrintServerTree() {
        _SceneTree.Root.PrintTreePretty();
    }

    public override void _Ready()
    {
        _SceneTree = new SceneTree();
        _SceneTree.Init();
        _SceneTree.Root.RenderTargetUpdateMode = Viewport.UpdateMode.Disabled;

        var rpc = new RPCService();
        _SceneTree.Root.AddChild(rpc);

        _Server = new ServerPeer() {
            ServerPort = ServerPort,
            MaxPlayers = MaxPlayers
        };
        _SceneTree.Root.AddChild(_Server);

        _Logger.DebugM("_Ready", "Listen server is ready.");
    }

    public override void _Process(float delta)
    {
        _SceneTree.Idle(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        _SceneTree.Iteration(delta);
    }

    public override void _Input(InputEvent @event)
    {
        _SceneTree.InputEvent(@event);
    }

    public override void _ExitTree()
    {
        _SceneTree.Finish();
        _Logger.DebugM("_ExitTree", "Listen server exited.");
    }
}