using Godot;
using SxGD;

public class ServerGame : Spatial
{
    private ListenServerPeer _ListenServer;
    private ServerPeer _Server;
    private ClientPeer _Client;
    private Logging _Logger;

    public ServerGame()
    {
        Name = "Game";
        _Logger = Logging.GetLogger("ServerGame");
        Logging.ShowInConsole = false;
        Logging.ConfigureLogLevels("info");
    }

    public override void _Ready()
    {
        _ListenServer = new ListenServerPeer()
        {
            ServerPort = CVars.GetInstance().GetVar<int>("host_server_port"),
            MaxPlayers = CVars.GetInstance().GetVar<int>("host_max_players")
        };
        AddChild(_ListenServer);
        _Server = _ListenServer.GetServer();

        var listenRegistry = new NodeRegistry();
        listenRegistry.RegisterNode("Server", _Server);

        _ListenServer.GetServerRoot().AddChild(listenRegistry);
        _ListenServer.GetServerRoot().AddChild(new Spatial()
        {
            Name = "Game"
        });
        _Server.SpawnSynchronizedSceneMapped(
            parent: "/root/Game",
            name: "GameMaster",
            serverScenePath: "res://scenes/game/GameMasterServer.tscn",
            clientScenePath: "res://scenes/game/GameMasterClient.tscn"
        );

        _Client = new ClientPeer()
        {
            ServerAddress = "127.0.0.1",
            ServerPort = CVars.GetInstance().GetVar<int>("host_server_port")
        };
        AddChild(_Client);
    }
}
