extends Node
class_name SyncPeerInput

var _input_state := {}
var _actions := [
    "accelerate", "brake", "steer_left", "steer_right", "jump"
]

func _init(peer_id: int = 1) -> void:
    name = "SyncPeerInput#%d" % peer_id
    set_network_master(peer_id)

    for action in _actions:
        _input_state[action] = 0

func get_input_state() -> Dictionary:
    return _input_state

func update_input(input: Dictionary) -> void:
    _input_state = input

func is_action_pressed(action_name: String) -> bool:
    if _input_state.has(action_name):
        return _input_state[action_name] > 0
    else:
        return false

func get_action_strength(action_name: String) -> float:
    if _input_state.has(action_name):
        return _input_state[action_name]
    else:
        return 0.0

func _process(_delta: float) -> void:
    if SxNetwork.is_network_master(self):
        for action in _actions:
            _input_state[action] = Input.get_action_strength(action)