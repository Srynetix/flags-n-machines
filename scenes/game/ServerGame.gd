extends Spatial
class_name ServerGame

var _listen_server_peer: ListenServerPeer
var _server_peer: ServerPeer
var _client_peer: ClientPeer
var _logger := SxLog.get_logger("ServerGame")

func _init() -> void:
    name = "Game"

func _ready() -> void:
    _listen_server_peer = ListenServerPeer.new()
    _listen_server_peer.server_port = CVars.get_var("host_server_port")
    _listen_server_peer.max_players = CVars.get_var("host_max_players")
    add_child(_listen_server_peer)
    _server_peer = _listen_server_peer.get_server()
    MainNodeRegistry.register_node("server", _server_peer)

    var listen_registry := NodeRegistry.new()
    listen_registry.register_node("server", _server_peer)

    var game_stub := Spatial.new()
    game_stub.name = "Game"
    _listen_server_peer.get_server_root().add_child(listen_registry)
    _listen_server_peer.get_server_root().add_child(game_stub)
    _server_peer.spawn_synchronized_scene_mapped("/root/Game", "GameMaster", "res://scenes/game/GameMasterServer.tscn", "res://scenes/game/GameMasterClient.tscn")

    _client_peer = ClientPeer.new()
    _client_peer.server_address = "127.0.0.1"
    _client_peer.server_port = CVars.get_var("host_server_port")
    add_child(_client_peer)
