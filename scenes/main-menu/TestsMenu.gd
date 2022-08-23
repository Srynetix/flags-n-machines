extends ColorRect
class_name TestsMenu

onready var _back_button: SxFAButton = $MainMargin/MainRows/TopColumn/BackButton
onready var _test_selection: HBoxContainer = $MainMargin/MainRows/MiddleColumn/Parameters/TestSelection/Values

func _ready() -> void:
    _back_button.connect("pressed", self, "_go_back")

    _test_selection.get_node("Car").connect("pressed", self, "_select_test", [ "TestCar" ])
    _test_selection.get_node("Networking").connect("pressed", self, "_select_test", [ "TestNetworking" ])
    _test_selection.get_node("CSG").connect("pressed", self, "_select_test", [ "TestCSG" ])

func _notification(what: int) -> void:
    match what:
        MainLoop.NOTIFICATION_WM_GO_BACK_REQUEST:
            _go_back()

func _go_back() -> void:
    get_tree().change_scene("res://scenes/main-menu/MainMenu.tscn")

func _select_test(path: String) -> void:
    get_tree().change_scene("res://scenes/tests/%s.tscn" % path)