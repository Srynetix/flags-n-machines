using Godot;

public class ClientLobby : ColorRect
{
    public async override void _Ready()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        GetTree().ChangeScene("res://scenes/game/ClientGame.tscn");
    }
}
