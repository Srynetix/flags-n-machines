extends Node
class_name ListenServerPeer

var server_port := 0
var max_players := 0

var _logger := SxLog.get_logger("ListenServerPeer")
var _scene_tree: SceneTree
var _server: ServerPeer

func _init() -> void:
    name = "ListenServerPeer"
    _logger.set_max_log_level(SxLog.LogLevel.DEBUG)

func get_server_root() -> Node:
    return _scene_tree.root

func get_server_tree() -> SceneTree:
    return _scene_tree

func get_server() -> ServerPeer:
    return _server

func print_server_tree() -> void:
    get_server_root().print_tree_pretty()

func _ready() -> void:
    _scene_tree = SceneTree.new()
    _scene_tree.init()

    var root := _scene_tree.root
    root.render_target_update_mode = Viewport.UPDATE_DISABLED

    var _rpc = RPCService.new()
    _rpc.name = "MainRPCService"
    root.add_child(_rpc)

    _server = ServerPeer.new()
    _server.server_port = server_port
    _server.max_players = max_players
    _server.rpc_service = _rpc
    root.add_child(_server)

    _logger.debug_m("_ready", "Listen server is ready.")

func _process(delta: float) -> void:
    _scene_tree.idle(delta)

func _physics_process(delta: float) -> void:
    _scene_tree.iteration(delta)

func _input(event: InputEvent) -> void:
    _scene_tree.input_event(event)

func _exit_tree() -> void:
    _scene_tree.finish()
    _logger.debug_m("_exit_tree", "Listen server exited.")
