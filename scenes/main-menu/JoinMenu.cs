using Godot;

public class JoinMenu : ColorRect
{
    private IconTouchButton _BackButton;

    public override void _Ready()
    {
        _BackButton = GetNode<IconTouchButton>("MainMargin/MainRows/TopColumn/BackButton");
        _BackButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_GoBack));
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
}
