using Godot;
using System.Collections.Generic;

public class SynchronizedCar : Spatial, ISynchronizable
{
    private NodeTracer _Tracer;

    public override void _Ready()
    {
        _Tracer = new NodeTracer();
        AddChild(_Tracer);
    }

    public Dictionary<string, object> _NetworkSend() {
        return new Dictionary<string, object>() {
            { "transform", Transform },
        };
    }

    public void _NetworkReceive(Dictionary<string, object> data) {
        if (data.ContainsKey("transform")) {
            var transform = data["transform"];
            if (transform is Transform t) {
                Transform = t;
            }
        }
    }

    public override void _Process(float delta)
    {
        if (GetTree().IsNetworkServerChecked()) {
            RotateX(delta);
            RotateY(delta);
            RotateZ(delta);
        }

        _Tracer.TraceParameter("Position", Transform.origin);
        _Tracer.TraceParameter("Rot X", Transform.basis.x);
        _Tracer.TraceParameter("Rot Y", Transform.basis.y);
        _Tracer.TraceParameter("Rot Z", Transform.basis.z);
    }
}
