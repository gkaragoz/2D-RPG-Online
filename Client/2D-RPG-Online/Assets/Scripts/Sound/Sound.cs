using UnityEngine.Audio;
using UnityEngine;

/// <summary>
/// This class is a data structure used in AudioManager.
/// </summary>
/// See <see cref="AudioManager"/> to usages.
[System.Serializable]
public class Sound {

    /// <summary>
    /// Sound name.
    /// </summary>
	public string name;

    /// <summary>
    /// Sound clip data.
    /// </summary>
	public AudioClip clip;

    /// <summary>
    /// Sound's current volume level.
    /// </summary>
	[Range(0f, 1f)]
	public float volume = .75f;
    /// <summary>
    /// Sound's volume variance value to create some variances of sound.
    /// See <see cref="AudioManager.Play(string)"/>
    /// </summary>
	[Range(0f, 1f)]
	public float volumeVariance = .1f;

    /// <summary>
    /// Sound's current pitch level.
    /// </summary>
	[Range(.1f, 3f)]
	public float pitch = 1f;
    /// <summary>
    /// Sound's pitch variance value to create some variances of sound
    /// See <see cref="AudioManager.Play(string)"/>
    /// </summary>
	[Range(0f, 1f)]
	public float pitchVariance = .1f;

    /// <summary>
    /// Play the sound forever.
    /// </summary>
	public bool loop = false;

    /// <summary>
    /// Mixer group holder data.
    /// </summary>
	public AudioMixerGroup mixerGroup;

    /// <summary>
    /// Created AudioSource holder.
    /// See <see cref="AudioManager.Start"/>
    /// </summary>
	[HideInInspector]
	public AudioSource source;

}
