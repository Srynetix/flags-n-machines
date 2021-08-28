using Godot;
using Godot.Collections;

public class SyncInput: Node {
    public Dictionary<int, SyncPeerInput> _PlayerInputs = new Dictionary<int, SyncPeerInput>();
    private Logging _Logger;

    public SyncInput() {
        _Logger = Logging.GetLogger("SyncInput");
        _Logger.SetMaxLogLevel(LogLevel.Error);
        Name = "SyncInput";
    }

    public SyncPeerInput GetCurrentInput() {
        var myId = GetTree().GetNetworkUniqueId();
        if (_PlayerInputs.ContainsKey(myId)) {
            return _PlayerInputs[myId];
        }
        return null;
    }

    public void CreatePeerInput(int peerId) {
        var input = new SyncPeerInput(peerId);
        AddChild(input);

        _PlayerInputs.Add(peerId, input);
        _Logger.DebugM("CreatePeerInput", $"Created peer input for peer {peerId}.");
    }

    public void RemovePeerInput(int peerId) {
        if (_PlayerInputs.ContainsKey(peerId)) {
            _PlayerInputs[peerId].QueueFree();
            _PlayerInputs.Remove(peerId);
        }
    }

    public void UpdatePeerInput(int peerId, Dictionary<string, float> input) {
        _PlayerInputs[peerId].UpdateInput(input);
    }

    public bool IsActionPressed(Node source, string actionName) {
        if (!NetworkExt.IsNetworkEnabled(GetTree())) {
            return Input.IsActionPressed(actionName);
        } else {
            var myId = source.GetNetworkMaster();
            if (myId == 1) {
                // Do not handle server input
                return false;
            }

            if (_PlayerInputs.ContainsKey(myId)) {
                return _PlayerInputs[myId].IsActionPressed(actionName);
            } else {
                return false;
            }
        }
    }

    public float GetActionStrength(Node source, string actionName) {
        if (!NetworkExt.IsNetworkEnabled(GetTree())) {
            return Input.GetActionStrength(actionName);
        } else {
            var myId = source.GetNetworkMaster();
            if (myId == 1) {
                // Do not handle server input
                return 0;
            }

            if (_PlayerInputs.ContainsKey(myId)) {
                return _PlayerInputs[myId].GetActionStrength(actionName);
            } else {
                return 0;
            }
        }
    }
}