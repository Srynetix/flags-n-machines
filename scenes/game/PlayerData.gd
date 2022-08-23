extends Object
class_name PlayerData

var name: String
var score: int

func deserialize(data: Dictionary):
    name = data["name"]
    score = data["score"]

func serialize() -> Dictionary:
    return {
        "name": name,
        "score": score
    }