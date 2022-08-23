extends Area
class_name LevelLimits

signal out_of_limits(node)

export var monitored_nodes := []

var _monitored_node_instances := []
var _initial_transforms := []
var _logger := SxLog.get_logger("LevelLimits")

func _ready() -> void:
    connect("body_exited", self, "_on_body_exited")
    for path in monitored_nodes:
        var node := get_node(path) as Node
        if node is Spatial:
            var spatial := node as Spatial
            _initial_transforms.append(spatial.global_transform)
            _monitored_node_instances.append(spatial)
        else:
            _logger.error_m("_ready", "Unsupported node type %s" % node)

func _on_body_exited(node: Node) -> void:
    if node is Spatial:
        var spatial := node as Spatial
        var index := _monitored_node_instances.find(spatial)
        if index >= 0:
            spatial.global_transform = _initial_transforms[index]

    emit_signal("out_of_limits", node)