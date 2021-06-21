using Godot;

public class MainMenu : ColorRect
{
    private IconTouchButton _SettingsButton;
    private Button _JoinButton;
    private Button _HostButton;

    public override void _Ready()
    {
        _JoinButton = GetNode<Button>("MainMargin/MainRows/MiddleColumn/JoinButton");
        _HostButton = GetNode<Button>("MainMargin/MainRows/MiddleColumn/HostButton");
        _SettingsButton = GetNode<IconTouchButton>("MainMargin/MainRows/TopColumn/PlayerSection/SettingsButton");
        _SettingsButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_OpenSettingsMenu));
        _JoinButton.Connect("pressed", this, nameof(_OpenJoinMenu));
        _HostButton.Connect("pressed", this, nameof(_OpenHostMenu));

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
}