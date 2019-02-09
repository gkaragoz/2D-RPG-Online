using UnityEngine;

public class Skill : MonoBehaviour {

    [Header("Initialization")]
    [SerializeField]
    private Skill_SO _skillDefinition_Template;

    [Header("Debug")]
    [SerializeField]
    [Utils.ReadOnly]
    private Skill_SO _skill;

    public Skill_SO.Skill_Name SkillName {
        get { return _skill._skillName; }
    }

    public Skill_SO.Skill_Type SkillType {
        get { return _skill._skillType; }
    }

    public Skill_SO.Skill_Range SkillRange {
        get { return _skill._skillRange; }
    }

    public Skill_SO.Skill_Behaviour SkillBehaviour {
        get { return _skill._skillBehaviour; }
    }

    #region Initializations

    private void Awake() {
        if (_skillDefinition_Template != null) {
            _skill = Instantiate(_skillDefinition_Template);
        }
    }

    #endregion

    private Vector3 _position;
    private Quaternion _rotation;

    public void PlayAction(Vector3 position, Quaternion rotation, float delay) {
        this._position = position;
        this._rotation = rotation;

        Invoke("PlayActionInvoker", delay);
    }

    public void PlayEffect(Vector3 position, Quaternion rotation, float delay) {
        this._position = position;
        this._rotation = rotation;

        Invoke("PlayEffectInvoker", delay);
    }

    private void PlayActionInvoker() {
        ParticleSystem VFX = Instantiate(_skill._VFX_ActionPrefab, transform);
        VFX.transform.position = this._position;
        VFX.transform.rotation = this._rotation;
        VFX.Play();

        Destroy(VFX.gameObject, VFX.main.duration);
    }

    private void PlayEffectInvoker() {
        ParticleSystem VFX = Instantiate(_skill._VFX_EffectPrefab, transform);
        VFX.transform.position = this._position;
        VFX.transform.rotation = this._rotation;
        VFX.Play();

        Destroy(VFX.gameObject, VFX.main.duration);
    }

}