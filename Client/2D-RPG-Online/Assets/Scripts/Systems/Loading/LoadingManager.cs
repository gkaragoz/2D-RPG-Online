using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : Menu {

    #region Singleton

    public static LoadingManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [SerializeField]
    private Image _imgFilledBar;
    [SerializeField]
    private TextMeshProUGUI _txtFilledAmount;

    [SerializeField]
    [Utils.ReadOnly]
    private bool[] _checkList;

    public void SetCheckList(int checkpoints) {
        _checkList = new bool[checkpoints];
        for (int ii = 0; ii < checkpoints; ii++) {
            _checkList[ii] = false;
        }

        UpdateUI("Loading! %0");
    }

    public void Progress(string progressName) {
        for (int ii = 0; ii < _checkList.Length; ii++) {
            if (_checkList[ii]) {
                continue;
            } else {
                _checkList[ii] = true;
                break;
            }
        }

        UpdateUI(progressName);
    }

    private void UpdateUI(string progressName) {
        float progressFilledAmount = GetPerProgressFilledAmount();

        float filledAmount = 0;

        for (int ii = 0; ii < _checkList.Length; ii++) {
            if (_checkList[ii]) {
                filledAmount += progressFilledAmount;        
            }
        }

        _imgFilledBar.fillAmount = filledAmount * 0.01f;
        _txtFilledAmount.text = progressName + " %" + filledAmount;
    }

    private float GetPerProgressFilledAmount() {
        return (100 / _checkList.Length);
    }

}
