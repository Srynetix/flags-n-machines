using Godot;

public class SettingsMenu : ColorRect
{
    private LineEdit _PlayerName;
    private IconTouchButton _BackButton;
    private Button _SaveButton;

    public override void _Ready()
    {
        _BackButton = GetNode<IconTouchButton>("MainMargin/MainRows/TopColumn/BackButton");
        _BackButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_GoBack));

        _PlayerName = GetNode<LineEdit>("MainMargin/MainRows/MiddleColumn/ParameterRows/PlayerName/Input");
        _PlayerName.Text = PlayerContext.GetInstance().PlayerName;

        _SaveButton = GetNode<Button>("MainMargin/MainRows/BottomColumn/SaveButton");
        _SaveButton.Connect("pressed", this, nameof(_SaveSettings));
    }

    public override void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmGoBackRequest) {
            _GoBack();
        }
    }

    private void _GoBack() {
        GetTree().ChangeScene("res://scenes/main-menu/MainMenu.tscn");
    }

    private void _SaveSettings() {
        PlayerContext.GetInstance().PlayerName = _PlayerName.Text;
        PlayerContext.GetInstance().SaveStore();
    }
}
