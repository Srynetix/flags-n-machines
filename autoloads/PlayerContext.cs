using Godot;

public class PlayerContext {
    private string PLAYER_CONTEXT_FILE = "user://player.json";

    public string PlayerName {
        get => _GetStoreValue<string>("player_name", "Dummy");
        set => _SetStoreValue("player_name", value);
    }

    private static PlayerContext _GlobalInstance;

    public static PlayerContext GetInstance() {
        if (_GlobalInstance == null) {
            _GlobalInstance = new PlayerContext();
        }

        return _GlobalInstance;
    }

    private T _GetStoreValue<T>(string key, T defaultValue) {
        return KVStore.GetInstance().Load<T>(key, defaultValue);
    }

    private void _SetStoreValue(string key, object value) {
        KVStore.GetInstance().Store(key, value);
    }

    public void ReadStore() {
        KVStore.GetInstance().ReadStoreFromPath(PLAYER_CONTEXT_FILE);
    }

    public void SaveStore() {
        KVStore.GetInstance().SaveStoreToPath(PLAYER_CONTEXT_FILE);
    }
}