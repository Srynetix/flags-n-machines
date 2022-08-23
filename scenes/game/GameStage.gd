extends Spatial
class_name GameStage

onready var chase_camera := $ChaseCamera as ChaseCamera
onready var spectator_camera := $SpectatorCamera as FPSCamera
onready var level_limits := $LevelLimits as LevelLimits
onready var scores_ui := $ScoresUI as ScoresUI