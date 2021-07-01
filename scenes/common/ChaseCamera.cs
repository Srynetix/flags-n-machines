using Godot;

public class ChaseCamera : Camera
{
    [Export]
    public float LerpSpeed = 20.0f;

    public Spatial Target;

    public override void _PhysicsProcess(float delta)
    {
        if (Target == null) {
            return;
        }

        GlobalTransform = GlobalTransform.InterpolateWith(Target.GlobalTransform, LerpSpeed * delta);
    }
}
