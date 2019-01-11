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
    private List<bool> _checkList = new List<bool>();

    public void SetCheckList(int checkpoints) {
        _checkList = new List<bool>();
        for (int ii = 0; ii < checkpoints; ii++) {
            _checkList[ii] = false;
        }

        UpdateUI();
    }

    public void CompleteACheckpoint() {
        for (int ii = 0; ii < _checkList.Count; ii++) {
            if (_checkList[ii]) {
                continue;
            } else {
                _checkList[ii] = true;
                break;
            }
        }
    }

    private void UpdateUI() {
        float progressFilledAmount = GetPerProgressFilledAmount();

        float filledAmount = 0;

        for (int ii = 0; ii < _checkList.Count; ii++) {
            if (_checkList[ii]) {
                filledAmount += progressFilledAmount;        
            }
        }

        _imgFilledBar.fillAmount = filledAmount;
        _txtFilledAmount.text = "Loading! %" + filledAmount;
    }

    private float GetPerProgressFilledAmount() {
        return 100 / _checkList.Count;
    }

}
