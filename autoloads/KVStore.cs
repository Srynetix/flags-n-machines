using Godot;
using Godot.Collections;

public class KVStore {
    private Dictionary<string, object> _Data = new Dictionary<string, object>();
    private static KVStore _GlobalInstance;
    private Logging _Logger;

    public KVStore() {
        _Logger = Logging.GetLogger("KVStore");
        _Logger.SetMaxLogLevel(LogLevel.Error);
    }

    public static KVStore GetInstance() {
        if (_GlobalInstance == null) {
            _GlobalInstance = new KVStore();
        }

        return _GlobalInstance;
    }

    public void Store(string key, object value) {
        _Logger.DebugM("Store", $"Storing '{key}' with value '{value}'");
        _Data[key] = value;
    }

    public object LoadObject(string key, object defaultValue = null) {
        if (_Data.ContainsKey(key)) {
            var value = _Data[key];
            _Logger.DebugM("LoadObject", $"Found '{key}' with value '{value}'.");
            return value;
        } else {
            _Logger.DebugM("LoadObject", $"Missing key '{key}', using default value '{defaultValue}'.");
            return defaultValue;
        }
    }

    public T Load<T>(string key, T defaultValue) {
        return (T)LoadObject(key, defaultValue);
    }

    public bool Exists(string key) {
        return _Data.ContainsKey(key);
    }

    public void ReadStoreFromPath(string path) {
        var file = new File();
        if (!file.FileExists(path)) {
            _Logger.WarnM("ReadStoreFromPath", $"Unknown file '{path}'. Skipping.");
            return;
        }
        _Logger.DebugM("ReadStoreFromPath", $"Reading file '{path}'.");

        file.Open(path, File.ModeFlags.Read);
        var line = file.GetLine();
        var parse_result = JSON.Parse(line);
        if (parse_result.Error != Error.Ok) {
            _Logger.ErrorM("ReadStoreFromPath", $"Bad JSON value: '{line}'");
        } else {
            var data = (Dictionary)parse_result.Result;
            foreach (var key in data.Keys) {
                Store((string)key, data[key]);
            }
        }

        file.Close();
    }

    public void SaveStoreToPath(string path) {
        var file = new File();
        var status = file.Open(path, File.ModeFlags.Write);
        if (status != Error.Ok) {
            _Logger.ErrorM("SaveStoreToPath", $"Error while opening file '{path}': {status}");
            return;
        }

        _Logger.InfoM("SaveStoreToPath", $"Writing to file '{path}'.");
        file.StoreLine(JSON.Print(_Data));
        file.Close();
    }
}