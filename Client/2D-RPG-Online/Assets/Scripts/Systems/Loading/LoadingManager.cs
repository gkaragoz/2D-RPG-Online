using System;
using System.Collections;
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
    [SerializeField]
    private Image _background;
    [SerializeField]
    private Sprite[] _randomBackground;

    [Header("Settings")]
    [SerializeField]
    private float _lerpSpeed;
    [SerializeField]
    private float _delay;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private float _progressFilledAmount;
    [SerializeField]
    [Utils.ReadOnly]
    private float _completedProgressAmount;
    [SerializeField]
    [Utils.ReadOnly]
    private float _divisionProblemAmount;
    [SerializeField]
    [Utils.ReadOnly]
    private List<bool> _checkList = new List<bool>();

    private void Start() {
        _imgFilledBar.fillAmount = 0;

        _background.sprite = _randomBackground[UnityEngine.Random.Range(0, _randomBackground.Length)];
    }

    private void Update() {
        _txtFilledAmount.text = "Loading! %" + (_completedProgressAmount + _divisionProblemAmount);
        _imgFilledBar.fillAmount = Mathf.Lerp(_imgFilledBar.fillAmount, (_completedProgressAmount + _divisionProblemAmount) * 0.01f, Time.deltaTime * _lerpSpeed);
    }

    public void ResetTasks() {
        _checkList = new List<bool>();
    }

    public void AddTask(LoadingTask task) {
        _checkList.Add(false);
        _progressFilledAmount = GetPerProgressFilledAmount();
    }

    public void Progress() {
        _completedProgressAmount = 0;

        for (int ii = 0; ii < _checkList.Count; ii++) {
            _completedProgressAmount += _progressFilledAmount;

            if (_checkList[ii]) {
                continue;
            } else {
                _checkList[ii] = true;
                break;
            }
        }

        if (_completedProgressAmount + _divisionProblemAmount >= 100) {
            StartCoroutine(Delay(() => {
                ResetTasks();
                onLoadingCompleted?.Invoke();
            }));
        }
    }

    private IEnumerator Delay(Action callback) {
        yield return new WaitForSeconds(_delay);
        callback();
    }

    private float GetPerProgressFilledAmount() {
        float amount = (100 / (_checkList.Count == 0 ? 1 : _checkList.Count));
        _divisionProblemAmount = 100 - (amount * _checkList.Count);

        return amount;
    }

}
