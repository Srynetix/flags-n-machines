extends ColorRect
class_name MainMenu

onready var _settings_button: SxFAButton = $MainMargin/MainRows/TopColumn/PlayerSection/SettingsButton
onready var _player_name: Label = $MainMargin/MainRows/TopColumn/PlayerSection/PlayerName
onready var _join_button: Button = $MainMargin/MainRows/MiddleColumn/JoinButton
onready var _host_button: Button = $MainMargin/MainRows/MiddleColumn/HostButton
onready var _tests_button: Button = $MainMargin/MainRows/BottomColumn/BottomRow/TestButton

func _ready() -> void:
    _join_button.connect("pressed", self, "_open_join_menu")
    _host_button.connect("pressed", self, "_open_host_menu")
    _tests_button.connect("pressed", self, "_open_tests_menu")
    _settings_button.connect("pressed", self, "_open_settings_menu")

    _player_name.text = PlayerContext.player_name
    get_tree().set_quit_on_go_back(true)

func _open_settings_menu() -> void:
    get_tree().set_quit_on_go_back(false)
    get_tree().change_scene("res://scenes/main-menu/SettingsMenu.tscn")

func _open_join_menu() -> void:
    get_tree().set_quit_on_go_back(false)
    get_tree().change_scene("res://scenes/main-menu/JoinMenu.tscn")

func _open_host_menu() -> void:
    get_tree().set_quit_on_go_back(false)
    get_tree().change_scene("res://scenes/main-menu/HostMenu.tscn")

func _open_tests_menu() -> void:
    get_tree().set_quit_on_go_back(false)
    get_tree().change_scene("res://scenes/main-menu/TestsMenu.tscn")
