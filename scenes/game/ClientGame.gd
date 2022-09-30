extends Spatial
class_name ClientGame

onready var _spectator_camera := $SpectatorCamera as FPSCamera
onready var _chase_camera := $ChaseCamera as ChaseCamera
var _client_peer: SxClientPeer

func _ready() -> void:
    MainRPCService.client.connect("spawned_from_server", self, "_node_spawned")

    _client_peer = SxClientPeer.new()
    _client_peer.server_address = CVars.get_var("join_server_address") as String
    _client_peer.server_port = CVars.get_var("join_server_port") as int
    add_child(_client_peer)

func _node_spawned(node: Node) -> void:
    if node is Car:
        var car := node as Car
        if car.is_owned_by_current_peer():
            car.get_input_controller().show_virtual_controls()
            _chase_camera.target = car
            _chase_camera.current = true