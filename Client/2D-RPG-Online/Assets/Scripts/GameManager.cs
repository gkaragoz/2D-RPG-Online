using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    #region Singleton

    public static GameManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [System.Serializable] public class EventGameState : UnityEvent<GameState, GameState> { }

    public enum GameState {
        PREGAME,
        LOADING,
        RUNNING,
        PAUSED
    }

    public EventGameState onGameStateChanged;

    private GameState _currentGameState = GameState.PREGAME;

    public GameState CurrentGameState {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    private void Start() {
        onGameStateChanged.Invoke(GameState.PREGAME, _currentGameState);
    }

    public void TogglePause() {
        UpdateState(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }

    public void RestartGame() {
        UpdateState(GameState.PREGAME);
    }

    public void QuitGame() {
        // Clean up application as necessary
        // Maybe save the players game

        Debug.Log("[GameManager] Quit Game.");
        Application.Quit();
    }

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
