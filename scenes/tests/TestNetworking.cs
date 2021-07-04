using Godot;

public class TestNetworking : Spatial
{
    private Button _ClientButton;
    private Button _ServerButton;
    private Button _ListenServerButton;

    private ServerPeer _Server;
    private ClientPeer _Client;
    private Logging _Logger;
    private ChaseCamera _Camera;
    private RPCService _RPC;

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
        _Camera = GetNode<ChaseCamera>("ChaseCamera");

        _RPC = RPCService.GetInstance(GetTree());
        _RPC.Client.Connect(nameof(ClientRPC.SpawnedFromServer), this, nameof(_OnClientNodeSpawned));
    }

    private void _CreateClient() {
        _ClientButton.Disabled = true;
        _ServerButton.Disabled = true;

        _Client = new ClientPeer() {
            ServerAddress = "127.0.0.1",
            ServerPort = 12341
        };
        _Client.Connect(nameof(ClientPeer.ConnectedToServer), this, nameof(_OnClientConnected));
        AddChild(_Client);
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
        _StartServerGame();
    }

    private void _OnClientConnected() {
        _ListenServerButton.Disabled = true;
    }

    private void _OnClientNodeSpawned(Node node) {
        if (node is Car car) {
            if (car.IsOwnedByCurrentPeer()) {
                _Camera.Target = car.CameraTarget;
            }
        }
    }

    private void _ServerPeerConnected(int peerId) {
        _StartClientGame(peerId);
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
        _StartServerGame();
    }

    private void _StartServerGame() {
        _Server.Connect(nameof(ServerPeer.PeerConnected), this, nameof(_ServerPeerConnected));
        _Server.SpawnSynchronizedScene<Node>(
            "/root/TestNetworking", "res://scenes/tests/TestLevel.tscn"
        );
    }

    private void _StartClientGame(int peerId) {
        var car = _Server.SpawnSynchronizedScene<Car>(
            "/root/TestNetworking", "res://scenes/common/Car.tscn",
            masterConfiguration: new Godot.Collections.Dictionary<NodePath, int> {
                { "InputController", peerId }
            }
        );
        car.Translate(car.Transform.basis.y * 10);
    }
}
