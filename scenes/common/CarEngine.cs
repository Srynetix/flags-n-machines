using Godot;
using Godot.Collections;

// Mostly implemented from https://kidscancode.org/godot_recipes/3d/kinematic_car/
public class CarEngine : KinematicBody, ISynchronizable
{
    // Public state
    public float Gravity = -40.0f;
    public float WheelBase = 0.6f;
    public float SteeringLimit = 10.0f;
    public float EnginePower = 12.0f;
    public float Braking = -9.0f;
    public float Friction = -2.0f;
    public float Drag = -2.0f;
    public float MaxSpeedForward = 20.0f;
    public float MaxSpeedReverse = 3.0f;
    public float MaxCarSteeringLimit = 0.085f;
    public float SlipSpeed = 15.0f;
    public float TractionSlow = 0.75f;
    public float TractionFast = 0.02f;
    public float JumpForce = 10.0f;
    public float AirControlVelocity = 10.0f;
    public float AirControlSteerVelocity = 10.0f;

    // Toggles
    public bool JumpEnabled = true;

    // Internal state
    protected bool _Drifting = false;
    protected bool _Jumping = false;
    protected Vector3 _Acceleration = Vector3.Zero;
    protected Vector3 _Velocity = Vector3.Zero;
    protected float _SteerAngle = 0;
    protected float _ForwardSteerAngle = 0;

    // Child nodes
    public Position3D CameraTarget;
    protected CarInputController _InputController;
    protected Spatial _Mesh;
    protected CPUParticles _DriftParticles;
    protected RayCast _FrontRay;
    protected RayCast _RearRay;
    protected NodeTracer _NodeTracer;
    protected MeshInstance _FrontLeftWheel;
    protected MeshInstance _FrontRightWheel;

    public override void _Ready()
    {
        _InputController = GetNode<CarInputController>("InputController");
        _Mesh = GetNode<Spatial>("Mesh");
        _DriftParticles = GetNode<CPUParticles>("DriftParticles");
        _FrontRay = GetNode<RayCast>("FrontRay");
        _RearRay = GetNode<RayCast>("RearRay");
        _NodeTracer = GetNode<NodeTracer>("NodeTracer");
        _FrontLeftWheel = GetNode<MeshInstance>("Mesh/wheel_standard3");
        _FrontRightWheel = GetNode<MeshInstance>("Mesh/wheel_standard4");

        CameraTarget = GetNode<Position3D>("CameraTarget");
    }

    public override void _PhysicsProcess(float delta)
    {
        _GetInput(delta);

        if (IsOnFloor() && !_Jumping) {
            _ApplyFriction(delta);
            _CalculateSteering(delta);
        } else {
            // No drift in mid-air
            _CalculateAirSteering(delta);
            _Drifting = false;
        }

        /// Apply gravity on car
        var gravityVector = Transform.basis.y;
        if (!_FrontRay.IsColliding() && !_RearRay.IsColliding()) {
            GlobalTransform = MathExt.InterpolateAlignWithY(GlobalTransform, Vector3.Up, 0.01f);
            gravityVector = Vector3.Up;
        }

        _Acceleration += Gravity * gravityVector * delta;
        _Velocity += _Acceleration * delta;

        var upVector = Transform.basis.y;
        if (_Jumping) {
            _Velocity = MoveAndSlide(_Velocity, upVector, false);
        } else {
            _Velocity = MoveAndSlideWithSnap(_Velocity, -Transform.basis.y, upVector, false);
        }

        // Detect floor
        if (_FrontRay.IsColliding() || _RearRay.IsColliding()) {
            var nf = _FrontRay.IsColliding() ? _FrontRay.GetCollisionNormal() : Transform.basis.y;
            var nr = _RearRay.IsColliding() ? _RearRay.GetCollisionNormal() : Transform.basis.y;
            var n = ((nr + nf) / 2.0f).Normalized();
            if (n != Vector3.Zero) {
                var transform = MathExt.AlignWithY(GlobalTransform, n);
                GlobalTransform = GlobalTransform.InterpolateWith(transform, 0.1f);
            }
        }

        // Show drift particles
        _DriftParticles.Emitting = _Drifting;

        _NodeTracer.TraceParameter("Acceleration", _Acceleration);
        _NodeTracer.TraceParameter("Velocity", _Velocity);
        _NodeTracer.TraceParameter("Position", GlobalTransform.origin);
        _NodeTracer.TraceParameter("Steer angle", _SteerAngle);
        _NodeTracer.TraceParameter("Forward steer angle", _ForwardSteerAngle);
        _NodeTracer.TraceParameter("Drifting?", _Drifting);
        _NodeTracer.TraceParameter("Jumping?", _Jumping);
        _NodeTracer.TraceParameter("On floor?", IsOnFloor());
    }

