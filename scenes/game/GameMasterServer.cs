using Godot;
using BinaryPack;
using System.Collections.Generic;

public class PlayerData {
    public string Name;
    public int Score;
}

public class PlayerDataDict {
    public Dictionary<int, PlayerData> data = new Dictionary<int, PlayerData>();
}

public enum GameMasterState {
    WaitingFirstPlayer,
    WaitingSecondPlayer,
    WaitingMaxPlayers,
    GameStarted
}

public class GameMasterServer : Node
{
    private Logging _Logger;
    private RPCService _RPC;
    private GameMasterState _State;
    private ServerPeer _Server;
    private Dictionary<int, Car> _Cars = new Dictionary<int, Car>();
    private PlayerDataDict _PlayerData = new PlayerDataDict();

    public GameMasterServer() {
        _Logger = Logging.GetLogger("GameMasterServer");
        _State = GameMasterState.WaitingFirstPlayer;
    }

    public override void _Ready() {
        _Logger.InfoM("_Ready", "GameMasterServer is ready.");

        _RPC = RPCService.GetInstance(GetTree());
        _Server = NodeRegistry.GetInstance(GetTree()).GetNodeFromRegistry<ServerPeer>("Server");
        _Server.Connect(nameof(ServerPeer.PeerConnected), this, nameof(_PeerConnected));
        _Server.Connect(nameof(ServerPeer.PeerDisconnected), this, nameof(_PeerDisconnected));
        _Server.SpawnSynchronizedNamedScene<GameStage>("/root/Game", "res://scenes/game/GameStage.tscn", "Stage");

        // Load player scores
        foreach (var player in _Server.GetPlayers()) {
            _RegisterPlayer(player.Key, player.Value);
        }

        if (_PlayerData.data.Count == 0) {
            _State = GameMasterState.WaitingFirstPlayer;
        } else if (_PlayerData.data.Count == 1) {
            _State = GameMasterState.WaitingSecondPlayer;
        } else {
            _State = GameMasterState.WaitingMaxPlayers;
        }
    }

    private void _PeerConnected(int peerId) {
        _Logger.InfoMN(GetTree().GetNetworkUniqueId(), "_Server_PeerConnected", peerId);

        var playerName = _Server.GetPlayers()[peerId];
        _RegisterPlayer(peerId, playerName);
        _StartClientGame(peerId);
    }

    private void _PeerDisconnected(int peerId) {
        _Logger.InfoMN(GetTree().GetNetworkUniqueId(), "_Server_PeerDisconnected", peerId);

        if (_Cars.ContainsKey(peerId)) {
            _Server.RemoveSynchronizedNode(_Cars[peerId]);
            _Cars.Remove(peerId);
        }
        _PlayerData.data.Remove(peerId);
    }

    private void _NodeOutOfLimits(Node node) {
        if (node is Car car) {
            car.ResetMovement();
            car.Translation = new Vector3();
            car.Rotation = new Vector3();
            car.Translate(car.Transform.basis.y * 10);
        }
    }

    private void _RegisterPlayer(int peerId, string name) {
        _Logger.InfoM("_RegisterPlayer", $"Registering player '{peerId}' (name: {name})");

        _PlayerData.data[peerId] = new PlayerData() {
            Name = name,
            Score = 0
        };

        _SendPlayerScores();
    }

    private void _StartClientGame(int peerId) {
        _Logger.InfoMN(GetTree().GetNetworkUniqueId(), "_StartClientGame", peerId);

        var car = _Server.SpawnSynchronizedScene<Car>(
            "/root/Game", "res://scenes/common/Car.tscn",
            masterConfiguration: new Dictionary<NodePath, int> {
                { "InputController", peerId }
            }
        );
        car.Translate(car.Transform.basis.y * 10);
        _Cars.Add(peerId, car);
    }

    private void _SendPlayerScores() {
        var bytes = BinaryConverter.Serialize(_PlayerData);
        Rpc(nameof(GameMasterClient.ReceivePlayerScores), bytes);
    }
}
