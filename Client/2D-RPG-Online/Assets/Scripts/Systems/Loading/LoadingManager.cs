using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : Menu {

    public delegate void LoadingCompleteDelegate();
    public LoadingCompleteDelegate onLoadingCompleted;

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

    private void Start() {
        _imgFilledBar.fillAmount = 0;
    }

    private void Update() {
        _txtFilledAmount.text = "Loading! %" + _completedProgressAmount;
        _imgFilledBar.fillAmount = Mathf.Lerp(_imgFilledBar.fillAmount, _completedProgressAmount * 0.01f, Time.deltaTime * _lerpSpeed);
    }

    public void SetCheckList(List<Task> tasks) {
        _checkList = new bool[tasks.Count];
    }

    public void Progress() {
        float progressFilledAmount = GetPerProgressFilledAmount();

        _completedProgressAmount = 0;

        for (int ii = 0; ii < _checkList.Length; ii++) {
            _completedProgressAmount += progressFilledAmount;

            if (_checkList[ii]) {
                continue;
            } else {
                _checkList[ii] = true;
                break;
            }
        }

        if (_completedProgressAmount >= 100) {
            onLoadingCompleted?.Invoke();
        }
    }

    private float GetPerProgressFilledAmount() {
        return (100 / _checkList.Length);
    }

}
