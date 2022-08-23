extends Camera
class_name ChaseCamera

export var lerp_speed := 20.0
export var target_path: NodePath
export var child_node: String
export var z_offset := 5.0

var target: Spatial

func _ready() -> void:
    if target == null:
        if target_path != "":
            target = get_node(target_path) as Spatial
            if child_node != "":
                target = target.get_node(child_node) as Spatial

func _physics_process(delta: float) -> void:
    if target == null:
        return

    global_transform = global_transform.interpolate_with(
        target.global_transform.translated(
            Vector3(0, 0, z_offset)
        ),
        lerp_speed * delta
    )
