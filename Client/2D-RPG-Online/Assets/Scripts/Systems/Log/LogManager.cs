using System;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : MonoBehaviour {

    #region Singleton

    public static LogManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [Header("Settings")]
    [SerializeField]
    private bool _writeLogsToFile = false;

    [SerializeField]
    private bool _isTracing = false;

    private Queue<Log> _allLogs = new Queue<Log>();

    private void Start() {
        Application.logMessageReceived += LogCallback;
    }

    public void AddLog(string message, Log.Type logType, bool appendToLogFile = true) {
        Log log = new Log(
            message,
            DateTime.Now,
            logType);

        _allLogs.Enqueue(log);

        if (appendToLogFile && _writeLogsToFile) {
            WriteToLogFile(log);
        }
    }

    private void LogCallback(string condition, string stackTrace, LogType type) {
        switch (type) {
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
            case LogType.Warning:
                AddLog("LogType: " + type, Log.Type.Error);
                AddLog("Condition: " + condition, Log.Type.Error);
                AddLog("StackTrace: " + stackTrace, Log.Type.Error);
                break;
            case LogType.Log:
                AddLog(condition, Log.Type.Info);
                break;
            default:
                break;
        }
    }

    private void WriteToLogFile(Log log) {
        string logString = string.Empty;

        string logMessage = log.GetMessage();
        string dateTime = log.GetFormattedDateTime();
        string tracingString = log.GetTracingString();

        logString = string.Format("[TIME:{0}][LOG-MESSAGE:\t{1}]",
                                       dateTime,
                                       logMessage);

        if (_isTracing) {
            logString += "\n\t[TRACING:\n\t" + tracingString + "]";
        }

        LogFile.WriteString(logString);
    }

}
