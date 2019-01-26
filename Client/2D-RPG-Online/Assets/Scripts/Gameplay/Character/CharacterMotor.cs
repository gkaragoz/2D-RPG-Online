using System;
using UnityEngine;

public class CharacterMotor : MonoBehaviour {

    private CharacterStats _characterStats;
    private Rigidbody _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _characterStats = GetComponent<CharacterStats>();
    }

    public void MoveToInput(Vector3 input) {
        _rb.transform.SetPositionAndRotation(
            _rb.position + (input * _characterStats.GetMovementSpeed() * Time.fixedDeltaTime),
            Quaternion.LookRotation(input));

        if (AudioManager.instance != null) {
            AudioManager.instance.Play("footstep");
        }
    }

    public void MoveToPosition(Vector3 position) {
        Vector3 desiredRotation = position - transform.position;

        _rb.transform.SetPositionAndRotation(
            position, 
            Quaternion.Euler(desiredRotation));

        if (AudioManager.instance != null) {
            AudioManager.instance.Play("footstep");
        }
    }

    public void LookTo(Vector3 direction) {
        if (direction != Vector3.zero) {
            Vector3 relativePos = direction - transform.position;
            _rb.MoveRotation(Quaternion.LookRotation(relativePos));
        }
    }

    public void Rotate(Vector3 direction) {
        _rb.rotation = Quaternion.Euler(direction - transform.position);
    }

}
