using Godot;

public class DebugMenu: MarginContainer {
    private LogPanel _LogPanel;
    private NodeTracerSystem _NodeTracerSystem;
    private Logging _Logger;

    public DebugMenu() {
        _Logger = Logging.GetLogger("DebugMenu");
    }

    public override void _Ready()
    {
        _LogPanel = (LogPanel)GetNode("/root/LogPanel");
        _NodeTracerSystem = (NodeTracerSystem)GetNode("/root/NodeTracerSystem");

        // Hide all
        _LogPanel.Visible = false;
        _NodeTracerSystem.Visible = false;

        for (var i = 0; i < 50; ++i) {
            _Logger.DebugM("_Ready", "DebugMenu is ready.");
        }
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey eventKey) {
            if (eventKey.Pressed) {
                if (eventKey.Scancode == (int)KeyList.F2) {
                    _ToggleNodeTracer();
                }

                else if (eventKey.Scancode == (int)KeyList.F3) {
                    _ToggleLogPanel();
                }
            }
        }
    }

    private void _ToggleNodeTracer() {
        _LogPanel.Visible = false;
        _NodeTracerSystem.Visible = !_NodeTracerSystem.Visible;
        _Logger.DebugM("_ToggleNodeTracer", $"Node tracer visibility: {_NodeTracerSystem.Visible}");
    }

    private void _ToggleLogPanel() {
        _NodeTracerSystem.Visible = false;
        _LogPanel.Visible = !_LogPanel.Visible;
        _Logger.DebugM("_ToggleLogPanel", $"Log panel visibility: {_LogPanel.Visible}");
    }
}