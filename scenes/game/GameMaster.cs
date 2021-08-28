using Godot;

public class GameMaster : Node
{
    private ServerPeer _Server;
    private Logging _Logger;

    private Car _ClientCar;

    public GameMaster() {
        _Logger = Logging.GetLogger("GameMaster");
    }
}