    private void _GetInput(float delta) {
        var turn = _InputController.SteerLeft;
        turn -= _InputController.SteerRight;
        _SteerAngle = turn * Mathf.Deg2Rad(SteeringLimit);

        var forwardTurn = 0;
        if (_InputController.Accelerate) {
            forwardTurn += 1;
        }
        if (_InputController.Brake) {
            forwardTurn -= 1;
        }
        _ForwardSteerAngle = forwardTurn * Mathf.Deg2Rad(SteeringLimit);

        _FrontLeftWheel.Rotation = new Vector3(_FrontLeftWheel.Rotation.x, _SteerAngle * 2.0f, _FrontLeftWheel.Rotation.z);
        _FrontRightWheel.Rotation = new Vector3(_FrontRightWheel.Rotation.x, _SteerAngle * 2.0f, _FrontRightWheel.Rotation.z);

        var yRotation = Mathf.Clamp((_Mesh.Rotation.y * 0.95f) + (_SteerAngle * 4 * delta), -MaxCarSteeringLimit, MaxCarSteeringLimit);
        _Mesh.Rotation = new Vector3(_Mesh.Rotation.x, yRotation, _Mesh.Rotation.z);

        if (IsOnFloor()) {
            // Handle acceleration
            _Acceleration = Vector3.Zero;
            if (_InputController.Accelerate) {
                _Acceleration = -Transform.basis.z * EnginePower;
            } else if (_InputController.Brake) {
                _Acceleration = -Transform.basis.z * Braking;
            }

            // Handle jump
            if (_Jumping) {
                _Jumping = false;
            } else if (JumpEnabled && _InputController.Jump && !_Jumping) {
                _Velocity += Transform.basis.y * JumpForce;
                _Jumping = true;
            }
        }
    }

    private void _ApplyFriction(float delta) {
        // Make the car brake automatically at low speed (disable 'soap effect')
        if (_Velocity.Length() < 1 && _Acceleration.Length() == 0) {
            _Velocity = Vector3.Zero;
        }

        var frictionForce = _Velocity * Friction * delta;
        var dragForce = _Velocity * _Velocity.Length() * Drag * delta;
        _Acceleration += dragForce + frictionForce;
    }

    private void _CalculateAirSteering(float delta) {
        Rotate(Transform.basis.y, _SteerAngle * delta * AirControlSteerVelocity);
        Rotate(Transform.basis.x, -_ForwardSteerAngle * delta * AirControlVelocity / 3);
    }

    private void _CalculateSteering(float delta) {
        var rearWheel = Transform.origin + Transform.basis.z * WheelBase / 2.0f;
        var frontWheel = Transform.origin - Transform.basis.z * WheelBase / 2.0f;
        rearWheel += _Velocity * delta;
        frontWheel += _Velocity.Rotated(Transform.basis.y.Normalized(), _SteerAngle) * delta;
        var newHeading = rearWheel.DirectionTo(frontWheel);

        // Traction
        if (!_Drifting && _Velocity.Length() > SlipSpeed && Mathf.Abs(_SteerAngle) > 0) {
            _Drifting = true;
        }
        if (_Drifting && _SteerAngle == 0) {
            _Drifting = false;
        }
        var traction = _Drifting ? TractionFast : TractionSlow;

        var d = newHeading.Dot(_Velocity.Normalized());
        if (d > 0) {
            _Velocity = MathExt.LerpVector3(_Velocity, newHeading * Mathf.Min(_Velocity.Length(), MaxSpeedForward), traction);
        } else if (d < 0) {
            _Velocity = -newHeading * Mathf.Min(_Velocity.Length(), MaxSpeedReverse);
        }

        LookAt(Transform.origin + newHeading, Transform.basis.y);
    }

    public void ResetMovement() {
        _Acceleration = Vector3.Zero;
        _Velocity = Vector3.Zero;
    }

    public CarInputController GetInputController() {
        return _InputController;
    }

    public Dictionary<string, object> _NetworkSend() {
        return new Dictionary<string, object>() {
            { "transform", Transform },
            { "mesh_transform", _Mesh.Transform },
            { "front_left_wheel_rot", _FrontLeftWheel.Rotation },
            { "front_right_wheel_rot", _FrontRightWheel.Rotation },
            { "drift_particles_enabled", _DriftParticles.Emitting }
        };
    }

    public void _NetworkReceive(Dictionary<string, object> data) {
        if (data.ContainsKey("transform")) {
            var transform = data["transform"];
            if (transform is Transform t) {
                Transform = t;
            }
        }

        if (data.ContainsKey("mesh_transform")) {
            var transform = data["mesh_transform"];
            if (transform is Transform t) {
                _Mesh.Transform = t;
            }
        }

        if (data.ContainsKey("drift_particles_enabled")) {
            var value = data["drift_particles_enabled"];
            if (value is bool v) {
                _DriftParticles.Emitting = v;
            }
        }

        if (data.ContainsKey("front_left_wheel_rot")) {
            var value = data["front_left_wheel_rot"];
            if (value is Vector3 v) {
                _FrontLeftWheel.Rotation = v;
            }
        }

        if (data.ContainsKey("front_right_wheel_rot")) {
            var value = data["front_right_wheel_rot"];
            if (value is Vector3 v) {
                _FrontRightWheel.Rotation = v;
            }
        }
    }
}
