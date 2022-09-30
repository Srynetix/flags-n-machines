extends Spatial
class_name ServerGame

var _listen_server_peer: SxListenServerPeer
var _server_peer: SxServerPeer
var _client_peer: SxClientPeer
var _logger := SxLog.get_logger("ServerGame")

func _init() -> void:
    name = "Game"

func _ready() -> void:
    _listen_server_peer = SxListenServerPeer.new()
    _listen_server_peer.server_port = CVars.get_var("host_server_port")
    _listen_server_peer.max_players = CVars.get_var("host_max_players")
    add_child(_listen_server_peer)
    _server_peer = _listen_server_peer.get_server()

    var game_stub := Spatial.new()
    game_stub.name = "Game"
    _listen_server_peer.get_server_root().add_child(game_stub)
    _server_peer.spawn_synchronized_scene_mapped("/root/Game", "GameMaster", "res://scenes/game/GameMasterServer.tscn", "res://scenes/game/GameMasterClient.tscn")

    _client_peer = SxClientPeer.new()
    _client_peer.server_address = "127.0.0.1"
    _client_peer.server_port = CVars.get_var("host_server_port")
    add_child(_client_peer)
