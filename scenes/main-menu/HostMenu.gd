extends ColorRect
class_name HostMenu

onready var _start_button: Button = $MainMargin/MainRows/BottomColumn/StartButton
onready var _max_players: LineEdit = $MainMargin/MainRows/MiddleColumn/Parameters/MaxPlayersInput/Value
onready var _server_port: LineEdit = $MainMargin/MainRows/MiddleColumn/Parameters/PortInput/Value
onready var _back_button: SxFAButton = $MainMargin/MainRows/TopColumn/BackButton
onready var _maps_container: HBoxContainer = $MainMargin/MainRows/MiddleColumn/Parameters/MapSelection/Values

var _selected_map := ""
var _map_buttons := []

func _ready() -> void:
    _start_button.connect("pressed", self, "_start_game")
    _start_button.disabled = true

    _max_players.connect("text_changed", self, "_validate_line_edit")
    _max_players.text = str(CVars.get_var("host_default_max_players"))
    
    _server_port.connect("text_changed", self, "_validate_line_edit")
    _server_port.text = str(CVars.get_var("host_default_server_port"))

    _back_button.connect("pressed", self, "_go_back")
    _map_buttons = _maps_container.get_children()

    for node in _map_buttons:
        var button: Button = node
        if !button.disabled:
            button.connect("toggled", self, "_select_map", [button])

    _update_form()

func _notification(what: int) -> void:
    match what:
        MainLoop.NOTIFICATION_WM_GO_BACK_REQUEST:
            _go_back()

func _go_back() -> void:
    get_tree().change_scene("res://scenes/main-menu/MainMenu.tscn")

func _select_map(toggle_status: bool, button: Button) -> void:
    if toggle_status:
        # Reset all other buttons
        for node in _map_buttons:
            var other_button: Button = node
            if other_button != button:
                other_button.pressed = false

        _selected_map = button.text

    elif _selected_map == button.text:
        _selected_map = ""

    _update_form()

func _update_form() -> void:
    _start_button.disabled = !_check_parameters_validation()

func _start_game() -> void:
    CVars.set_var("host_server_port", int(_server_port.text))
    CVars.set_var("host_max_players", int(_max_players.text))

    get_tree().change_scene("res://scenes/main-menu/ServerLobby.tscn")

func _validate_line_edit(_value: String) -> void:
    _update_form()

func _check_parameters_validation() -> bool:
    if _selected_map != "":
        var max_players = int(_max_players.text)
        if max_players > 0:
            var server_port = int(_server_port.text)
            if server_port > 0:
                return true

    return false
