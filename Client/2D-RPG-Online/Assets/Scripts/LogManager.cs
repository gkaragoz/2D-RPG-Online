using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class is responsible to handle any logs.
/// </summary>
/// <remarks>
/// <para>Has Application.logMessageReceived event. That means any Debug.Log function triggers this class.</para>
/// </remarks>
public class LogManager : Menu {

    #region Singleton

    public static LogManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    public GameObject chatContainer;
    public GameObject logTextPrefab;
    public FadeInOut fadeInOut;

    [Header("Settings")]
    public bool isTracing = false;
    public bool limitLogsCount = false;
    public int maxLogsCount = 25;
    public Color infoColor, errorColor, lootColor, interactColor, dropColor, expColor;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private List<Log> _allLogs = new List<Log>();

    private void Start() {
        Application.logMessageReceived += LogCallback;

        AddLog("Press TAB to toggle Log Panel.", Log.Type.Info);
        AddLog("Press 1 to show Info message.", Log.Type.Info);
        AddLog("Press 2 to show Error message.", Log.Type.Info);
        AddLog("Press 3 to show Loot message.", Log.Type.Info);
        AddLog("Press 4 to show Interact message.", Log.Type.Info);
        AddLog("Press 5 to show Drop message.", Log.Type.Info);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            AddLog("Example info log.", Log.Type.Info);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            AddLog("Example error log.", Log.Type.Error);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            AddLog("Example loot log.", Log.Type.Loot);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            AddLog("Example interact log.", Log.Type.Interact);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            AddLog("Example drop log.", Log.Type.Drop);
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Toggle();
        }
    }

    public new void Toggle() {
        isOpen = !isOpen;

        if (isOpen) {
            fadeInOut.ShowImmediately(true);
        } else {
            fadeInOut.HideImmediately();
        }
    }

    public void AddLog(string message, Log.Type logType, bool appendToLogFile = true) {
        Log log = CreateLogObject(message, logType);
        _allLogs.Add(log);

        if (limitLogsCount) {
            CheckLogLimits();
        }
        if (appendToLogFile) {
            WriteToLogFile(log);
        }

        ShowPanel();
    }

    private void CheckLogLimits() {
        if (_allLogs.Count >= maxLogsCount) {
            _allLogs[0].DestroyItself();
            _allLogs.Remove(_allLogs[0]);
        }
    }

    private Log CreateLogObject(string message, Log.Type logType) {
        string colorStringHEX = "#" + ColorUtility.ToHtmlStringRGBA(GetLogColor(logType));

        Log log = Instantiate(logTextPrefab, chatContainer.transform).GetComponent<Log>();
        log.Init(
            message,
            DateTime.Now,
            colorStringHEX,
            logType);

        return log;
    }

    private void ShowPanel() {
        fadeInOut.FadeIn();
    }

    private void HidePanel() {
        fadeInOut.FadeOut();
    }

    private Color GetLogColor(Log.Type logType) {
        Color color = infoColor;

        switch (logType) {
            case Log.Type.Info:
            color = infoColor;
            break;
            case Log.Type.Error:
            color = errorColor;
            break;
            case Log.Type.Loot:
            color = lootColor;
            break;
            case Log.Type.Interact:
            color = interactColor;
            break;
            case Log.Type.Drop:
            color = dropColor;
            break;
        }

        return color;
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
        int sessionID = SessionWatcher.instance.SessionID;
        string tracingString = log.GetTracingString();

        logString = string.Format("[SESSION:{0}][TIME:{1}][LOG-MESSAGE:\t{2}]",
                                       sessionID,
                                       dateTime,
                                       logMessage);

        if (isTracing) {
            logString += "\n\t[TRACING:\n\t" + tracingString + "]";
        }

        LogFile.WriteString(logString);
    }

}
