using Godot;

public class FPSCamera : Camera
{
    private const float MIN_LOOK_ANGLE = -90;
    private const float MAX_LOOK_ANGLE = 90;

    [Export]
    public float MovementSpeed = 20;
    [Export]
    public float LookSensitivity = 10;

    private Vector2 _MouseDelta;

    public override void _Ready()
    {
        // Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Process(float delta)
    {
        var movement = Vector2.Zero;

        if (Input.IsActionPressed("steer_left")) {
            movement.x -= 1;
        }

        if (Input.IsActionPressed("steer_right")) {
            movement.x += 1;
        }

        if (Input.IsActionPressed("accelerate")) {
            movement.y -= 1;
        }

        if (Input.IsActionPressed("brake")) {
            movement.y += 1;
        }

        movement = movement.Normalized();
        var forward = Transform.basis.z;
        var right = Transform.basis.x;
        var relativeDir = (forward * movement.y + right * movement.x);
        Translation += relativeDir * delta * MovementSpeed;

        var rotation = RotationDegrees;
        rotation.x -= _MouseDelta.y * LookSensitivity * delta;
        rotation.x = Mathf.Clamp(rotation.x, MIN_LOOK_ANGLE, MAX_LOOK_ANGLE);
        rotation.y -= _MouseDelta.x * LookSensitivity * delta;
        RotationDegrees = rotation;

        _MouseDelta = Vector2.Zero;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motionEvent) {
            _MouseDelta = motionEvent.Relative;
        }
    }
}
