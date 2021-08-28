using Godot;

public class GameMasterClient : Node
{
    private Logging _Logger;
    private RPCService _RPC;
    private FPSCamera _SpectatorCamera;
    private ChaseCamera _ChaseCamera;
    private Car _Car;

    public GameMasterClient() {
        _Logger = Logging.GetLogger("GameMasterClient");
    }

    public override void _Ready() {
        _Logger.InfoM("_Ready");

        _RPC = RPCService.GetInstance(GetTree());
        _RPC.Client.Connect(nameof(ClientRPC.SpawnedFromServer), this, nameof(_NodeSpawned));
    }

    private void _NodeSpawned(Node node) {
        _Logger.InfoMN(GetTree().GetNetworkUniqueId(), "_Client_NodeSpawned", node);
        GD.Print("client node spawned");

        if (node is Car car) {
            if (car.IsOwnedByCurrentPeer()) {
                car.GetInputController().ShowVirtualControls();
                _Car = car;
                // var controller = car.GetInputController();
                // if (controller != null) {
                //     controller.ShowVirtualControls();
                // }
                // _ChaseCamera.Target = car;
                // _ChaseCamera.Current = true;
            }
        }

        else if (node is ChaseCamera cam) {
            _ChaseCamera = cam;
            _ChaseCamera.Target = _Car;
            _ChaseCamera.Current = true;
        }

        else if (node is FPSCamera cam2) {
            _SpectatorCamera = cam2;
        }
    }
}
