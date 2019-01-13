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

    private void Initialize() {
        if (Camera.main != null) {
            GameObject.Find("DummyCamera").SetActive(false);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartTutorial() {
        PopupManager.instance.ShowPopupMessage("Tutorial!", "Let's learn basics!", PopupMessage.Type.Success);
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
