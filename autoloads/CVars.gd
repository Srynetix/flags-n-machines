extends Node

class _GameData:
    extends SxGameData

    func _ready() -> void:
        default_file_path = "user://cvars.dat"
        var logger = SxLog.get_logger("SxGameData")
        logger.set_max_log_level(SxLog.LogLevel.DEBUG)

var _store: _GameData
var _logger := SxLog.get_logger("CVars")
var _known_cvars := {}

func _ready():
    _logger.set_max_log_level(SxLog.LogLevel.DEBUG)

    # Prepare store
    _store = _GameData.new()
    add_child(_store)
    _store.load_from_disk()

    initialize_vars()

func initialize_vars() -> void:
    _register_var("host_default_max_players", 8);
    _register_var("host_default_server_port", 13795);
    _register_var("join_default_server_address", "127.0.0.1");
    _register_var("host_max_players", 8);
    _register_var("host_server_port", 13795);
    _register_var("join_server_address", "127.0.0.1");
    _register_var("join_server_port", 13795);

func get_var(name: String):
    if _known_cvars.has(name):
        return _known_cvars[name]
    else:
        _logger.error("Unknown CVar '%s'" % name)
        return null

func set_var(name: String, value) -> void:
    if _known_cvars.has(name):
        _known_cvars[name] = value
        _store.store_value(name, value)
        _store.persist_to_disk()
    else:
        _logger.error("Unknown CVar '%s'" % name)

func _register_var(name: String, default_value) -> void:
    _logger.debug("Registering CVar '%s' with default value '%s'" % [name, default_value])
    _known_cvars[name] = default_value
    _store.store_value(name, default_value)