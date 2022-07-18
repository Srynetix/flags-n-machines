extends Node

var _registry := {}
var _logger := SxLog.get_logger("NodeRegistry")

func _ready() -> void:
    name = "NodeRegistry"

func show_registry() -> void:
    if len(_registry) == 0:
        print("Empty registry.")
    else:
        for key in _registry:
            var value = _registry[key]
            print("- %s: %s" % [key, value])

func register_path(name: String, path: NodePath) -> void:
    register_node(name, get_node(path))

func register_node(name: String, node: Node) -> void:
    if _registry.has(name):
        _logger.warn("Node '%s' already registered. Overwriting ..." % name)
    else:
        _logger.debug("Registering node '%s' in registry of type '%s'" % [name, node])

    _registry[name] = node

func get_node_from_registry(name: String) -> Node:
    if !_registry.has(name):
        _logger.error("Unknown node '%s'." % name)
        return null
    else:
        return _registry[name]