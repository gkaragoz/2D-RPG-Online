using System;
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

    [Header("Settings")]
    [SerializeField]
    private float _lerpSpeed;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private float _completedProgressAmount;
    [SerializeField]
    [Utils.ReadOnly]
    private bool[] _checkList;

    private void Update() {
        _imgFilledBar.fillAmount = Mathf.Lerp(_imgFilledBar.fillAmount, _completedProgressAmount * 0.01f, Time.deltaTime * _lerpSpeed);
    }

    public void SetCheckList(List<Task> tasks) {
        _checkList = new bool[tasks.Count];

        UpdateUI();
    }

    public void Progress() {
        for (int ii = 0; ii < _checkList.Length; ii++) {
            if (_checkList[ii]) {
                continue;
            } else {
                _checkList[ii] = true;
                break;
            }
        }

        UpdateUI();
    }

    private void UpdateUI() {
        float progressFilledAmount = GetPerProgressFilledAmount();

        _completedProgressAmount = 0;

        for (int ii = 0; ii < _checkList.Length; ii++) {
            if (_checkList[ii]) {
                _completedProgressAmount += progressFilledAmount;        
            }
        }

        _txtFilledAmount.text =  "%" + _completedProgressAmount;
    }

    private float GetPerProgressFilledAmount() {
        return (100 / _checkList.Length);
    }

}
