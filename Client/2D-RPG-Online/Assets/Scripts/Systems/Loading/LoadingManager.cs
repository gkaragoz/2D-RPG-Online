using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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

    public List<LoadingTask> CheckList { get { return _checkList; } }

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

    private List<LoadingTask> _checkList = new List<LoadingTask>();

    private void Start() {
        _imgFilledBar.fillAmount = 0;

        _background.sprite = _randomBackground[UnityEngine.Random.Range(0, _randomBackground.Length)];
    }

    private void Update() {
    }

    public void ResetTasks() {
        _checkList = new List<LoadingTask>();
    }

    public void AddTask(LoadingTask task) {
        _checkList.Add(task);
        _progressFilledAmount = GetPerProgressFilledAmount();
    }

    public void Progress(LoadingTask task) {
        if (task.IsCompleted) {
            _completedProgressAmount += _progressFilledAmount;
        }

        ProgressBar();

        if (_completedProgressAmount + _divisionProblemAmount >= 100) {
            StartCoroutine(Delay(() => {
                ResetTasks();
                Hide();

                onLoadingCompleted?.Invoke();
            }));
        }
    }

    private IEnumerator Delay(Action callback) {
        yield return new WaitForSeconds(_delay);
        callback();
    }

    private void ProgressBar() {
        _txtFilledAmount.text = "Loading! %" + (_completedProgressAmount + _divisionProblemAmount);
        _imgFilledBar.fillAmount = Mathf.Lerp(_imgFilledBar.fillAmount, (_completedProgressAmount + _divisionProblemAmount) * 0.01f, Time.deltaTime * _lerpSpeed);
    }

    private float GetPerProgressFilledAmount() {
        float amount = (100 / (_checkList.Count == 0 ? 1 : _checkList.Count));
        _divisionProblemAmount = 100 - (amount * _checkList.Count);

        return amount;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(LoadingManager))]
public class ExampleScriptEditor : Editor {
    public LoadingManager loadingManager;

    public void OnEnable() {
        loadingManager = (LoadingManager)target;
    }

    public override void OnInspectorGUI() {
        GUI.backgroundColor = Color.cyan;

        base.OnInspectorGUI();

        if (loadingManager.CheckList.Count == 0) {
            return;
        }

        GUILayout.Space(10f);
        GUILayout.Label("Check List");

        for (int ii = 0; ii < loadingManager.CheckList.Count; ii++) {
            if (loadingManager.CheckList[ii].IsCompleted) {
                GUIStyle style = new GUIStyle(EditorStyles.textField);
                style.normal.textColor = Color.green;
                GUILayout.Label(loadingManager.CheckList[ii].Name + ":" + loadingManager.CheckList[ii].IsCompleted.ToString(), style);
            } else {
                GUIStyle style = new GUIStyle(EditorStyles.textField);
                style.normal.textColor = Color.red;
                GUILayout.Label(loadingManager.CheckList[ii].Name + ":" + loadingManager.CheckList[ii].IsCompleted.ToString(), style);
            }
            GUILayout.Space(10f);
        }

        GUI.backgroundColor = Color.cyan;
    }
}
#endif