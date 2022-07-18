extends ColorRect
class_name JoinMenu

onready var _start_button: Button = $MainMargin/MainRows/BottomColumn/StartButton
onready var _back_button: SxFAButton = $MainMargin/MainRows/TopColumn/BackButton
onready var _server_address: LineEdit = $MainMargin/MainRows/MiddleColumn/Parameters/AddressInput/Value
onready var _server_port: LineEdit = $MainMargin/MainRows/MiddleColumn/Parameters/PortInput/Value

func _ready() -> void:
    _start_button.connect("pressed", self, "_start_game")
    _start_button.disabled = true

    _back_button.connect("pressed", self, "_go_back")

    _server_address.connect("text_changed", self, "_validate_line_edit")
    _server_address.text = CVars.get_var("join_default_server_address")

    _server_port.connect("text_changed", self, "_validate_line_edit")
    _server_port.text = str(CVars.get_var("host_default_server_port"))

    _update_form()

func _notification(what: int) -> void:
    match what:
        MainLoop.NOTIFICATION_WM_GO_BACK_REQUEST:
            _go_back()

func _start_game() -> void:
    CVars.set_var("join_server_address", _server_address.text)
    CVars.set_var("join_server_port", int(_server_port.text))
    get_tree().change_scene("res://scenes/main-menu/ClientLobby.tscn")

func _validate_line_edit() -> void:
    _update_form()

func _update_form() -> void:
    _start_button.disabled = !_check_parameters_validation()

func _check_parameters_validation() -> bool:
    var server = _server_address.text
    if server != "":
        var port = int(_server_port.text)
        if port > 0:
            return true

    return false

func _go_back():
    get_tree().change_scene("res://scenes/main-menu/MainMenu.tscn")
