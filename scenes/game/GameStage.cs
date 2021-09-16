using Godot;

public class GameStage : Spatial
{
    public ChaseCamera ChaseCamera;
    public FPSCamera SpectatorCamera;
    public LevelLimits LevelLimits;
    public ScoresUI ScoresUI;

    public override void _Ready()
    {
        ChaseCamera = GetNode<ChaseCamera>("ChaseCamera");
        SpectatorCamera = GetNode<FPSCamera>("SpectatorCamera");
        LevelLimits = GetNode<LevelLimits>("LevelLimits");
        ScoresUI = GetNode<ScoresUI>("ScoresUI");
    }
}
