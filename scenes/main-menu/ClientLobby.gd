extends ColorRect
class_name ClientLobby

func _ready() -> void:
    yield(get_tree().create_timer(1.0), "timeout")
    get_tree().change_scene("res://scenes/game/ClientGame.tscn")