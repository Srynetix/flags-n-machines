extends Node

class _GameData:
    extends SxGameData

    func _ready() -> void:
        default_file_path = "user://player.dat"

var player_name: String setget _set_player_name

var _store: _GameData

func _ready() -> void:
    _store = _GameData.new()
    add_child(_store)
    _store.load_from_disk()

    player_name = _store.load_value("player_name", "Dummy")

func _set_player_name(value: String) -> void:
    player_name = value
    _store.store_value("player_name", player_name)
    _store.persist_to_disk()