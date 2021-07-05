using Godot;

public class ServerLobby : ColorRect
{
    public async override void _Ready()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        GetTree().ChangeScene("res://scenes/game/ServerGame.tscn");
    }
}
