using Godot;

public class ChaseCamera : Camera
{
    [Export]
    public float LerpSpeed = 20.0f;
    [Export]
    public NodePath TargetPath;
    [Export]
    public string ChildNode;
    [Export]
    public float ZOffset = 5;

    public Spatial Target;

    public override void _Ready()
    {
        if (Target == null)
        {
            if (TargetPath != null)
            {
                Target = GetNode<Spatial>(TargetPath);
                if (ChildNode != null)
                {
                    Target = Target.GetNode<Spatial>(ChildNode);
                }
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Target == null)
        {
            return;
        }

        GlobalTransform = GlobalTransform.InterpolateWith(Target.GlobalTransform.Translated(new Vector3(0, 0, ZOffset)), LerpSpeed * delta);
    }
}
