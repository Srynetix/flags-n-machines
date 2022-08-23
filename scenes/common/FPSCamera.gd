extends Camera
class_name FPSCamera

const MIN_LOOK_ANGLE := -90
const MAX_LOOK_ANGLE := 90

export var movement_speed := 20
export var look_sensitivity := 10

var _mouse_delta := Vector2()

func _process(delta: float) -> void:
    var movement := Vector2()

    if Input.is_action_pressed("steer_left"):
        movement.x -= 1
    if Input.is_action_pressed("steer_right"):
        movement.x += 1
    if Input.is_action_pressed("accelerate"):
        movement.y -= 1
    if Input.is_action_pressed("brake"):
        movement.y += 1

    movement = movement.normalized()
    var forward := transform.basis.z
    var right := transform.basis.x
    var relative_dir := (forward * movement.y + right * movement.x)
    translation += relative_dir * delta * movement_speed

    var rotation = rotation_degrees
    rotation.x -= _mouse_delta.y * look_sensitivity * delta
    rotation.x = clamp(rotation.x, MIN_LOOK_ANGLE, MAX_LOOK_ANGLE)
    rotation.y -= _mouse_delta.x * look_sensitivity * delta
    rotation_degrees = rotation

    _mouse_delta = Vector2()

func _input(event: InputEvent) -> void:
    if event is InputEventMouseMotion:
        var motion_event := event as InputEventMouseMotion
        _mouse_delta = motion_event.relative