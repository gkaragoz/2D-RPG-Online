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

    public LoadingTask sceneLoadedProgress;

    public Action<string> onSceneLoaded;

    public Scene GetActiveScene() {
        return SceneManager.GetActiveScene();
    }

    public void LoadSceneAsync(string name, LoadSceneMode mode) {
        LoadingManager.instance.Show();
        LoadingManager.instance.AddTask(this.sceneLoadedProgress);

        StartCoroutine(ILoadSceneAsync(name, mode));
    }

    private IEnumerator ILoadSceneAsync(string name, LoadSceneMode mode) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name, mode);

        while (!asyncLoad.isDone) {
            yield return null;
        }

        onSceneLoaded?.Invoke(name);
        sceneLoadedProgress?.Invoke();
    }

}
