extends Node
class_name CarInputController

var steer_left: float
var steer_right: float
var accelerate: bool
var brake: bool
var jump: bool

onready var _virtual_controls = $VirtualControls as SxVirtualControls

func _ready() -> void:
    if SxNetwork.is_network_master(self):
        _virtual_controls.visible = true

func show_virtual_controls() -> void:
    _virtual_controls.visible = true

func _physics_process(_delta: float) -> void:
    var sync_input := MainRPCService.sync_input as SxSyncInput
    if SxNetwork.is_network_server(get_tree()):
        steer_left = sync_input.get_action_strength(self, "steer_left")
        steer_right = sync_input.get_action_strength(self, "steer_right")
        accelerate = sync_input.is_action_pressed(self, "accelerate")
        brake = sync_input.is_action_pressed(self, "brake")
        jump = sync_input.is_action_pressed(self, "jump")
