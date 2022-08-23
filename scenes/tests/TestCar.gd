extends Spatial

onready var _car := $Car as Car
onready var _initial_car_transform := _car.transform
onready var _world_camera := $WorldCamera as Camera
onready var _chase_camera := $ChaseCamera as ChaseCamera

func _ready() -> void:
    _chase_camera.target = _car.camera_target
    get_node("LevelLimits").connect("out_of_limits", self, "_out_of_limits")

func _input(event: InputEvent) -> void:
    if event is InputEventKey:
        var key_event := event as InputEventKey
        if key_event.pressed:
            if key_event.physical_scancode == KEY_ENTER:
                _reset_car(_car)

            elif key_event.physical_scancode == KEY_TAB:
                _world_camera.current = !_world_camera.current

func _out_of_limits(node: Node) -> void:
    if node is Car:
        _reset_car(node)

func _reset_car(car: Car) -> void:
    car.reset_movement()
    car.transform = _initial_car_transform