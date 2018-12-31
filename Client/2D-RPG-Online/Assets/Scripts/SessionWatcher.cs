using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to count developer or player hit the play button.
/// </summary>
/// <remarks>
/// <para>Work with ScriptableObject</para>
/// </remarks>
public class SessionWatcher : MonoBehaviour {
    
    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static SessionWatcher instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    /// <summary>
    /// Session data scriptable object.
    /// </summary>
    [Header("Initialization")]
    [SerializeField]
    private Session_SO _session;

    /// <summary>
    /// To show current session ID on inspector.
    /// </summary>
    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private int _sessionIDDebug;

    /// <summary>
    /// Getter of SessionID.
    /// </summary>
    public int SessionID {
        get {
            return _session.ID;
        }
    }

    /// <summary>
    /// Increment sessionID just one.
    /// </summary>
    private void Start() {
        _session.ID++;

        _sessionIDDebug = _session.ID;
    }
}

/// <summary>
/// This class is a scriptable object for SessionWatcher class.
/// See <see cref="SessionWatcher"/>
/// </summary>
[CreateAssetMenu(fileName = "SessionData", menuName = "Session/Create Session", order = 1)]
public class Session_SO : ScriptableObject {
    public int ID;
}