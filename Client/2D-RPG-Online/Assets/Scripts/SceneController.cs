using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    #region Singleton

    public static SceneController instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    public enum STATE {
        Main,
        Tutorial,
        CharacterSelection,
        CharacterCreation,
        Lobby,
        Gameplay
    }

    public LoadingTask sceneLoadedProgress;

    public Action onSceneLoaded;

    public STATE State { get; private set; }

    public void LoadSceneAsync(STATE state, LoadSceneMode mode) {
        LoadingManager.instance.Show();
        LoadingManager.instance.AddTask(sceneLoadedProgress);

        this.State = state;

        StartCoroutine(ILoadSceneAsync(mode));
    }

    public void SetState(STATE state) {
        this.State = state;
    }

    private IEnumerator ILoadSceneAsync(LoadSceneMode mode) {
        string sceneName = "";

        switch (this.State) {
            case STATE.Main:
                sceneName = "Main";
                break;
            case STATE.Tutorial:
                sceneName = "Tutorial";
                break;
            case STATE.CharacterSelection:
            case STATE.CharacterCreation:
                sceneName = "Characters";
                break;
            case STATE.Lobby:
                sceneName = "Lobby";
                break;
            case STATE.Gameplay:
                sceneName = "Gameplay";
                break;
            default:
                break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);

        while (!asyncLoad.isDone) {
            yield return null;
        }

        onSceneLoaded?.Invoke();
        sceneLoadedProgress.Complete();
    }

}
