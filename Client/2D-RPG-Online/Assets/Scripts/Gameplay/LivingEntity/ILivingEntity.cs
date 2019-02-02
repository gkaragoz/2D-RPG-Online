using UnityEngine;

public interface ILivingEntity {

    void TakeDamage(int damage);

    void OnDeath();

    void Attack();

    void MoveByInput();

    void MoveToPosition(Vector3 position);

    void Stop();

    void Rotate();

    void Destroy();

}