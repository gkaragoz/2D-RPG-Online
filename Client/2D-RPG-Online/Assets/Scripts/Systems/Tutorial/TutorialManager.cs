using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : Menu {

    #region Singleton

    public static TutorialManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    public Action onTutorialCompleted;

    private void Initialize() {
        if (Camera.main != null) {
            GameObject.Find("DummyCamera").SetActive(false);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;

        Invoke("CompleteTutorial", 5f);
    }
    
    public void CompleteTutorial() {
        onTutorialCompleted?.Invoke();
    }

    public void StartTutorial() {
        PopupManager.instance.ShowPopupMessage("Tutorial!", "Let's learn basics!", PopupMessage.Type.Success);
    }

    public void PauseTutorial() {
        PopupManager.instance.ShowPopupMessage("Tutorial!", "Tutorial paused!", PopupMessage.Type.Info);
    }

    public void ResumeTutorial() {
        PopupManager.instance.ShowPopupMessage("Tutorial!", "Tutorial resumed!", PopupMessage.Type.Info);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
        if (scene.name == "Tutorial") {
            Initialize();
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

}
