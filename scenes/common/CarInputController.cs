using Godot;

public class CarInputController: Node {
    public float SteerLeft;
    public float SteerRight;
    public bool Accelerate;
    public bool Brake;
    public bool Jump;

    public override void _PhysicsProcess(float delta)
    {
        var syncInput = RPCService.GetInstance(GetTree()).SyncInput;

        if (NetworkExt.IsNetworkServer(GetTree())) {
            SteerLeft = syncInput.GetActionStrength(this, "steer_left");
            SteerRight = syncInput.GetActionStrength(this, "steer_right");
            Accelerate = syncInput.IsActionPressed(this, "accelerate");
            Brake = syncInput.IsActionPressed(this, "brake");
            Jump = syncInput.IsActionPressed(this, "jump");
        }
    }
}