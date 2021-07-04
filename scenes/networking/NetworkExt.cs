using Godot;

public static class NetworkExt {
    public static bool IsNetworkServerChecked(this SceneTree tree) {
        if (tree.NetworkPeer == null) {
            return true;
        } else {
            return tree.IsNetworkServer();
        }
    }

    public static bool IsNetworkServer(SceneTree tree) {
        return tree.IsNetworkServerChecked();
    }

    public static bool IsNetworkMasterChecked(this Node node) {
        if (node.GetTree().NetworkPeer == null) {
            return true;
        } else {
            return node.IsNetworkMaster();
        }
    }

    public static bool IsNetworkMaster(Node node) {
        return node.IsNetworkMasterChecked();
    }

    public static bool IsNetworkEnabled(SceneTree tree) {
        return tree.NetworkPeer != null;
    }
}