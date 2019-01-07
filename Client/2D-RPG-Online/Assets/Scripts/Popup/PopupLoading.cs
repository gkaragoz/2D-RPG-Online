using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupLoading : Menu {

    [SerializeField]
    private Image _loadingBar;
    [SerializeField]
    private float _speed;
    [SerializeField]
    [Utils.ReadOnly]
    private float _currentAmount;

    private void Update() {
        if (_currentAmount < 100) {
            _currentAmount += _speed * Time.deltaTime;
        }

        _loadingBar.fillAmount = _currentAmount / 100;
    }

}
