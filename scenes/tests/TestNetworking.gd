extends Spatial

onready var _client_button := $GUI/Container/Client as Button
onready var _server_button := $GUI/Container/Server as Button
onready var _listen_server_button := $GUI/Container/ListenServer as Button
onready var _camera := $ChaseCamera as ChaseCamera

var _server_peer: SxServerPeer
var _client_peer: SxClientPeer
var _cars := {}
var _logger := SxLog.get_logger("TestNetworking")

func _ready() -> void:
    _client_button.connect("pressed", self, "_create_client")
    _server_button.connect("pressed", self, "_create_server")
    _listen_server_button.connect("pressed", self, "_create_listen_server")

    MainRPCService.client.connect("spawned_from_server", self, "_on_client_node_spawned")

func _create_client() -> void:
    _client_button.disabled = true
    _server_button.disabled = true
    _client_peer = SxClientPeer.new()
    _client_peer.server_address = "127.0.0.1"
    _client_peer.server_port = 12341
    _client_peer.connect("connected_to_server", self, "_on_client_connected")
    add_child(_client_peer)

func _create_server() -> void:
    _client_button.disabled = true
    _server_button.disabled = true
    _listen_server_button.disabled = true

    _server_peer = SxServerPeer.new()
    _server_peer.server_port = 12341
    _server_peer.max_players = 10
    add_child(_server_peer)
    _start_server_game()

func _on_client_connected():
    _listen_server_button.disabled = true

func _on_client_node_spawned(node: Node) -> void:
    if node is Car:
        var car := node as Car
        if car.is_owned_by_current_peer():
            car.get_input_controller().show_virtual_controls()
            _camera.target = car.camera_target

func _server_peer_connected(peer_id: int) -> void:
    _start_client_game(peer_id)

func _server_peer_disconnected(peer_id: int) -> void:
    if _cars.has(peer_id):
        _server_peer.remove_synchronized_node(_cars[peer_id])
        _cars.erase(peer_id)

func _create_listen_server() -> void:
    _listen_server_button.disabled = true
    _server_button.disabled = true

    var server = SxListenServerPeer.new()
    server.server_port = 12341
    server.max_players = 10
    add_child(server)

    # Spawn transform on server
    var scene = Spatial.new()
    scene.name = "TestNetworking"
    server.get_server_root().add_child(scene)
    _server_peer = server.get_server()
    _start_server_game()

func _start_server_game() -> void:
    _server_peer.connect("peer_connected", self, "_server_peer_connected")
    _server_peer.connect("peer_disconnected", self, "_server_peer_disconnected")

    var node := _server_peer.spawn_synchronized_scene("/root/TestNetworking", "res://scenes/tests/TestLevel.tscn") as Node
    var limits := _server_peer.spawn_synchronized_scene(node.get_path(), "res://scenes/game/LevelLimits.tscn") as LevelLimits
    limits.connect("out_of_limits", self, "_node_out_of_limits")

func _start_client_game(peer_id: int) -> void:
    var car := _server_peer.spawn_synchronized_scene(
        "/root/TestNetworking", "res://scenes/common/Car.tscn", 1,
        {
            "InputController": peer_id
        }
    ) as Car
    car.translate(car.transform.basis.y * 10)
    _cars[peer_id] = car

func _node_out_of_limits(node: Node) -> void:
    if node is Car:
        var car := node as Car
        car.reset_movement()
        car.translation = Vector3()
        car.rotation = Vector3()
        car.translate(car.transform.basis.y * 10)
