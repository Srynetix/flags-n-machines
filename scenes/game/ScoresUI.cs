using Godot;
using System.Collections.Generic;

public class ScoresUI : MarginContainer {
    private VBoxContainer _Rows;
    private Dictionary<int, HBoxContainer> _PlayerLines = new Dictionary<int, HBoxContainer>();

    public override void _Ready() {
        _Rows = GetNode<VBoxContainer>("Panel/InnerMargin/Row/Rows");
    }

    public void UpdateScores(Dictionary<int, PlayerData> data) {
        foreach (var kv in data) {
            _UpdatePlayerScore(kv.Key, kv.Value);
        }
    }

    private void _UpdatePlayerScore(int peerId, PlayerData data) {
        if (_PlayerLines.ContainsKey(peerId)) {
            _UpdatePlayerLine(_PlayerLines[peerId], data);
        } else {
            var line = new HBoxContainer();
            var name = new Label() {
                SizeFlagsHorizontal = (int)SizeFlags.ExpandFill,
                SizeFlagsVertical = (int)SizeFlags.ShrinkCenter,
                Text = data.Name
            };
            var score = new Label() {
                SizeFlagsHorizontal = (int)SizeFlags.Expand | (int)SizeFlags.ShrinkEnd,
                SizeFlagsVertical = (int)SizeFlags.ShrinkCenter,
                Text = data.Score.ToString()
            };
            line.AddChild(name);
            line.AddChild(score);
            _Rows.AddChild(line);

            _PlayerLines[peerId] = line;
        }
    }

    private void _UpdatePlayerLine(HBoxContainer line, PlayerData data) {
        var nameLabel = line.GetChild<Label>(0);
        var scoreLabel = line.GetChild<Label>(1);
        nameLabel.Text = data.Name;
        scoreLabel.Text = data.Score.ToString();
    }
}
