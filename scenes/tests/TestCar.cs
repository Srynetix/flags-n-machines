using Godot;

public class TestCar : Spatial
{
    private Car _Car;
    private Transform _InitialCarTransform;
    private Camera _WorldCamera;
    private ChaseCamera _ChaseCamera;

    public override void _Ready()
    {
        _Car = GetNode<Car>("Car");
        _InitialCarTransform = _Car.Transform;

        _WorldCamera = GetNode<Camera>("WorldCamera");
        _ChaseCamera = GetNode<ChaseCamera>("ChaseCamera");
        _ChaseCamera.Target = _Car.CameraTarget;

        GetNode<LevelLimits>("LevelLimits").Connect(nameof(LevelLimits.OutOfLimits), this, nameof(_ResetCar));
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey) {
            if (eventKey.Pressed) {
                if (eventKey.Scancode == ((uint)KeyList.Enter)) {
                    _ResetCar(_Car);
                }

                else if (eventKey.Scancode == ((uint)KeyList.Tab)) {
                    _WorldCamera.Current = !_WorldCamera.Current;
                }
            }
        }
    }

    private void _OutOfLimits(Node node) {
        if (node is Car car) {
            _ResetCar(car);
        }
    }

    private void _ResetCar(Car car) {
        car.ResetMovement();
        car.Transform = _InitialCarTransform;
    }
}
