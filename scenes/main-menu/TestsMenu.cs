using Godot;
using Godot.Collections;
using SxGD;

public class TestsMenu : ColorRect
{
    private IconTouchButton _BackButton;

    public override void _Ready()
    {
        _BackButton = GetNode<IconTouchButton>("MainMargin/MainRows/TopColumn/BackButton");
        _BackButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_GoBack));

        var testSelection = GetNode("MainMargin/MainRows/MiddleColumn/Parameters/TestSelection/Values");
        testSelection.GetNode<Button>("Car").Connect("pressed", this, nameof(_SelectTest), new Array { "TestCar" });
        testSelection.GetNode<Button>("Networking").Connect("pressed", this, nameof(_SelectTest), new Array { "TestNetworking" });
        testSelection.GetNode<Button>("CSG").Connect("pressed", this, nameof(_SelectTest), new Array { "TestCSG" });
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

    private void _SelectTest(string path)
    {
        GetTree().ChangeScene($"res://scenes/tests/{path}.tscn");
    }
}
