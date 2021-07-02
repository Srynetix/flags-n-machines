using Godot;

public class TestNetworking : Spatial
{
    private Button _ClientButton;
    private Button _ServerButton;
    private Button _ListenServerButton;

    private ServerPeer _Server;
    private Logging _Logger;

    public TestNetworking() {
        _Logger = Logging.GetLogger("TestNetworking");
    }

    public override void _Ready()
    {
        _ClientButton = GetNode<Button>("GUI/Container/Client");
        _ServerButton = GetNode<Button>("GUI/Container/Server");
        _ListenServerButton = GetNode<Button>("GUI/Container/ListenServer");
        _ClientButton.Connect("pressed", this, nameof(_CreateClient));
        _ServerButton.Connect("pressed", this, nameof(_CreateServer));
        _ListenServerButton.Connect("pressed", this, nameof(_CreateListenServer));
    }

    private void _CreateClient() {
        _ClientButton.Disabled = true;
        _ServerButton.Disabled = true;

        var client = new ClientPeer() {
            ServerAddress = "127.0.0.1",
            ServerPort = 12341
        };
        AddChild(client);
    }

    private void _CreateServer() {
        _ClientButton.Disabled = true;
        _ServerButton.Disabled = true;
        _ListenServerButton.Disabled = true;

        _Server = new ServerPeer() {
            ServerPort = 12341,
            MaxPlayers = 10
        };
        AddChild(_Server);
    }

    private void _CreateListenServer() {
        _ListenServerButton.Disabled = true;
        _ServerButton.Disabled = true;

        var server = new ListenServerPeer() {
            ServerPort = 12341,
            MaxPlayers = 10
        };
        AddChild(server);

        // Spawn transform on server
        var scene = new Spatial() {
            Name = "TestNetworking"
        };
        server.GetServerRoot().AddChild(scene);
        _Server = server.GetServer();

        var ground = _Server.SpawnSynchronizedScene<Node>(
            "/root/TestNetworking", "res://scenes/tests/Ground.tscn"
        );

        var car = _Server.SpawnSynchronizedScene<Car>(
            "/root/TestNetworking", "res://scenes/common/Car.tscn"
        );
        car.Translate(car.Transform.basis.y * 10);
    }
}
