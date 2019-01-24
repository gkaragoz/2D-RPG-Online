using System;
using UnityEngine;

public class CharacterMotor : MonoBehaviour {

    private CharacterStats _characterStats;
    private Rigidbody _rb;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _characterStats = GetComponent<CharacterStats>();
    }

    public void Move(Vector3 direction) {
        _rb.transform.SetPositionAndRotation(
                _rb.transform.position + (direction * _characterStats.GetMovementSpeed() * Time.fixedDeltaTime),
                Quaternion.LookRotation(direction)
        );

        if (AudioManager.instance != null) {
            AudioManager.instance.Play("footstep");
        }
    }

    public void Rotate(Vector3 direction) {
        _rb.transform.rotation = Quaternion.Euler(transform.position - direction);
    }

}
