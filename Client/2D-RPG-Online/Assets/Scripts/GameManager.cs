using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is responsible to manage the game main states.
/// </summary>
/// <remarks>
/// <para>Current available states are: PREGAME, LOADING, RUNNING, PAUSED</para>
/// </remarks>
public class GameManager : MonoBehaviour {

    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static GameManager instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    /// <summary>
    /// Used on game state changed.
    /// </summary>
    [System.Serializable] public class EventGameState : UnityEvent<GameState, GameState> { }

    /// <summary>
    /// Game states
    /// </summary>
    public enum GameState {
        PREGAME,
        LOADING,
        RUNNING,
        PAUSED
    }

    /// <summary>
    /// Called on game state changed for any reason.
    /// </summary>
    public EventGameState onGameStateChanged;

    /// <summary>
    /// Stores current level name.
    /// </summary>
    private string _currentLevelName;

    /// <summary>
    /// Stores current game state.
    /// </summary>
    private GameState _currentGameState = GameState.PREGAME;

    /// <summary>
    /// Public accessor to currentGameState variable.
    /// </summary>
    public GameState CurrentGameState {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    /// <summary>
    /// Initializing game to PREGAME state. 
    /// </summary>
    private void Start() {
        onGameStateChanged.Invoke(GameState.PREGAME, _currentGameState);
    }

    /// <summary>
    /// This function does toggles between PAUSE and RUNNING game states.
    /// </summary>
    public void TogglePause() {
        UpdateState(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }

    /// <summary>
    /// This function does restarting the game. Basically sets the GameState to PREGAME.
    /// </summary>
    public void RestartGame() {
        UpdateState(GameState.PREGAME);
    }

    /// <summary>
    /// This function does Application.Quit() event.
    /// </summary>
    /// <remarks>
    /// <para>You might be clean up application datas if its necessary.</para>
    /// <para>You might be saving player datas to local database.</para>
    /// </remarks>
    public void QuitGame() {
        // Clean up application as necessary
        // Maybe save the players game

        Debug.Log("[GameManager] Quit Game.");
        Application.Quit();
    }

    /// <summary>
    /// Updating currentState of game.
    /// </summary>
    /// <param name="state"></param>
    private void UpdateState(GameState state) {
        GameState previousGameState = CurrentGameState;
        CurrentGameState = state;

        switch (CurrentGameState) {
            case GameState.PREGAME:
                // Initialize any systems that need to be reset
                Time.timeScale = 1.0f;
                break;
            case GameState.LOADING:
                // Initialize loading stuff.
                Time.timeScale = 1.0f;
                break;
            case GameState.RUNNING:
                // Unlock player, enemies and input in other systems, update tick if you are managing time
                Time.timeScale = 1.0f;
                break;
            case GameState.PAUSED:
                // Pause player, enemies etc, Lock other input in other systems
                Time.timeScale = 0.0f;
                break;

            default:
                break;
        }

        onGameStateChanged.Invoke(_currentGameState, previousGameState);
    }

}
