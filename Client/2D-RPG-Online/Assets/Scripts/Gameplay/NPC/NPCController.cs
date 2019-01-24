using UnityEngine;

public class NPCController : MonoBehaviour, IAttackable {

    public bool IsDeath {
        get { return _isDeath; }
    }

    private bool _isDeath = false;

    private CharacterStats _characterStats;
    private CharacterController _characterController;
    private PathFollower _pathFollower;

    private void Start() {
        _characterStats = GetComponent<CharacterStats>();
        _characterController = GetComponent<CharacterController>();
        _pathFollower = GetComponent<PathFollower>();

        _characterStats.characterDefinition.onDeath += OnDeath;
    }

    public void TakeDamage(int damage) {
        _characterStats.TakeDamage(damage);
        _characterController.TakeDamage();

        UpdateUI();
    }

    public void OnDeath() {
        _pathFollower.Stop();
        _characterController.OnDeath();

        UpdateUI();
    }

    private void UpdateUI() {
        _characterController.UpdateUI();
    }

    private void OnTriggerEnter(Collider other) {
        PlayerController playerController = (PlayerController)other.gameObject.GetComponentInParent<IAttackable>();
        TakeDamage(playerController.AttackDamage);
    }

}
