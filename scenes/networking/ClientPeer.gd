extends Node
class_name ClientPeer

signal connected_to_server()

var server_address := ""
var server_port := 0

var _logger := SxLog.get_logger("ClientPeer")

func _init() -> void:
    name = "ClientPeer"
    _logger.set_max_log_level(SxLog.LogLevel.ERROR)

func _get_sync_input() -> SyncInput:
    return MainRPCService.sync_input

func _get_server() -> ServerRPC:
    return MainRPCService.server

func _ready() -> void:
    get_tree().connect("network_peer_connected", self, "_peer_connected")
    get_tree().connect("network_peer_disconnected", self, "_peer_disconnected")
    get_tree().connect("connected_to_server", self, "_connected_to_server")
    get_tree().connect("connection_failed", self, "_connection_failed")
    get_tree().connect("server_disconnected", self, "_server_disconnected")

    var peer = NetworkedMultiplayerENet.new()
    peer.create_client(server_address, server_port)
    peer.allow_object_decoding = true
    get_tree().network_peer = peer

    _logger.debug_m("_ready", "ClientPeer is ready")

func _peer_connected(peer_id: int) -> void:
    _logger.debug_m("_peer_connected", "Peer '%d' connected." % peer_id)

func _peer_disconnected(peer_id: int) -> void:
    _logger.debug_m("_peer_disconnected", "Peer '%d' disconnected." % peer_id)

func _connected_to_server() -> void:
    _logger.info_m("_connected_to_server", "Connected to server.")

    _get_sync_input().create_peer_input(SxNetwork.get_nuid(self))
    _get_server().ping()

    emit_signal("connected_to_server")

func _connection_failed() -> void:
    _logger.info_m("_connection_failed", "Connection failed.")

func _server_disconnected() -> void:
    _logger.error_m("_server_disconnected", "Server disconnected")
    queue_free()

func _exit_tree() -> void:
    get_tree().network_peer = null

func _physics_process(_delta: float) -> void:
    var my_input = _get_sync_input().get_current_input()
    if my_input != null:
        _get_server().send_input(my_input.get_input_state())

func _input(event: InputEvent) -> void:
    if event is InputEventKey:
        var key_event: InputEventKey = event
        if key_event.scancode == KEY_F6:
            get_tree().root.print_tree_pretty()