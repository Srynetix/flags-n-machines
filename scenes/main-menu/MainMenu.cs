using Godot;
using SxGD;

public class MainMenu : ColorRect
{
    [OnReady("MainMargin/MainRows/TopColumn/PlayerSection/SettingsButton")]
    private readonly IconTouchButton _SettingsButton;
    [OnReady("MainMargin/MainRows/TopColumn/PlayerSection/PlayerName")]
    private readonly Label _PlayerName;
    [OnReady("MainMargin/MainRows/MiddleColumn/JoinButton")]
    private readonly Button _JoinButton;
    [OnReady("MainMargin/MainRows/MiddleColumn/HostButton")]
    private readonly Button _HostButton;
    [OnReady("MainMargin/MainRows/BottomColumn/BottomRow/TestButton")]
    private readonly Button _TestsButton;

    public override void _Ready()
    {
        PlayerContext.GetInstance().ReadStore();
        NodeExt.BindNodes(this);

        _JoinButton.Connect("pressed", this, nameof(_OpenJoinMenu));
        _HostButton.Connect("pressed", this, nameof(_OpenHostMenu));
        _TestsButton.Connect("pressed", this, nameof(_OpenTests));
        _SettingsButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_OpenSettingsMenu));

        _PlayerName.Text = PlayerContext.GetInstance().PlayerName;

        // Enable go back on quit
        GetTree().SetQuitOnGoBack(true);
    }

    private void _OpenSettingsMenu()
    {
        GetTree().SetQuitOnGoBack(false);
        GetTree().ChangeScene("res://scenes/main-menu/SettingsMenu.tscn");
    }

    private void _OpenJoinMenu()
    {
        GetTree().SetQuitOnGoBack(false);
        GetTree().ChangeScene("res://scenes/main-menu/JoinMenu.tscn");
    }

    private void _OpenHostMenu()
    {
        GetTree().SetQuitOnGoBack(false);
        GetTree().ChangeScene("res://scenes/main-menu/HostMenu.tscn");
    }

    private void _OpenTests()
    {
        GetTree().ChangeScene("res://scenes/main-menu/TestsMenu.tscn");
    }
}