using Godot;
using Godot.Collections;

public class NodeRegistry : Node
{
    private Dictionary<string, Node> _Registry = new Dictionary<string, Node>();
    private Logging _Logger;

    public NodeRegistry() {
        Name = "NodeRegistry";
        _Logger = Logging.GetLogger("NodeRegistry");
    }

    public static NodeRegistry GetInstance(SceneTree tree) {
        return tree.Root.GetNode<NodeRegistry>("NodeRegistry");
    }

    public void ShowRegistry() {
        if (_Registry.Count == 0) {
            GD.Print("Empty registry.");
        } else {
            foreach (var kv in _Registry) {
                GD.Print($"- {kv.Key}: {kv.Value}");
            }
        }
    }

    public void RegisterPath(string name, NodePath path) {
        RegisterNode(name, GetNode(path));
    }

    public void RegisterNode(string name, Node node) {
        if (_Registry.ContainsKey(name)) {
            _Logger.WarnM("RegisterNode", $"Node '{name}' already registered. Overwriting ...");
        } else {
            _Logger.DebugM("RegisterNode", $"Registering node '{name}' in registry of type {node}.");
        }

        _Registry.Add(name, node);
    }

    public T GetNodeFromRegistry<T>(string name) where T: Node {
        if (!_Registry.ContainsKey(name)) {
            _Logger.ErrorM("GetNodeFromRegistry", $"Unknown node '{name}'.");
            return null;
        } else {
            return (T)_Registry[name];
        }
    }
}
