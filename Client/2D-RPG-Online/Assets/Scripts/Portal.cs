using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to spawn a player.
/// </summary>
public class Portal : MonoBehaviour {

    /// <summary>
    /// Animator component to handle own animations.
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Instantiation object prefab.
    /// </summary>
    public Transform spawnedTransform;

    /// <summary>
    /// That function gets the spawned object prefab from other scripts.
    /// </summary>
    /// <param name="spawnedTransform"></param>
    public void Init(Transform spawnedTransform) {
        this.spawnedTransform = spawnedTransform;
    }

    /// <summary>
    /// Spawn an object.
    /// </summary>
    /// <remarks>
    /// <para>Sets the animation trigger as Move</para>
    /// </remarks>
    public void Spawn() {
        spawnedTransform = Instantiate(spawnedTransform);
        animator.SetTrigger("Move");
    }

    /// <summary>
    /// If spawnedTransform is not null, than keep animating it's position to portal start point position.
    /// </summary>
    private void Update() {
        if (spawnedTransform != null) {
            spawnedTransform.position = animator.transform.position;
        }
    }

}
