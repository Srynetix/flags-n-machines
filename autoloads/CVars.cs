public class CVars {
    public class UnknownCVarException: System.Exception {
        public UnknownCVarException(string name): base($"Unknown CVar: {name}") {}
    }

    private string CVARS_CONTEXT_FILE = "user://cvars.json";

    private KVStore _Store;
    private static CVars _GlobalInstance;
    private Logging _Logger;

    public CVars() {
        _Logger = Logging.GetLogger("CVars");
        _Store = KVStore.GetInstance();
        _InitializeVars();
    }

    public static CVars GetInstance() {
        if (_GlobalInstance == null) {
            _GlobalInstance = new CVars();
        }

        return _GlobalInstance;
    }

    public T GetVar<T>(string name) {
        var output = _Store.LoadObject(name, null);
        if (output == null) {
            _Logger.ErrorM("GetVar", $"Unknown CVar named '{name}'.");
            throw new UnknownCVarException(name);
        }

        return (T)output;
    }

    public void SetVar<T>(string name, T value) {
        _Store.Store(name, value);
    }

    private void _InitializeVars() {
        _RegisterVar("host_default_max_players", 8);
        _RegisterVar("host_default_server_port", 13795);
        _RegisterVar("join_default_server_address", "127.0.0.1");
        _RegisterVar("host_max_players", 8);
        _RegisterVar("host_server_port", 13795);
        _RegisterVar("join_server_address", "127.0.0.1");
        _RegisterVar("join_server_port", 13795);
    }

    private void _RegisterVar(string name, object value) {
        _Logger.DebugM("RegisterVar", $"Registering CVar '{name}' with value '{value}'.");
        _Store.Store(name, value);
    }
}