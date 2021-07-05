using Godot;
using Godot.Collections;

public class LevelLimits : Area
{
    [Export]
    public Array<NodePath> MonitoredNodes = new Array<NodePath>();

    [Signal]
    public delegate void OutOfLimits(Node node);

    private Array<Spatial> _MonitoredNodeInstances = new Array<Spatial>();
    private Array<Transform> _InitialTransforms = new Array<Transform>();
    private Logging _Logger;

    public LevelLimits() {
        _Logger = Logging.GetLogger("LevelLimits");
    }

    public override void _Ready() {
        Connect("body_exited", this, nameof(_OnBodyExited));

        foreach (var path in MonitoredNodes) {
            var node = GetNode(path);
            if (node is Spatial spatial) {
                _InitialTransforms.Add(spatial.GlobalTransform);
                _MonitoredNodeInstances.Add(spatial);
            } else {
                _Logger.ErrorM("_Ready", $"Unsupported node type {node}.");
            }
        }
    }

    private void _OnBodyExited(Node node) {
        if (node is Spatial spatial) {
            var index = _MonitoredNodeInstances.IndexOf(spatial);
            if (index >= 0) {
                spatial.GlobalTransform = _InitialTransforms[index];
            }
        }

        EmitSignal(nameof(OutOfLimits), node);
    }
}
