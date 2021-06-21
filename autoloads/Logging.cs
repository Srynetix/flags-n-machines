using Godot;
using Godot.Collections;
using System.Linq;

enum LogLevel {
    Trace,
    Debug,
    Info,
    Warning,
    Error,
    Critical
}

public class Logging: Node {
    private static Dictionary<string, Logging> _Loggers = new Dictionary<string, Logging>();

    public Logging(): this("root") {}

    public Logging(string name) {
        Name = name;
    }

    public static Logging GetLogger(string name) {
        if (_Loggers.ContainsKey(name)) {
            return _Loggers[name];
        } else {
            var logger = new Logging(name);
            _Loggers[name] = logger;
            return logger;
        }
    }

    private string _FormatArgs(params object[] values) {
        return string.Join(" ", values.Select(p => p.ToString()).ToArray());
    }

    private string _FormatLog(LogLevel level, params object[] values) {
        var log_level = level.ToString().ToUpper();
        return $"[{log_level}] [{Name}] {_FormatArgs(values)}";
    }

    private string _FormatLogMethod(LogLevel level, string method, params object[] values) {
        var log_level = level.ToString().ToUpper();
        return $"[{log_level}] [{Name}::{method}] {_FormatArgs(values)}";
    }

    private void _Log(LogLevel level, params object[] values) {
        var logLine = _FormatLog(level, values);
        if (level < LogLevel.Warning) {
            GD.Print(logLine);
        } else {
            GD.PrintErr(logLine);
        }
    }

    private void _LogMethod(LogLevel level, string method, params object[] values) {
        var logLine = _FormatLogMethod(level, method, values);
        if (level < LogLevel.Warning) {
            GD.Print(logLine);
        } else {
            GD.PrintErr(logLine);
        }
    }

    public void Trace(params object[] values) {
        _Log(LogLevel.Trace, values);
    }

    public void Debug(params object[] values) {
        _Log(LogLevel.Debug, values);
    }

    public void Info(params object[] values) {
        _Log(LogLevel.Info, values);
    }

    public void Warn(params object[] values) {
        _Log(LogLevel.Warning, values);
    }

    public void Error(params object[] values) {
        _Log(LogLevel.Error, values);
    }

    public void Critical(params object[] values) {
        _Log(LogLevel.Critical, values);
    }

    public void TraceM(string method, params object[] values) {
        _LogMethod(LogLevel.Trace, method, values);
    }

    public void DebugM(string method, params object[] values) {
        _LogMethod(LogLevel.Debug, method, values);
    }

    public void InfoM(string method, params object[] values) {
        _LogMethod(LogLevel.Info, method, values);
    }

    public void WarnM(string method, params object[] values) {
        _LogMethod(LogLevel.Warning, method, values);
    }

    public void ErrorM(string method, params object[] values) {
        _LogMethod(LogLevel.Error, method, values);
    }

    public void CriticalM(string method, params object[] values) {
        _LogMethod(LogLevel.Critical, method, values);
    }
}