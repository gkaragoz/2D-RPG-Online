using TMPro;
using UnityEngine;

public class PlayerHUD : Menu {

    [SerializeField]
    private TextMeshProUGUI _txtName;

    public void SetName(string name) {
        _txtName.text = name;
    }

}
