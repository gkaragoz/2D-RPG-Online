using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to any objects that can be able to destroy or destructable.
/// </summary>
public class Destructable : MonoBehaviour {

    /// <summary>
    /// Object's health.
    /// </summary>
    [Header("Initialization")]
    public int health = 3;

    /// <summary>
    /// Animator component for handle destruction animation.
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// Get component references.
    /// </summary>
    private void Start() {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// This function does the real job. 
    /// </summary>
    /// <remarks>
    /// <para>Setting the trigger as a Destruct key.</para>
    /// <para>Playing a destruct sound effect.</para>
    /// </remarks>
    private void Destruct() {
        AudioManager.instance.Play("destruct" + Random.Range(1, 3));
        _animator.SetTrigger("Destruct");
    }

    /// <summary>
    /// Shaking the camera, called on Destruct animations key events.
    /// </summary>
    public void ShakeCamera() {
        CameraShaker.Instance.ShakeOnce(Random.Range(1f, 2f), 5f, 0.1f, 1f);
    }

    /// <summary>
    /// When someone hit to that particular object, this function is triggered to calculate damage taken events.
    /// </summary>
    /// <remarks>
    /// <para>Setting the trigger as a Damage key.</para>
    /// <para>Decreasing the current health of this object.</para>
    /// </remarks>
    public void OnDamageTaken() {
        if (health <= 0) {
            return;
        }

        _animator.SetTrigger("Damage");
        health--;

        if (health <= 0) {
            Destruct();
        }
    }

}
