using Godot;
using Godot.Collections;

public class ServerGame : Spatial
{
    private ListenServerPeer _ListenServer;
    private FPSCamera _SpectatorCamera;
    private ChaseCamera _ChaseCamera;
    private ServerPeer _Server;
    private ClientPeer _Client;
    private RPCService _RPC;

    private Dictionary<int, Car> _Cars = new Dictionary<int, Car>();

    public ServerGame() {
        Name = "Game";
    }

    public override void _Ready()
    {
        _SpectatorCamera = GetNode<FPSCamera>("SpectatorCamera");
        _ChaseCamera = GetNode<ChaseCamera>("ChaseCamera");
        _RPC = RPCService.GetInstance(GetTree());
        _RPC.Client.Connect(nameof(ClientRPC.SpawnedFromServer), this, nameof(_Client_NodeSpawned));

        _ListenServer = new ListenServerPeer() {
            ServerPort = CVars.GetInstance().GetVar<int>("host_server_port"),
            MaxPlayers = CVars.GetInstance().GetVar<int>("host_max_players")
        };
        AddChild(_ListenServer);

        _ListenServer.GetServerRoot().AddChild(new Spatial() {
            Name = "Game"
        });
        _Server = _ListenServer.GetServer();
        _Server.Connect(nameof(ServerPeer.PeerConnected), this, nameof(_Server_PeerConnected));
        _Server.Connect(nameof(ServerPeer.PeerDisconnected), this, nameof(_Server_PeerDisconnected));
        _StartServerGame();

        _Client = new ClientPeer() {
            ServerAddress = "127.0.0.1",
            ServerPort = CVars.GetInstance().GetVar<int>("host_server_port")
        };
        AddChild(_Client);
    }

    private void _StartServerGame() {
        _Server.SpawnSynchronizedScene(
            "/root/Game", "res://scenes/tests/MapCSG.tscn"
        );

        var limits = _Server.SpawnSynchronizedScene<LevelLimits>(
            "/root/Game", "res://scenes/tests/LevelLimits.tscn"
        );
        limits.Connect(nameof(LevelLimits.OutOfLimits), this, nameof(_NodeOutOfLimits));
    }

    private void _StartClientGame(int peerId) {
        var car = _Server.SpawnSynchronizedScene<Car>(
            "/root/Game", "res://scenes/common/Car.tscn",
            masterConfiguration: new Dictionary<NodePath, int> {
                { "InputController", peerId }
            }
        );
        car.Translate(car.Transform.basis.y * 10);
        _Cars.Add(peerId, car);
    }

    private void _Server_PeerConnected(int peerId) {
        _StartClientGame(peerId);
    }

    private void _Server_PeerDisconnected(int peerId) {
        if (_Cars.ContainsKey(peerId)) {
            _Server.RemoveSynchronizedNode(_Cars[peerId]);
            _Cars.Remove(peerId);
        }
    }

    private void _Client_NodeSpawned(Node node) {
        if (node is Car car) {
            if (car.IsOwnedByCurrentPeer()) {
                car.GetInputController().ShowVirtualControls();
                _ChaseCamera.Target = car;
                _ChaseCamera.Current = true;
            }
        }
    }

    private void _NodeOutOfLimits(Node node) {
        if (node is Car car) {
            car.ResetMovement();
            car.Translation = new Vector3();
            car.Rotation = new Vector3();
            car.Translate(car.Transform.basis.y * 10);
        }
    }
}
