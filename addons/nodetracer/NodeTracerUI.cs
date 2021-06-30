using Godot;
using Godot.Collections;

public class NodeTracerUI : ColorRect
{
    private Label _NodeTitle;
    private VBoxContainer _Parameters;
    private Dictionary<string, NodeTracerUIParam> _ParametersLabels = new Dictionary<string, NodeTracerUIParam>();

    public static NodeTracerUI CreateInstance() {
        var scene = (PackedScene)GD.Load("res://addons/nodetracer/NodeTracerUI.tscn");
        return scene.Instance<NodeTracerUI>();
    }

    public override void _Ready()
    {
        _NodeTitle = GetNode<Label>("Margin/MainContainer/NodeTitle");
        _Parameters = GetNode<VBoxContainer>("Margin/MainContainer/ParametersSection/Parameters");

        // Remove samples
        foreach (Node node in _Parameters.GetChildren()) {
            node.QueueFree();
        }
    }

    public void UpdateUsingTracer(NodeTracer tracer) {
        _NodeTitle.Text = tracer.Title;
        foreach (var kv in tracer.Parameters) {
            _TraceParameter(kv.Key, kv.Value);
        }
    }

    private void _CreateParameter(string name, string value) {
        var label = NodeTracerUIParam.CreateInstance();
        _ParametersLabels[name] = label;
        _Parameters.AddChild(label);

        label.UpdateValue(name, value);
    }

    private void _TraceParameter(string name, string value) {
        if (_ParametersLabels.ContainsKey(name)) {
            _ParametersLabels[name].UpdateValue(name, value);
        } else {
            _CreateParameter(name, value);
        }
    }
}
