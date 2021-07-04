using Godot;

public class MainMenu : ColorRect
{
    private IconTouchButton _SettingsButton;
    private Label _PlayerName;
    private Button _JoinButton;
    private Button _HostButton;
    private Button _TestsButton;

    public override void _Ready()
    {
        PlayerContext.GetInstance().ReadStore();

        _JoinButton = GetNode<Button>("MainMargin/MainRows/MiddleColumn/JoinButton");
        _JoinButton.Connect("pressed", this, nameof(_OpenJoinMenu));

        _HostButton = GetNode<Button>("MainMargin/MainRows/MiddleColumn/HostButton");
        _HostButton.Connect("pressed", this, nameof(_OpenHostMenu));

        _SettingsButton = GetNode<IconTouchButton>("MainMargin/MainRows/TopColumn/PlayerSection/SettingsButton");
        _SettingsButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_OpenSettingsMenu));

        _TestsButton = GetNode<Button>("MainMargin/MainRows/BottomColumn/BottomRow/TestButton");
        _TestsButton.Connect("pressed", this, nameof(_OpenTests));

        _PlayerName = GetNode<Label>("MainMargin/MainRows/TopColumn/PlayerSection/PlayerName");
        _PlayerName.Text = PlayerContext.GetInstance().PlayerName;

        // Enable go back on quit
        GetTree().SetQuitOnGoBack(true);
    }

    private void _OpenSettingsMenu() {
        GetTree().SetQuitOnGoBack(false);
        GetTree().ChangeScene("res://scenes/main-menu/SettingsMenu.tscn");
    }

    private void _OpenJoinMenu() {
        GetTree().SetQuitOnGoBack(false);
        GetTree().ChangeScene("res://scenes/main-menu/JoinMenu.tscn");
    }

    private void _OpenHostMenu() {
        GetTree().SetQuitOnGoBack(false);
        GetTree().ChangeScene("res://scenes/main-menu/HostMenu.tscn");
    }

    private void _OpenTests() {
        GetTree().ChangeScene("res://scenes/tests/TestCar.tscn");
    }
}