using Godot;
using Godot.Collections;
using System.Text;

public class LogPanel: Control {
    private RichTextLabel _Content;
    private Array<LogMessage> _Messages;

    public override void _Ready()
    {
        _Content = GetNode<RichTextLabel>("Container/ColorRect/Margin/Content");
    }

    public override void _Process(float delta)
    {
        _Content.BbcodeText = _GenerateContent(Logging.Messages);
    }

    public string _GenerateContent(Array<LogMessage> messages) {
        var sb = new StringBuilder();
        foreach (LogMessage message in messages) {
            sb.AppendLine($"[b][color=yellow][{message.Level.ToString().ToUpper()}][/color][/b] [b][color=green][{message.LoggerName}][/color][b] {message.Message}");
        }
        return sb.ToString();
    }
}