using System;
using TMPro;

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

    public string message;
    public DateTime dateTime;
    public TextMeshProUGUI UI;
    public Type logType;

}
