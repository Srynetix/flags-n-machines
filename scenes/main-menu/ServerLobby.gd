extends ColorRect
class_name ServerLobby

func _ready() -> void:
    yield(get_tree().create_timer(1.0), "timeout")
    get_tree().change_scene("res://scenes/game/ServerGame.tscn")