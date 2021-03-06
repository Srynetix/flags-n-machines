using Godot.Collections;

public interface ISynchronizable {
    Dictionary<string, object> _NetworkSend();
    void _NetworkReceive(Dictionary<string, object> data);
}