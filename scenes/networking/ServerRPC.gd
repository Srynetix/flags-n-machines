extends Node
class_name ServerRPC

var _logger := SxLog.get_logger("ServerRPC")
var _service: Node  # RPCService

func _init() -> void:
    name = "ServerRPC"
    _logger.set_max_log_level(SxLog.LogLevel.DEBUG)

func link_service(service: Node) -> void:
    _service = service

func _get_client():
    return _service.client

func _get_sync_input() -> SyncInput:
    return _service.sync_input as SyncInput

func ping() -> void:
    var my_id = SxNetwork.get_nuid(self)
    _logger.debug_mn(my_id, "ping", "Sending ping to server.")
    rpc_id(1, "_ping")

func send_input(input: Dictionary) -> void:
    rpc_unreliable_id(1, "_send_input", input)

# Master network methods

master func _send_input(input: Dictionary) -> void:
    var peer_id := SxNetwork.get_sender_nuid(self)
    _get_sync_input().update_peer_input(peer_id, input)

master func _ping() -> void:
    var my_id := SxNetwork.get_nuid(self)
    var peer_id := SxNetwork.get_sender_nuid(self)

    _logger.debug_mn(my_id, "_ping", "Ping request received from peer '%d'." % peer_id)
    _get_client().pong(peer_id)
