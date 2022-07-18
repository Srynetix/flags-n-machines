extends ColorRect
class_name SettingsMenu

onready var _player_name: LineEdit = $MainMargin/MainRows/MiddleColumn/ParameterRows/PlayerName/Input
onready var _back_button: SxFAButton = $MainMargin/MainRows/TopColumn/BackButton
onready var _save_button: Button = $MainMargin/MainRows/BottomColumn/SaveButton

func _ready() -> void:
    _back_button.connect("pressed", self, "_go_back")
    _save_button.connect("pressed", self, "_save_settings")
    _player_name.text = PlayerContext.player_name

func _notification(what: int):
    match what:
        MainLoop.NOTIFICATION_WM_GO_BACK_REQUEST:
            _go_back()

func _go_back() -> void:
    get_tree().change_scene("res://scenes/main-menu/MainMenu.tscn")

func _save_settings() -> void:
    PlayerContext.player_name = _player_name.text