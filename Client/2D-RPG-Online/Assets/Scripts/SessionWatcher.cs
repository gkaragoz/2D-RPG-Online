using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionWatcher : MonoBehaviour {

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _sessionIDDebug;

    private static int _sessionID = 0;

    public static int SessionID {
        get {
            return _sessionID;
        }
    }

    private void Start() {
        _sessionID++;
        _sessionIDDebug = _sessionID;
    }

}
