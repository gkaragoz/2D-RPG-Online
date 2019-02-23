using ManaShiftServer.Data.RestModels;
using System.Collections;
using UnityEditor;
using UnityEngine;

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

    private void Start() {
        Application.targetFrameRate = 30;

        Bootup();
    }

    private void Bootup() {
        AudioManager.instance.ChangeBackgroundMusic(SceneController.instance.State);

        AccountManager.instance.onAccountManagerInitialized += OnAccountManagerInitialized;
        LoginManager.instance.onLoginCompleted += OnLoginCompleted;
        SceneController.instance.onSceneLoaded += OnSceneLoaded;
        LoadingManager.instance.onLoadingCompleted += OnLoadingCompleted;
        CharacterManager.instance.onCharacterCreated += OnCharacterCreated;
        CharacterManager.instance.onCharacterSelected += OnCharacterSelected;
        RoomManager.instance.onRoomCreated += OnRoomCreated;
        RoomManager.instance.onRoomJoined += OnRoomJoined;
        RoomManager.instance.onRoomLeft += OnRoomLeft;

        Debug.Log("First time play? " + !HasPlayedBefore);

        if (HasPlayedBefore) {
            LoadingManager.instance.Show();
            LoadingManager.instance.AddTask(LoginManager.instance.initializationProgress);
            LoadingManager.instance.AddTask(LoginManager.instance.googlePlaySignInResponseProgress);
            LoadingManager.instance.AddTask(LoginManager.instance.sessionIdResponseProgress);
            LoadingManager.instance.AddTask(LoginManager.instance.accountDataResponseProgress);
            LoadingManager.instance.AddTask(AccountManager.instance.initializationProgress);
            LoadingManager.instance.AddTask(CharacterManager.instance.initializationProgress);

            LoginManager.instance.Initialize();
            LoginManager.instance.Login();
        } else {
            LoadingManager.instance.Show();
            LoadingManager.instance.AddTask(LoginManager.instance.initializationProgress);

            LoginManager.instance.Initialize();
            LoginManager.instance.Login();
        }
    }

    private void OnAccountManagerInitialized() {
        if (!HasPlayedBefore) {
            LoadingManager.instance.Show();
            LoadingManager.instance.AddTask(SceneController.instance.sceneLoadedProgress);
            //Go To Tutorial Scene.
            SceneController.instance.LoadSceneAsync(SceneController.STATE.Tutorial, UnityEngine.SceneManagement.LoadSceneMode.Single);
        } else if (AccountManager.instance.HasCharacter) {
            LoadingManager.instance.Show();
            LoadingManager.instance.AddTask(SceneController.instance.sceneLoadedProgress);
            //Go To Characters Scene selection.
            SceneController.instance.LoadSceneAsync(SceneController.STATE.CharacterSelection, UnityEngine.SceneManagement.LoadSceneMode.Single);
        } else if (!AccountManager.instance.HasCharacter) {
            LoadingManager.instance.Show();
            LoadingManager.instance.AddTask(SceneController.instance.sceneLoadedProgress);
            //Go To Characters Scene creation.
            SceneController.instance.LoadSceneAsync(SceneController.STATE.CharacterCreation, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    private void OnTutorialCompleted() {
        PlayerPrefs.SetInt(HAS_PLAYED_BEFORE, 1);

        LoadingManager.instance.ResetTasks();
        LoadingManager.instance.Show();
        LoadingManager.instance.AddTask(SceneController.instance.sceneLoadedProgress);
        //Go To Characters Scene.
        SceneController.instance.LoadSceneAsync(SceneController.STATE.CharacterCreation, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void OnSceneLoaded() {
        if (SceneController.instance.State == SceneController.STATE.CharacterSelection) {
            CharacterManager.instance.InitializeCharacterSelection();
        } else if (SceneController.instance.State == SceneController.STATE.CharacterCreation) {
            CharacterManager.instance.InitializeCharacterCreation();
        } else if (SceneController.instance.State == SceneController.STATE.Gameplay) {

        }
    }

    private void OnLoadingCompleted() {
        if (SceneController.instance.State == SceneController.STATE.Tutorial) {
            Application.targetFrameRate = 60;
            TutorialManager.instance.onTutorialCompleted += OnTutorialCompleted;
            TutorialManager.instance.StartTutorial();
        } else if (SceneController.instance.State == SceneController.STATE.CharacterSelection) {
            Application.targetFrameRate = 30;
            CharacterManager.instance.HideCharacterCreationMenu();
            CharacterManager.instance.ShowCharacterSelectionMenu();
        } else if (SceneController.instance.State == SceneController.STATE.CharacterCreation) {
            Application.targetFrameRate = 30;
            CharacterManager.instance.HideCharacterSelectionMenu();
            CharacterManager.instance.ShowCharacterCreationMenu();
        } else if (SceneController.instance.State == SceneController.STATE.Gameplay) {
            Application.targetFrameRate = 60;
            MenuManager.instance.Hide();
            RoomManager.instance.Show();
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
        MenuManager.instance.Hide();
        LoadingManager.instance.Show();
        LoadingManager.instance.AddTask(SceneController.instance.sceneLoadedProgress);
        LoadingManager.instance.AddTask(RoomManager.instance.roomManagerInitializedProgress);
        //Go To Gameplay Scene.
        SceneController.instance.LoadSceneAsync(SceneController.STATE.Gameplay, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void OnRoomJoined() {
        MenuManager.instance.Hide();
        LoadingManager.instance.Show();
        LoadingManager.instance.AddTask(SceneController.instance.sceneLoadedProgress);
        LoadingManager.instance.AddTask(RoomManager.instance.roomManagerInitializedProgress);
        //Go To Gameplay Scene.
        SceneController.instance.LoadSceneAsync(SceneController.STATE.Gameplay, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void OnRoomLeft() {
        //Go To Characters Scene.
        LoadingManager.instance.Show();
        LoadingManager.instance.AddTask(SceneController.instance.sceneLoadedProgress);
        SceneController.instance.LoadSceneAsync(SceneController.STATE.CharacterSelection, UnityEngine.SceneManagement.LoadSceneMode.Single);
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
