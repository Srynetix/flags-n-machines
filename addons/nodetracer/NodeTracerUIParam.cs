using Godot;

public class NodeTracerUIParam : ColorRect
{
    private RichTextLabel _Label;

    public static NodeTracerUIParam CreateInstance() {
        var scene = (PackedScene)GD.Load("res://addons/nodetracer/NodeTracerUIParam.tscn");
        return scene.Instance<NodeTracerUIParam>();
    }

    public override void _Ready()
    {
        _Label = GetNode<RichTextLabel>("MarginContainer/Parameter");
    }

    public void UpdateValue(string name, string value) {
        _Label.BbcodeText = $"[b]{name}[/b]: {value}";
    }
}
