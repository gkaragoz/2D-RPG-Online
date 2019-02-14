﻿using UnityEngine;

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

    public enum VFXType {
        Action,
        Effect
    }

    private ParticleSystem _VFX;

    public void Initialize(Skill_SO skill) {
        this._skill = Instantiate(skill);
    }

    public void Run(Vector3 startPosition, VFXType vfxType) {
        switch (vfxType) {
            case VFXType.Action:
                _VFX = Instantiate(_skill._VFX_ActionPrefab, transform);
                break;
            case VFXType.Effect:
                _VFX = Instantiate(_skill._VFX_EffectPrefab, transform);
                break;
        }

        switch (this._skill._skillRange) {
            case Skill_SO.Skill_Range.Melee:
                _VFX.transform.position = startPosition;

                Vector3 relativePos = startPosition - transform.position;
                relativePos.y = 0;

                if (relativePos != Vector3.zero) {
                    _VFX.transform.rotation = Quaternion.LookRotation(relativePos);
                }

                break;
            case Skill_SO.Skill_Range.Ranged:
                Projectile projectile = _VFX.gameObject.AddComponent<Projectile>();
                projectile.SetTarget(null);
                break;
            default:
                break;
        }

        _VFX.Play();

        Destroy(gameObject, _VFX.main.duration);
    }

    public void Run(Vector3 startPosition, Transform target, VFXType vfxType) {
        switch (vfxType) {
            case VFXType.Action:
                _VFX = Instantiate(_skill._VFX_ActionPrefab, transform);
                break;
            case VFXType.Effect:
                _VFX = Instantiate(_skill._VFX_EffectPrefab, transform);
                break;
        }

        switch (this._skill._skillRange) {
            case Skill_SO.Skill_Range.Melee:
                _VFX.transform.position = startPosition;

                Vector3 relativePos = target.position - transform.position;
                if (relativePos != Vector3.zero) {
                    _VFX.transform.rotation = Quaternion.LookRotation(relativePos);
                }

                break;
            case Skill_SO.Skill_Range.Ranged:
                Projectile projectile = _VFX.gameObject.AddComponent<Projectile>();
                projectile.SetTarget(target);
                break;
            default:
                break;
        }


        _VFX.Play();

        Destroy(gameObject, _VFX.main.duration);
    }

}