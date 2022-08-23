extends KinematicBody
class_name CarEngine

# Mostly implemented from https://kidscancode.org/godot_recipes/3d/kinematic_car/

var gravity := -20.0
var wheel_base := 0.6
var steering_limit := 10.0
var engine_power := 12.0
var braking := -9.0
var friction := -2.0
var drag := -2.0
var max_speed_forward := 20.0
var max_speed_reverse := 3.0
var max_car_steering_limit := 0.085
var slip_speed := 15.0
var traction_slow := 0.75
var traction_fast := 0.02
var jump_force := 8.0
var air_control_velocity := 10.0
var air_control_steer_velocity := 10.0

var jump_enabled := true

var _drifting := false
var _jumping := false
var _acceleration := Vector3()
var _velocity := Vector3()
var _steer_angle := 0.0
var _forward_steer_angle := 0.0

onready var camera_target := $CameraTarget as Position3D
onready var _input_controller := $InputController as CarInputController
onready var _mesh := $Mesh as Spatial
onready var _drift_particles := $DriftParticles as CPUParticles
onready var _front_ray := $FrontRay as RayCast
onready var _rear_ray := $RearRay as RayCast
onready var _node_tracer := $NodeTracer as SxNodeTracer
onready var _front_left_wheel := $Mesh/wheel_standard3 as MeshInstance
onready var _front_right_wheel := $Mesh/wheel_standard4 as MeshInstance

func _physics_process(delta: float) -> void:
    _get_input(delta)

    if is_on_floor() && !_jumping:
        _apply_friction(delta)
        _calculate_steering(delta)
    else:
        # No drift in mid-air
        _calculate_air_steering(delta)
        _calculate_air_friction(delta)
        _drifting = false

    # Apply gravity on car
    var gravity_vector := transform.basis.y
    if !_front_ray.is_colliding() && !_rear_ray.is_colliding():
        global_transform = SxMath.interpolate_align_with_y(global_transform, Vector3.UP, 0.01)
        gravity_vector = Vector3.UP

    _acceleration += gravity * gravity_vector * delta
    _velocity += _acceleration * delta

    var up_vector := transform.basis.y
    if _jumping:
        _velocity = move_and_slide(_velocity, up_vector, false)
    else:
        _velocity = move_and_slide_with_snap(_velocity, -transform.basis.y, up_vector, false)

    # Detect floor
    if _front_ray.is_colliding() || _rear_ray.is_colliding():
        var nf := _front_ray.get_collision_normal() if _front_ray.is_colliding() else transform.basis.y
        var nr := _rear_ray.get_collision_normal() if _rear_ray.is_colliding() else transform.basis.y
        var n := ((nr + nf) / 2.0).normalized()
        if n != Vector3():
            var transform := SxMath.align_with_y(global_transform, n)
            global_transform = global_transform.interpolate_with(transform, 0.1)

    # Show drift particles
    _drift_particles.emitting = _drifting
    _node_tracer.trace_parameter("Acceleration", _acceleration)
    _node_tracer.trace_parameter("Velocity", _velocity)
    _node_tracer.trace_parameter("Position", global_transform.origin)
    _node_tracer.trace_parameter("Steer angle", _steer_angle)
    _node_tracer.trace_parameter("Forward steer angle", _forward_steer_angle)
    _node_tracer.trace_parameter("Drifting?", _drifting)
    _node_tracer.trace_parameter("Jumping?", _jumping)
    _node_tracer.trace_parameter("On floor?", is_on_floor())

func _get_input(delta: float) -> void:
    var turn := _input_controller.steer_left
    turn -= _input_controller.steer_right
    _steer_angle = turn * deg2rad(steering_limit)

    var forward_turn := 0
    if _input_controller.accelerate:
        forward_turn += 1
    if _input_controller.brake:
        forward_turn -= 1
    _forward_steer_angle = forward_turn * deg2rad(steering_limit)
    _front_left_wheel.rotation = Vector3(_front_left_wheel.rotation.x, _steer_angle * 2.0, _front_left_wheel.rotation.z)
    _front_right_wheel.rotation = Vector3(_front_right_wheel.rotation.x, _steer_angle * 2.0, _front_right_wheel.rotation.z)

    var y_rotation = clamp((_mesh.rotation.y * 0.95) + (_steer_angle * 4 * delta), -max_car_steering_limit, max_car_steering_limit)
    _mesh.rotation = Vector3(_mesh.rotation.x, y_rotation, _mesh.rotation.z)

    if is_on_floor():
        # Handle acceleration
        _acceleration = Vector3()
        if _input_controller.accelerate:
            _acceleration = -transform.basis.z * engine_power
        elif _input_controller.brake:
            _acceleration = -transform.basis.z * braking

        # Handle jump
        if _jumping:
            _jumping = false
        elif jump_enabled && _input_controller.jump && !_jumping:
            _velocity += transform.basis.y * jump_force
            _jumping = true

func _apply_friction(delta: float) -> void:
    # Make the car brake automatically at low speed (disable 'soap effect')
    if _velocity.length() < 1 && _acceleration.length() == 0:
        _velocity = Vector3()

    var friction_force := _velocity * friction * delta
    var drag_force := _velocity * _velocity.length() * drag * delta
    _acceleration += drag_force + friction_force

func _calculate_air_steering(delta: float) -> void:
    rotate(transform.basis.y, _steer_angle * delta * air_control_steer_velocity)
    rotate(transform.basis.x, -_forward_steer_angle * delta * air_control_velocity / 3)

func _calculate_air_friction(_delta: float) -> void:
    # _acceleration += -gravity * transform.basis.y * delta * 0.5
    pass

func _calculate_steering(delta: float) -> void:
    var rear_wheel := transform.origin + transform.basis.z * wheel_base / 2.0
    var front_wheel := transform.origin - transform.basis.z * wheel_base / 2.0
    rear_wheel += _velocity * delta
    front_wheel += _velocity.rotated(transform.basis.y.normalized(), _steer_angle) * delta
    var new_heading := rear_wheel.direction_to(front_wheel)

    if !_drifting && _velocity.length() > slip_speed && abs(_steer_angle) > 0:
        _drifting = true
    if _drifting && _steer_angle == 0:
        _drifting = false
    var traction = traction_fast if _drifting else traction_slow

    var d = new_heading.dot(_velocity.normalized())
    if d > 0:
        _velocity = SxMath.lerp_vector3(_velocity, new_heading * min(_velocity.length(), max_speed_forward), traction)
    elif d < 0:
        _velocity = -new_heading * min(_velocity.length(), max_speed_reverse)

    look_at(transform.origin + new_heading, transform.basis.y)

func reset_movement() -> void:
    _acceleration = Vector3()
    _velocity = Vector3()

func get_input_controller() -> CarInputController:
    return _input_controller

func _network_send() -> Dictionary:
    return {
        "transform": transform,
        "mesh_transform": _mesh.transform,
        "front_left_wheel_rot": _front_left_wheel.rotation,
        "front_right_wheel_rot": _front_right_wheel.rotation,
        "drift_particles_enabled": _drift_particles.emitting
    }

func _network_receive(data: Dictionary) -> void:
    if data.has("transform"):
        transform = data["transform"]
    if data.has("mesh_transform"):
        _mesh.transform = data["mesh_transform"]
    if data.has("front_left_wheel_rot"):
        _front_left_wheel.rotation = data["front_left_wheel_rot"]
    if data.has("front_right_wheel_rot"):
        _front_right_wheel.rotation = data["front_right_wheel_rot"]
    if data.has("drift_particles_enabled"):
        _drift_particles.emitting = data["drift_particles_enabled"]