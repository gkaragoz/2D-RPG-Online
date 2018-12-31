using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible to manage character's real movement.
/// <list type="bullet">
/// Required Components:
/// <item>
/// <term>Rigidbody2D</term>
/// <description>See <see cref="Rigidbody2D"/></description>
/// </item>
/// </list>
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMotor : MonoBehaviour {

    /// <summary>
    /// Character's movement speed.
    /// </summary>
    [Header("Initialization")]
    public float speed = 3f;

    public bool IsMoving {
        get {
            return _rb2D.velocity.magnitude > 0 ? true : false;
        }
    }

    /// <summary>
    /// Rigidbody component to handle collisions.
    /// </summary>
    private Rigidbody2D _rb2D;

    /// <summary>
    /// Get component references
    /// </summary>
    private void Start() {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Actual function does the movement based on Rigidbody2D.
    /// </summary>
    /// <remarks>
    /// <para>Play footstep sound.</para>
    /// </remarks>
    /// <param name="direction"></param>
    public void Move(Vector2 direction) {
        Vector2 currentPosition = transform.position;
        _rb2D.MovePosition(currentPosition + (direction * speed * Time.fixedDeltaTime));

        AudioManager.instance.Play("footstep");
    }

}
