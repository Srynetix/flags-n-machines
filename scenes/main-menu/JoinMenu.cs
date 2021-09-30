using Godot;
using SxGD;

public class JoinMenu : ColorRect
{
    private IconTouchButton _BackButton;
    private LineEdit _ServerAddress;
    private LineEdit _ServerPort;
    private Button _StartButton;

    public override void _Ready()
    {
        _StartButton = GetNode<Button>("MainMargin/MainRows/BottomColumn/StartButton");
        _StartButton.Connect("pressed", this, nameof(_StartGame));
        _StartButton.Disabled = true;

        _BackButton = GetNode<IconTouchButton>("MainMargin/MainRows/TopColumn/BackButton");
        _BackButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_GoBack));

        _ServerAddress = GetNode<LineEdit>("MainMargin/MainRows/MiddleColumn/Parameters/AddressInput/Value");
        _ServerAddress.Connect("text_changed", this, nameof(_ValidateLineEdit));
        _ServerAddress.Text = CVars.GetInstance().GetVar<string>("join_default_server_address");

        _ServerPort = GetNode<LineEdit>("MainMargin/MainRows/MiddleColumn/Parameters/PortInput/Value");
        _ServerPort.Connect("text_changed", this, nameof(_ValidateLineEdit));
        _ServerPort.Text = CVars.GetInstance().GetVar<int>("host_default_server_port").ToString();

        _UpdateForm();
    }

    public override void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmGoBackRequest)
        {
            _GoBack();
        }
    }

    private void _StartGame()
    {
        // Update parameters
        CVars.GetInstance().SetVar<string>("join_server_address", _ServerAddress.Text);
        CVars.GetInstance().SetVar<int>("join_server_port", int.Parse(_ServerPort.Text));

        GetTree().ChangeScene("res://scenes/main-menu/ClientLobby.tscn");
    }

    private void _ValidateLineEdit(string text)
    {
        _UpdateForm();
    }

    private void _UpdateForm()
    {
        _StartButton.Disabled = !_CheckParametersValidation();
    }

    private bool _CheckParametersValidation()
    {
        var server = _ServerAddress.Text;
        if (server != "")
        {
            var port = _ServerPort.Text;
            var portInt = 0;
            if (int.TryParse(port, out portInt))
            {
                if (portInt > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void _GoBack()
    {
        GetTree().ChangeScene("res://scenes/main-menu/MainMenu.tscn");
    }
}
