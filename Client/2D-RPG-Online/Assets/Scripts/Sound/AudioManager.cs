using UnityEngine.Audio;
using System;
using UnityEngine;

/// <summary>
/// This class is responsible to handle Audio system in game.
/// </summary>
/// <remarks>
/// <para>AudioManager operates the sound as Play or be able to check a sound is playing or not.</para>
/// </remarks>
public class AudioManager : MonoBehaviour {

    #region Singleton

    /// <summary>
    /// Instance of this class.
    /// </summary>
    public static AudioManager instance;

    /// <summary>
    /// Initialize Singleton pattern.
    /// </summary>
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(instance);
    }

    #endregion

    /// <summary>
    /// This variable holds all sounds used in the game.
    /// </summary>
    public Sound[] sounds;

    /// <summary>
    /// Initialize, create AudioSource for per sound in the sounds array.
    /// </summary>
    /// <remarks>
    /// Plays a background music called Desecrated Temple.
    /// </remarks>
    private void Start() {
        foreach (Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.loop = sound.loop;

            sound.source.outputAudioMixerGroup = sound.mixerGroup;
        }

        Play("Desecrated Temple");
    }

    /// <summary>
    /// Play a sound for one shot.
    /// </summary>
    /// <param name="sound"></param>
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
        }
    }

    /// <summary>
    /// Check a sound is currently playing or not.
    /// </summary>
    /// <param name="sound"></param>
    /// <returns>Bool for playing or not.</returns>
    private bool IsPlaying(Sound sound) {
        if (sound.source.isPlaying) {
            return true;
        }

        return false;
    }

}
