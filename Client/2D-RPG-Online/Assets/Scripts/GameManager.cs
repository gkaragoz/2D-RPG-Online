using ManaShiftServer.Data.RestModels;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public LoadingTask accountDataResponseProgress;

    public const string HAS_PLAYED_BEFORE = "HAS_PLAYED_BEFORE";

    public bool HasPlayedBefore {
        get {
            return PlayerPrefs.GetInt(HAS_PLAYED_BEFORE) == 1 ? true : false;
        }
    }

    private const string ATTEMP_TO_CREATE_CHARACTER = "ATTEMP to create character!";
    private const string ERROR_CREATE_CHARACTER = "ERROR on create character!";
    private const string SUCCESS_CREATE_CHARACTER = "SUCCESS on create character!";

    private const string ERROR_SIGN_IN_TITLE = "Ups! Google Play!";
    private const string ERROR_SIGN_IN_MESSAGE = "You must logged in your Google account to save your account informations!";

    private const string ERROR_GET_SESSION_ID_TITLE = "Authorization Problem!";
    private const string ERROR_GET_SESSION_ID_MESSAGE = "Some problem occured on server!";

    private const string ERROR_GET_ACCOUNT_DATA_TITLE = "Ups! Account!";
    private const string ERROR_GET_ACCOUNT_DATA_MESSAGE = "Some problem occured on server!";

    private const string ERROR_GET_ID_TOKEN_TITLE = "Wow!";
    private const string ERROR_GET_ID_TOKEN_MESSAGE = "Some problem occured on server!";

    private Scene _currentScene;

    private void Start() {
        Application.targetFrameRate = 60;

        StartCoroutine(Bootup());
    }

    private IEnumerator Bootup() {
        AudioManager.instance.ChangeBackgroundMusic(SceneManager.GetActiveScene());

        yield return new WaitForSeconds(3f);

        LoginManager.instance.onLoginCompleted += OnLoginCompleted;
        LoadingManager.instance.onLoadingCompleted += OnLoadingCompleted;
        CharacterManager.instance.onCharacterCreated += OnCharacterCreated;
        CharacterManager.instance.onCharacterSelected += OnCharacterSelected;
        RoomManager.instance.onRoomCreated += OnRoomCreated;
        RoomManager.instance.onRoomJoined += OnRoomJoined;
        RoomManager.instance.onRoomLeft += OnRoomLeft;

        Debug.Log("First time play? " + !HasPlayedBefore);

        if (HasPlayedBefore) {
            LoadingManager.instance.ResetTasks();
            LoadingManager.instance.AddTask(LoginManager.instance.initializationProgress);
            LoadingManager.instance.AddTask(LoginManager.instance.googlePlaySignInResponseProgress);
            LoadingManager.instance.AddTask(LoginManager.instance.sessionIdResponseProgress);
            LoadingManager.instance.AddTask(LoginManager.instance.accountDataResponseProgress);
            LoadingManager.instance.AddTask(AccountManager.instance.initializationProgress);
            LoadingManager.instance.AddTask(CharacterManager.instance.initializationProgress);

            LoginManager.instance.Initialize();
            LoginManager.instance.Login();
        } else {
            LoadingManager.instance.ResetTasks();
            LoadingManager.instance.AddTask(LoginManager.instance.initializationProgress);

            LoginManager.instance.Initialize();
        }
    }

    private void OnTutorialCompleted() {
        PlayerPrefs.SetInt(HAS_PLAYED_BEFORE, 1);

        SceneManager.UnloadSceneAsync("Tutorial");

        CharacterManager.instance.ShowCharacterCreationMenu();
    }

    private void OnLoadingCompleted() {
        LoadingManager.instance.Hide();

        if (!HasPlayedBefore) {
            //Go To Tutorial Scene.
            SceneManager.LoadScene("Tutorial", LoadSceneMode.Additive);

            _currentScene = SceneManager.GetSceneByName("Tutorial");

            TutorialManager.instance.onTutorialCompleted += OnTutorialCompleted;
            TutorialManager.instance.StartTutorial();

            LoginManager.instance.Login();

            TutorialManager.instance.PauseTutorial();
        } else {
            //Open Character Selection Menu.
            CharacterManager.instance.ShowCharacterSelectionMenu();
        }
    }

    private void OnLoginCompleted() {
        if (!HasPlayedBefore) {
            TutorialManager.instance.ResumeTutorial();
        }
    }

    private void OnCharacterCreated(CharacterModel newCharacter) {
        CharacterManager.instance.HideCharacterCreationMenu();
        MenuManager.instance.Show();
    }

    private void OnCharacterSelected(CharacterModel selectedCharacter) {
        CharacterManager.instance.HideCharacterSelectionMenu();
        MenuManager.instance.Show();
    }

    private void OnRoomCreated() {
        //Go To Gameplay Scene.
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);

        _currentScene = SceneManager.GetSceneByName("Gameplay");

        AudioManager.instance.ChangeBackgroundMusic(_currentScene);

        MenuManager.instance.Hide();
        RoomManager.instance.Show();
    }

    private void OnRoomJoined() {
        //Go To Gameplay Scene.
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);

        _currentScene = SceneManager.GetSceneByName("Gameplay");

        AudioManager.instance.ChangeBackgroundMusic(_currentScene);

        MenuManager.instance.Hide();
        RoomManager.instance.Show();
    }

    private void OnRoomLeft() {
        //Go To Menu Scene.
        SceneManager.UnloadSceneAsync("Gameplay");

        _currentScene = SceneManager.GetSceneByName("Main");

        AudioManager.instance.ChangeBackgroundMusic(_currentScene);

        RoomManager.instance.Hide();
        MenuManager.instance.Show();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class ExampleScriptEditor : Editor {
        public GameManager gameManager;

        public void OnEnable() {
            gameManager = (GameManager)target;
        }

        public override void OnInspectorGUI() {
            GUI.backgroundColor = PlayerPrefs.HasKey(HAS_PLAYED_BEFORE) ? Color.red : Color.green;
            GUILayout.Space(10f);
            GUILayout.Label("Player Prefs");
            if (GUILayout.Button("RESET")) {
                PlayerPrefs.DeleteAll();

                EditorWindow view = EditorWindow.GetWindow<SceneView>();
                view.Repaint();
            }
            GUI.backgroundColor = Color.white;

            base.OnInspectorGUI();
        }
    }
#endif

}
