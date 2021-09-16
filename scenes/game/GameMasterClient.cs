using Godot;
using System.Collections.Generic;
using BinaryPack;

public class GameMasterClient : Node
{
    private Logging _Logger;
    private RPCService _RPC;
    private Car _Car;
    private GameStage _GameNode;

    public GameMasterClient() {
        _Logger = Logging.GetLogger("GameMasterClient");
    }

    public override void _Ready() {
        _RPC = RPCService.GetInstance(GetTree());
        _RPC.Client.Connect(nameof(ClientRPC.SpawnedFromServer), this, nameof(_NodeSpawned));
        _GameNode = GetParent().GetNode<GameStage>("Stage");
    }

    private void _NodeSpawned(Node node) {
        _Logger.InfoMN(GetTree().GetNetworkUniqueId(), "_Client_NodeSpawned", node);

        if (node is Car car) {
            if (car.IsOwnedByCurrentPeer()) {
                car.GetInputController().ShowVirtualControls();
                _Car = car;
                _GameNode.ChaseCamera.Target = car;
                _GameNode.ChaseCamera.Current = true;
            }
        }
    }

    [Remote]
    public void ReceivePlayerScores(byte[] data) {
        var scores = BinaryConverter.Deserialize<PlayerDataDict>(data);
        _GameNode.ScoresUI.UpdateScores(scores.data);
    }
}
