extends Node
class_name GameMasterClient

onready var _game_node := get_parent().get_node("Stage") as GameStage
var _logger := SxLog.get_logger("GameMasterClient")
var _car: Car

func _ready() -> void:
    MainRPCService.client.connect("spawned_from_server", self, "_node_spawned")

func _node_spawned(node: Node) -> void:
    _logger.info_mn(SxNetwork.get_nuid(self), "_node_spawned", str(node))

    if node is Car:
        var car := node as Car
        if car.is_owned_by_current_peer():
            car.get_input_controller().show_virtual_controls()
            _car = car
            _game_node.chase_camera.target = car
            _game_node.chase_camera.current = true

remote func receive_player_scores(data: String):
    var result := JSON.parse(data)
    if result.error == OK:
        var scores = {}
        for key in result.result:
            var pdata = PlayerData.new()
            pdata.deserialize(result.result[key])
            scores[int(key)] = pdata
        _game_node.scores_ui.update_scores(scores)
