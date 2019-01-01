using UnityEngine;

public class LoginManager : Menu {

    #region Singleton

    public static LoginManager instance;
    public ClientData clientInfo;


    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    private void Start() {
        if (!NetworkManager.instance.IsOffline) {

            clientInfo = new ClientData();
            clientInfo.Loginname = "Test";
            clientInfo.MachineId = SystemInfo.deviceUniqueIdentifier;
            clientInfo.MachineName = SystemInfo.deviceName;
        }

    }
}
