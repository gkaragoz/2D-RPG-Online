using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

    #region Singleton

    public static AudioManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    [Header("Initialize")]
    public Sound[] sounds;

    [Header("Settings")]
    [SerializeField]
    private bool _playBgMusic = false;

    [Header("Debug")]
    [Utils.ReadOnly]
    private string _currentBgMusic = string.Empty;

    private void Start() {
        foreach (Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.loop = sound.loop;

            sound.source.outputAudioMixerGroup = sound.mixerGroup;
        }

        if (_playBgMusic) {
            Play("MenuMusic");
        }
    }

    public void Play(string sound) {
        Sound tempSound = Array.Find(sounds, item => item.name == sound);
        if (tempSound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        tempSound.source.volume = tempSound.volume * (1f + UnityEngine.Random.Range(-tempSound.volumeVariance / 2f, tempSound.volumeVariance / 2f));
        tempSound.source.pitch = tempSound.pitch * (1f + UnityEngine.Random.Range(-tempSound.pitchVariance / 2f, tempSound.pitchVariance / 2f));

        if (!IsPlaying(tempSound)) {
            tempSound.source.Play();

            switch (tempSound.type) {
                case Sound.Type.FX:
                    break;
                case Sound.Type.Music:
                    _currentBgMusic = tempSound.name;
                    break;
                default:
                    break;
            }
        }
    }

    public void Stop(string sound) {
        Sound tempSound = Array.Find(sounds, item => item.name == sound);
        if (tempSound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        tempSound.source.Stop();
    }

    public void StopAllSounds() {
        foreach (Sound sound in sounds) {
            if (IsPlaying(sound)) {
                sound.source.Stop();
            }
        }
    }

    public void ChangeBackgroundMusic(Scene scene) {
        if (_currentBgMusic != string.Empty) {
            Stop(_currentBgMusic);
        }

        if (scene.name == "Gameplay") {
            StopAllSounds();

            int random = UnityEngine.Random.Range(0, 1);
            if (random == 0) {
                Play("BattleMusic1");
            } else {
                Play("BattleMusic2");
            }
        }
    }

    private bool IsPlaying(Sound sound) {
        if (sound.source.isPlaying) {
            return true;
        }

        return false;
    }

}
