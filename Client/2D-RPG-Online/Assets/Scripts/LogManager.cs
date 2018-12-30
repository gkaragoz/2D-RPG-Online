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

    [Header("Initializers")]
    public GameObject chatContainer;
    public GameObject logTextPrefab;
    public int maxLogsCount = 25;
    public Color infoColor, errorColor, lootColor, interactColor, dropColor, expColor;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private List<Log> _allLogs = new List<Log>();

    private void Start() {
        Application.logMessageReceived += LogCallback;

        AddLog("Press 1 to send test Info message.", Log.Type.Info);
        AddLog("Press 2 to send test Error message.", Log.Type.Info);
    }

    public override void Toggle() {
        base.Toggle();
    }

    public void AddLog(string text, Log.Type logType) {
        if (_allLogs.Count >= maxLogsCount) {
            Destroy(_allLogs[0].UI.gameObject);
            _allLogs.Remove(_allLogs[0]);
        }

        Log log = new Log();
        log.message = text;

        string colorStringHEX = "#" + ColorUtility.ToHtmlStringRGBA(GetLogColor(logType));

        GameObject textUIObject = Instantiate(logTextPrefab, chatContainer.transform);
        log.UI = textUIObject.GetComponent<TextMeshProUGUI>();
        log.dateTime = DateTime.Now;
        log.UI.text = string.Format("[{0}] <color={1}>{2}</color>", log.dateTime.ToLongTimeString(), colorStringHEX, log.message);
        _allLogs.Add(log);
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

    void LogCallback(string condition, string stackTrace, LogType type) {
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

}
