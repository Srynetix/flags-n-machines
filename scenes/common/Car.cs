using Godot;

public class Car : CarEngine
{
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        SteerLeftInput = Input.GetActionStrength("steer_left");
        SteerRightInput = Input.GetActionStrength("steer_right");
        BrakeInput = Input.IsActionPressed("brake");
        AccelerateInput = Input.IsActionPressed("accelerate");
        JumpInput = Input.IsActionJustPressed("jump");
    }
}
