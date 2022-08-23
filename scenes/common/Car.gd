extends CarEngine
class_name Car

func is_owned_by_current_peer() -> bool:
    return SxNetwork.is_network_master(_input_controller)