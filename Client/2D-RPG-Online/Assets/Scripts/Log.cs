using System;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is responsible to hold simple Log data.
/// </summary>
public class Log : MonoBehaviour {

    public enum Type {
        Empty,
        Info,
        Error,
        Loot,
        Interact,
        Drop,
        Exp
    }

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private Type _logType;
    [SerializeField]
    [Utils.ReadOnly]
    private string _message;
    [SerializeField]
    [Utils.ReadOnly]
    private DateTime _dateTime;
    [SerializeField]
    [Utils.ReadOnly]
    private string _tracingString;
    [SerializeField]
    [Utils.ReadOnly]
    private string _colorStringHEX;
    [SerializeField]
    [Utils.ReadOnly]
    private TextMeshProUGUI _txtLog;

    private void Start() {
        _txtLog = GetComponent<TextMeshProUGUI>();
    }

    public void Init(string message, DateTime dateTime, string colorStringHEX, Type logType) {
        this._message = message;
        this._dateTime = dateTime;
        this._colorStringHEX = colorStringHEX;
        this._tracingString = System.Environment.StackTrace;
        this._logType = logType;

        this._txtLog.text = string.Format("[{0}] <color={1}>{2}</color>", 
                            this._dateTime.ToLongTimeString(), 
                            colorStringHEX, 
                            this._message);
    }

    public string GetMessage() {
        return this._message;
    }

    public string GetFormattedDateTime() {
        return string.Format("{0:d/M/yyyy HH:mm:ss}", this._dateTime);
    }

    public string GetTracingString() {
        return _tracingString;
    }

    public void DestroyItself() {
        Destroy(this.gameObject);
    }

}
