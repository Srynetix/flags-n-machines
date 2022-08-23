extends Node
class_name GameMasterServer

enum GameMasterState {
    WAITING_FIRST_PLAYER,
    WAITING_SECOND_PLAYER,
    WAITING_MAX_PLAYERS,
    GAME_STARTED
}

var _logger := SxLog.get_logger("GameMasterServer")
var _state := GameMasterState.WAITING_FIRST_PLAYER as int
var _server_peer: ServerPeer
var _cars := {}
var _player_data := {}

func _ready() -> void:
    _logger.info_m("_ready", "GameMasterServer is ready.")

    _server_peer = MainNodeRegistry.get_node_from_registry("server") as ServerPeer
    _server_peer.connect("peer_connected", self, "_peer_connected")
    _server_peer.connect("peer_disconnected", self, "_peer_disconnected")
    _server_peer.spawn_synchronized_named_scene(
        "/root/Game",
        "res://scenes/game/GameStage.tscn",
        "Stage"
    )

    # Load player scores
    var players := _server_peer.get_players()
    for player_idx in players:
        _register_player(player_idx, players[player_idx])

    if len(_player_data) == 0:
        _state = GameMasterState.WAITING_FIRST_PLAYER
    elif len(_player_data) == 1:
        _state = GameMasterState.WAITING_SECOND_PLAYER
    else:
        _state = GameMasterState.WAITING_MAX_PLAYERS

func _peer_connected(peer_id: int) -> void:
    _logger.info_mn(SxNetwork.get_nuid(self), "_peer_connected", str(peer_id))

    var player_name = _server_peer.get_players()[peer_id]
    _register_player(peer_id, player_name)
    _start_client_game(peer_id)

func _peer_disconnected(peer_id: int) -> void:
    _logger.info_mn(SxNetwork.get_nuid(self), "_peer_disconnected", str(peer_id))

    if _cars.has(peer_id):
        _server_peer.remove_synchronized_node(_cars[peer_id])
        _cars.erase(peer_id)

    _player_data.erase(peer_id)

func _node_out_of_limits(node: Node) -> void:
    if node is Car:
        var car := node as Car
        car.reset_movement()
        car.translation = Vector3()
        car.rotation = Vector3()
        car.translate(car.transform.basis.y * 10)

func _register_player(peer_id: int, name: String) -> void:
    _logger.info_m("_register_player", "Registering player '%d' (name: %s)" % [peer_id, name])

    var data = PlayerData.new()
    data.name = name
    data.score = 0
    _player_data[peer_id] = data

    _send_player_scores()

func _start_client_game(peer_id: int) -> void:
    _logger.info_mn(SxNetwork.get_nuid(self), "_start_client_game", str(peer_id))

    var car = _server_peer.spawn_synchronized_scene(
        "/root/Game",
        "res://scenes/common/Car.tscn",
        1,
        {
            "InputController": peer_id
        }
    ) as Car
    car.translate(car.transform.basis.y * 10)
    _cars[peer_id] = car

func _send_player_scores() -> void:
    var serialized_data := {}
    for k in _player_data:
        serialized_data[k] = _player_data[k].serialize()
    var json_data := to_json(serialized_data)

    _logger.info_mn(SxNetwork.get_nuid(self), "_send_player_scores", json_data)
    rpc("receive_player_scores", json_data)
