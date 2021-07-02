using Godot;

public static class NetworkExtensions {
    public static bool IsNetworkServerChecked(this SceneTree tree) {
        if (tree.NetworkPeer == null) {
            return false;
        } else {
            return tree.IsNetworkServer();
        }
    }
}