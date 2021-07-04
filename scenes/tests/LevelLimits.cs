using Godot;

public class LevelLimits : Area
{
    [Signal]
    public delegate void OutOfLimits(Node node);

    public override void _Ready() {
        Connect("body_exited", this, nameof(_OnBodyExited));
    }

    private void _OnBodyExited(Node node) {
        EmitSignal(nameof(OutOfLimits), node);
    }
}
