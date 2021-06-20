using Godot;

public class MainMenu : ColorRect
{
    private IconTouchButton _SettingsButton;

    public override void _Ready()
    {
        _SettingsButton = GetNode<IconTouchButton>("MainMargin/MainRows/TopColumn/PlayerSection/SettingsButton");
        _SettingsButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_OpenSettingsMenu));

        // Enable go back on quit
        GetTree().SetQuitOnGoBack(true);
    }

    private void _OpenSettingsMenu() {
        GetTree().SetQuitOnGoBack(false);
        GetTree().ChangeScene("res://scenes/main-menu/SettingsMenu.tscn");
    }
}