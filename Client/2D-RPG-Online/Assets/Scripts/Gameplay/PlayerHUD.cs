using TMPro;
using UnityEngine;

public class PlayerHUD : Menu {

    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private TextMeshProUGUI _txtNonAckPlayerInputs;

    public void SetName(string name) {
        _txtName.text = name;
    }

    public void Update(int count) {
        _txtNonAckPlayerInputs.text = count.ToString();
    }

}
