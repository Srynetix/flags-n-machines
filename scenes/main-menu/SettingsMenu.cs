using Godot;
using SxGD;

public class SettingsMenu : ColorRect
{
    [OnReady("MainMargin/MainRows/MiddleColumn/ParameterRows/PlayerName/Input")]
    private LineEdit _PlayerName;
    [OnReady("MainMargin/MainRows/TopColumn/BackButton")]
    private IconTouchButton _BackButton;
    [OnReady("MainMargin/MainRows/BottomColumn/SaveButton")]
    private Button _SaveButton;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _BackButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_GoBack));
        _SaveButton.Connect("pressed", this, nameof(_SaveSettings));

        _PlayerName.Text = PlayerContext.GetInstance().PlayerName;
    }

    public override void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmGoBackRequest)
        {
            _GoBack();
        }
    }

    private void _GoBack()
    {
        GetTree().ChangeScene("res://scenes/main-menu/MainMenu.tscn");
    }

    private void _SaveSettings()
    {
        PlayerContext.GetInstance().PlayerName = _PlayerName.Text;
        PlayerContext.GetInstance().SaveStore();
    }
}
