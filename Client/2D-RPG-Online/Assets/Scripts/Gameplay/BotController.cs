﻿using UnityEngine;

public class BotController : LivingEntity {

    public CharacterController CharacterController { get { return _characterController; } }

    private CharacterController _characterController;

    public override void Awake() {
        base.Awake();

        _characterController = GetComponent<CharacterController>();
    }

    private void Start() {
        this.CharacterController.InitializeOffline(this);
    }

    public override void TakeDamage(int damage) {
        CharacterController.TakeDamage(damage);
    }

    public override void OnDeath() {
        CharacterController.OnDeath();
    }

    public override void Attack() {
        CharacterController.Attack();
    }

    public void MoveByInput(Vector3 input) {
        CharacterController.MoveToInput(input);
    }

    public override void MoveToPosition(Vector3 position) {
        CharacterController.MoveToPosition(position);
    }

    public override void Stop() {
        CharacterController.Stop();
    }

    public void Rotate(Vector3 input) {
        CharacterController.Rotate(input);
    }

    public override void Destroy() {
        Destroy(this.gameObject);
    }

}