extends MarginContainer
class_name ScoresUI

onready var _rows := $Panel/InnerMargin/Row/Rows as VBoxContainer
var _player_lines := {}

func update_scores(data: Dictionary) -> void:
    for key in data:
        _update_player_score(key, data[key])

func _update_player_score(peer_id: int, data: PlayerData):
    if _player_lines.has(peer_id):
        _update_player_line(_player_lines[peer_id], data)
    else:
        var line := HBoxContainer.new()
        var name := Label.new()
        name.size_flags_horizontal = SIZE_EXPAND_FILL
        name.size_flags_vertical = SIZE_SHRINK_CENTER
        name.text = data.name

        var score := Label.new()
        score.size_flags_horizontal = SIZE_EXPAND_FILL
        score.size_flags_vertical = SIZE_SHRINK_CENTER
        score.text = str(data.score)

        line.add_child(name)
        line.add_child(score)
        _rows.add_child(line)
        _player_lines[peer_id] = line

func _update_player_line(line: HBoxContainer, data: PlayerData) -> void:
    var name_label := line.get_child(0) as Label
    var score_label := line.get_child(1) as Label
    name_label.text = data.name
    score_label.text = str(data.score)
