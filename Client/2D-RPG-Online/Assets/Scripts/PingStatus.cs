using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PingStatus : MonoBehaviour {

    [Serializable]
    public class PingColor {
        public int baseLimit;
        public int upLimit;
        public Color color;
    }

    [Header("Initialization")]
    [SerializeField]
    private PingColor[] _pingColors;
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private TextMeshProUGUI _txtValue;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private long _currentPingValue;

    private Stopwatch _stopwatch;

    private void Start() {
        NetworkManager.mss.AddEventListener(MSServerEvent.PingRequest, OnPingResponse);

        StartCoroutine(SendPingRequest());
    }

    private IEnumerator SendPingRequest() {
        while (true) {
            if (NetworkManager.mss.IsConnected) {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();

                NetworkManager.mss.SendMessage(MSServerEvent.PingRequest);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void SetPingColor() {
        _txtValue.text = _currentPingValue.ToString() + " ms";

        for (int ii = 0; ii < _pingColors.Length; ii++) {
            if (_currentPingValue >= _pingColors[ii].baseLimit && _currentPingValue <= _pingColors[ii].upLimit) {
                _icon.color = _pingColors[ii].color;
                _txtValue.color = _pingColors[ii].color;
                break;
            }
        }
    }

    private void OnPingResponse(ShiftServerData data) {
        _stopwatch.Stop();

        _currentPingValue = _stopwatch.Elapsed.Milliseconds;

        SetPingColor();
    }

}
