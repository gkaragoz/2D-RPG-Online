using System;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is responsible to hold simple Log data.
/// </summary>
[System.Serializable]
public class Log {

    public enum Type {
        Info,
        Error,
        Loot,
        Interact,
        Drop,
        Exp
    }

    private string _message;
    private DateTime _dateTime;
    private string _tracingString;
    private string _colorStringHEX;
    private TextMeshProUGUI _txtLog;
    private Type _logType;

    public Log(string message, DateTime dateTime, string colorStringHEX, TextMeshProUGUI txtLog, Type logType) {
        this._message = message;
        this._dateTime = dateTime;
        this._colorStringHEX = colorStringHEX;
        this._tracingString = System.Environment.StackTrace;
        this._txtLog = txtLog;
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
        UnityEngine.Object.Destroy(this._txtLog.gameObject);
    }

}
