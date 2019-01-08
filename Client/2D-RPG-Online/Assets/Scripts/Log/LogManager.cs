using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class LogManager : Menu {

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

    public delegate void IsLimitedLogCountChanged(bool status);
    public event IsLimitedLogCountChanged onIsLimitedLogCountChanged;

    public delegate void MaxLogsCountChanged(int count);
    public event MaxLogsCountChanged onMaxLogsCountChanged;

    [System.Serializable]
    public class UISettings {
        public Slider sliderMaxLogsCount;
        public TextMeshProUGUI txtLimitLogsCount;
        public TextMeshProUGUI txtMaxLogsCountHandle;
        public Toggle toggleIsLogCountLimited;

        public void Initialize(int count, bool status) {
            SetSliderMaxLogsCount(count);
            SetToggleMaxLogsCount(status);
        }

        public void SetSliderMaxLogsCount(int count) {
            sliderMaxLogsCount.value = count;

            txtMaxLogsCountHandle.text = count.ToString();
            txtLimitLogsCount.text = "Limit Logs (" + count + ")";
        }

        public void SetToggleMaxLogsCount(bool status) {
            toggleIsLogCountLimited.isOn = status;
        }
    }

    public GameObject chatContainer;
    public GameObject logTextPrefab;
    public FadeInOut fadeInOut;

    [Header("UI Initialization")]
    [SerializeField]
    private UISettings _UISettings;

    [Header("Settings")]
    [SerializeField]
    private bool _writeLogsToFile = false;

    [SerializeField]
    private bool _isTracing = false;

    [SerializeField]
    private bool _isLimitedLogCount = false;

    [SerializeField]
    private int _maxLogsCount = 25;

    public Color infoColor, errorColor, lootColor, interactColor, dropColor, expColor, serverColor;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _lastHidedLogIndex = 0;
    [SerializeField]
    [Utils.ReadOnly]
    private Queue<Log> _allLogs = new Queue<Log>();
    [SerializeField]
    [Utils.ReadOnly]
    private Queue<Log> _hidedLogs = new Queue<Log>();

    private void Start() {
        onIsLimitedLogCountChanged += _UISettings.SetToggleMaxLogsCount;
        onMaxLogsCountChanged += _UISettings.SetSliderMaxLogsCount;

        _UISettings.Initialize(_maxLogsCount, _isLimitedLogCount);

        Application.logMessageReceived += LogCallback;
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
        _allLogs.Enqueue(log);

        CheckLogLimits();

        if (appendToLogFile && _writeLogsToFile) {
            WriteToLogFile(log);
        }

        ShowPanel();
    }

    public void SetLimitedLog(bool status) {
        _isLimitedLogCount = status;

        CheckLogLimits();

        onIsLimitedLogCountChanged.Invoke(_isLimitedLogCount);
    }

    public void SetMaxLogsCount(float count) {
        _maxLogsCount = (int)count;

        CheckLogLimits();

        onMaxLogsCountChanged.Invoke(_maxLogsCount);
    }

    private void CheckLogLimits() {
        if (_isLimitedLogCount) {
            if (_maxLogsCount < _allLogs.Count) {
                HideLogs();
            } else {
                ShowLogs();
            }
        } else {
            ShowLogs();
        }
    }

    private void ShowLogs() {
        for (int ii = 0; ii < _hidedLogs.Count; ii++) {
            Log log = _hidedLogs.Dequeue();
            log.Show();

            _allLogs.Enqueue(log);
        }
    }

    private void HideLogs() {
        while (_maxLogsCount < _allLogs.Count) {
            Log log = _allLogs.Dequeue();
            log.Hide();

            _hidedLogs.Enqueue(log);
        }
    }

    private Log CreateLogObject(string message, Log.Type logType) {
        string colorStringHEX = "#" + ColorUtility.ToHtmlStringRGB(GetLogColor(logType));

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
            case Log.Type.Server:
            color = serverColor;
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
