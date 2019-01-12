using UnityEngine;

public class TutorialManager : Menu {

    [SerializeField]
    private GameObject _cameraHolderPrefab;
    [SerializeField]
    private GameObject _dummyCamera;

    private void Start() {
        _cameraHolderPrefab.SetActive(false);

        if (GameObject.Find(_cameraHolderPrefab.name) == null) {
            Instantiate(_cameraHolderPrefab);
        }
    }

}
