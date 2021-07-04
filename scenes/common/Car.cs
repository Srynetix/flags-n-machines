public class Car : CarEngine {
    public bool IsOwnedByCurrentPeer() {
        return NetworkExt.IsNetworkMaster(_InputController);
    }
}
