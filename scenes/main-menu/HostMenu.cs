using Godot;
using Godot.Collections;

public class HostMenu : ColorRect
{
    private IconTouchButton _BackButton;
    private Array _MapButtons = new Array();

    private LineEdit _MaxPlayers;
    private LineEdit _ServerPort;
    private Button _StartButton;
    private string _SelectedMap = "";

    public override void _Ready()
    {
        _StartButton = GetNode<Button>("MainMargin/MainRows/BottomColumn/StartButton");
        _StartButton.Disabled = true;

        _MaxPlayers = GetNode<LineEdit>("MainMargin/MainRows/MiddleColumn/Parameters/MaxPlayersInput/Value");
        _MaxPlayers.Connect("text_changed", this, nameof(_ValidateLineEdit));
        _MaxPlayers.Text = CVars.GetInstance().GetVar<int>("host_default_max_players").ToString();

        _ServerPort = GetNode<LineEdit>("MainMargin/MainRows/MiddleColumn/Parameters/PortInput/Value");
        _ServerPort.Connect("text_changed", this, nameof(_ValidateLineEdit));
        _ServerPort.Text = CVars.GetInstance().GetVar<int>("host_default_server_port").ToString();

        _BackButton = GetNode<IconTouchButton>("MainMargin/MainRows/TopColumn/BackButton");
        _BackButton.Connect(nameof(IconTouchButton.pressed), this, nameof(_GoBack));

        var mapsContainer = GetNode<HBoxContainer>("MainMargin/MainRows/MiddleColumn/Parameters/MapSelection/Values");
        _MapButtons = mapsContainer.GetChildren();

        foreach (Button button in _MapButtons) {
            if (!button.Disabled) {
                button.Connect("toggled", this, nameof(_SelectMap), new Array { button });
            }
        }

        _UpdateForm();
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

    private void _SelectMap(bool toggle_status, Button btn) {
        if (toggle_status) {
            // Reset all other buttons
            foreach (Button otherBtn in _MapButtons) {
                if (otherBtn != btn) {
                    otherBtn.Pressed = false;
                }
            }

            _SelectedMap = btn.Text;
        } else if (_SelectedMap == btn.Text) {
            _SelectedMap = "";
        }

        _UpdateForm();
    }

    private void _ValidateLineEdit(string value) {
        _UpdateForm();
    }

    private bool _CheckParametersValidation() {
        if (_SelectedMap != "") {
            var playersStr = _MaxPlayers.Text;
            var playersInt = 0;
            if (int.TryParse(playersStr, out playersInt)) {
                if (playersInt > 0) {
                    var portStr = _ServerPort.Text;
                    var portInt = 0;
                    if (int.TryParse(portStr, out portInt)) {
                        if (portInt > 0) {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private void _UpdateForm() {
        _StartButton.Disabled = !_CheckParametersValidation();
    }
}
