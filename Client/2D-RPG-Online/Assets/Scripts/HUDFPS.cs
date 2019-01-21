using UnityEngine;
using TMPro;

public class HUDFPS : MonoBehaviour {

    public float updateInterval = 0.5F;
    public TextMeshProUGUI txtFps;

    private float _accum = 0;    // FPS accumulated over the interval
    private int _frames = 0;     // Frames drawn over the interval
    private float _timeleft;     // Left time for current interval

    private void Start() {
        _timeleft = updateInterval;
    }

    private void Update() {
        _timeleft -= Time.deltaTime;
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;
        // Interval ended - update GUI text and start new interval
        if (_timeleft <= 0.0) {
            // display two fractional digits (f2 format)
            float fps = _accum / _frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            txtFps.text = format;

            if (fps < 30) {
                txtFps.color = Color.yellow;
            } else if (fps < 10) { 
                txtFps.color = Color.red;
            } else {
                txtFps.color = Color.green;
            }

            _timeleft = updateInterval;
            _accum = 0.0F;
            _frames = 0;
        }
    }

}