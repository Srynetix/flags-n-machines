extends Spatial
class_name SynchronizedCar

var _tracer: SxNodeTracer

func _ready() -> void:
    _tracer = SxNodeTracer.new()
    add_child(_tracer)

func _network_send() -> Dictionary:
    return {
        "transform": transform
    }

func _network_receive(data: Dictionary) -> void:
    if data.has("transform"):
        var item = data["transform"]
        if item is Transform:
            var t := item as Transform
            transform = t

func _process(delta: float) -> void:
    if SxNetwork.is_network_server(get_tree()):
        rotate_x(delta)
        rotate_y(delta)
        rotate_z(delta)

    _tracer.trace_parameter("Position", transform.origin)
    _tracer.trace_parameter("Rot X", transform.basis.x)
    _tracer.trace_parameter("Rot Y", transform.basis.y)
    _tracer.trace_parameter("Rot Z", transform.basis.z)