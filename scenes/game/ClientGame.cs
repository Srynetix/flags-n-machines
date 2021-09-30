using Godot;

public class ClientGame : Spatial
{
    private ListenServerPeer _ListenServer;
    private FPSCamera _SpectatorCamera;
    private ChaseCamera _ChaseCamera;
    private ClientPeer _Client;
    private RPCService _RPC;

    public ClientGame()
    {
        Name = "Game";
    }

    public override void _Ready()
    {
        _SpectatorCamera = GetNode<FPSCamera>("SpectatorCamera");
        _ChaseCamera = GetNode<ChaseCamera>("ChaseCamera");
        _RPC = RPCService.GetInstance(GetTree());
        _RPC.Client.Connect(nameof(ClientRPC.SpawnedFromServer), this, nameof(_Client_NodeSpawned));

        _Client = new ClientPeer()
        {
            ServerAddress = CVars.GetInstance().GetVar<string>("join_server_address"),
            ServerPort = CVars.GetInstance().GetVar<int>("join_server_port")
        };
        AddChild(_Client);
    }

    private void _Client_NodeSpawned(Node node)
    {
        if (node is Car car)
        {
            if (car.IsOwnedByCurrentPeer())
            {
                car.GetInputController().ShowVirtualControls();
                _ChaseCamera.Target = car;
                _ChaseCamera.Current = true;
            }
        }
    }
}
