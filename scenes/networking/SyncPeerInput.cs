using Godot;
using System.Collections.Generic;

public class SyncPeerInput: Node {
    private Dictionary<string, float> _InputState = new Dictionary<string, float>();
    private string[] _Actions = new string[] {
        "accelerate", "brake", "steer_left", "steer_right", "jump"
    };

    public SyncPeerInput(int peerId = 1) {
        Name = $"SyncPeerInput#{peerId}";
        SetNetworkMaster(peerId);

        foreach (var action in _Actions) {
            _InputState[action] = 0;
        }
    }

    public Dictionary<string, float> GetInputState() {
        return _InputState;
    }

    public void UpdateInput(Dictionary<string, float> input) {
        _InputState = input;
    }

    public bool IsActionPressed(string actionName) {
        if (_InputState.ContainsKey(actionName)) {
            return _InputState[actionName] > 0;
        } else {
            return false;
        }
    }

    public float GetActionStrength(string actionName) {
        if (_InputState.ContainsKey(actionName)) {
            return _InputState[actionName];
        } else {
            return 0;
        }
    }

    public override void _Process(float delta) {
        if (NetworkExt.IsNetworkMaster(this)) {
            foreach (var action in _Actions) {
                _InputState[action] = Input.GetActionStrength(action);
            }
        }
    }
}