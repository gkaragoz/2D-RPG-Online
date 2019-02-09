using UnityEngine;

public class Skill : MonoBehaviour {

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

    private ParticleSystem _VFX;

    public void Initialize(Skill_SO skill) {
        this._skill = Instantiate(skill);
    }

    public void Run(Vector3 startPosition) {
        _VFX = Instantiate(_skill._VFX_ActionPrefab, transform);
        _VFX.transform.position = startPosition;

        Vector3 relativePos = startPosition - transform.position;
        relativePos.y = 0;

        if (relativePos != Vector3.zero) {
            _VFX.transform.rotation = Quaternion.LookRotation(relativePos);
        }

        _VFX.Play();

        Destroy(_VFX.gameObject, _VFX.main.duration);
    }

    public void Run(Vector3 startPosition, Transform target) {
        _VFX = Instantiate(_skill._VFX_ActionPrefab, transform);
        _VFX.transform.position = startPosition;

        Vector3 relativePos = target.position - transform.position;
        if (relativePos != Vector3.zero) {
            _VFX.transform.rotation = Quaternion.LookRotation(relativePos);
        }

        _VFX.Play();

        Destroy(_VFX.gameObject, _VFX.main.duration);
    }

}