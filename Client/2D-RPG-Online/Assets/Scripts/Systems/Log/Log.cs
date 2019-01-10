using System;

[System.Serializable]
public class Log {

    public enum Type {
        Empty,
        Info,
        Error,
        Loot,
        Interact,
        Drop,
        Exp,
        Server
    }

    private Type _logType;
    private string _body;
    private string _message;
    private DateTime _dateTime;
    private string _tracingString;

    public Log(string message, DateTime dateTime, Type logType) {
        this._message = message;
        this._dateTime = dateTime;
        this._tracingString = System.Environment.StackTrace;
        this._logType = logType;

        string _title = string.Empty;

        switch (logType) {
            case Type.Server:
                _title = "[SERVER]";
                break;
        }

        this._body = string.Format("[{0}] {1} {2}",
                            this._dateTime.ToLongTimeString(), 
                            _title,
                            this._message);
    }

    public string GetBody() {
        return this._body;
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

}
