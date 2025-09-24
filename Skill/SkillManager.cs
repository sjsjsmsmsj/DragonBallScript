using UnityEngine;
using System.Collections.Generic;
using System;

public class SkillManager : MonoBehaviour
{
    public const int DAM_GALICK = 1;
    public const int HOA_KHI_DOT_KHONG_LO = 2;
    public const int ATOMIC = 3;
    public const int PHAO_RADITZ = 4;
    public const int DONG_MAU_SAIYAN = 5;
    public const int CONG_KICH_TOAN_LUC = 6;
    public const int TAI_TAO_NANG_LUONG = 7;
    public const int BODY_STRENGTHEN = 8;

    [SerializeField] private List<Skill> skillRefabs = new List<Skill>();
    [SerializeField] private int sizePoolNormalHit = 10;
    [SerializeField] private WarriorManager warriorManager;
    private GameManager gameManager;
    private List<NormalHit> poolNormalHit = new List<NormalHit>();
    public static event Action<NormalHit, Character> CharUseNormalHitSkill;
    public static event Action<AtomicSkill, Character> CharUseAtomicSkill;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        for (int i = 0; i < sizePoolNormalHit; i++)
        {
            NormalHit normalHit = CreateSkillByID(DAM_GALICK, Vector3.zero, Quaternion.identity) as NormalHit;
            if (normalHit != null)
            {
                normalHit.gameObject.SetActive(false);
                poolNormalHit.Add(normalHit);
            }
        }
    }

    public Skill CreateSkillByID(int skillID, Vector3 pos, Quaternion rotate)
    {
        Skill skill = FindSkillRefabsByID(skillID);
        if (skill == null) {
            Debug.Log("Skill id: " + skillID.ToString() + "not available !");
            return null;
        }
        GameObject skillInit = Instantiate(skill.gameObject, pos, rotate);
        return skillInit.GetComponent<Skill>();
    }
    public NormalHit UseNormalHitSkill(Vector3 pos, Character owner, Character target)
    {
        if (owner == null || owner.GetIsDie() || target == null || target.GetIsDie()) return null;
        NormalHit normalHit = GetNormalHitFromPool();
        if (normalHit == null)
        {
            Debug.Log("Create Normal hit skill failed !");
            return null;
        }
        CharUseNormalHitSkill?.Invoke(normalHit, owner);
        normalHit.SetOwner(owner);
        normalHit.SetTarget(target);
        normalHit.transform.position = pos;
        normalHit.transform.localScale = owner.transform.localScale;
        return normalHit;
    }
    public AtomicSkill UseAtomicSkill(Vector3 pos, Character owner, Character target)
    {
        if (owner == null || owner.GetIsDie() || target == null || target.GetIsDie()) return null;
        Vector2 direc = target.transform.position - pos;
        float angle = Mathf.Atan2(direc.y, direc.x) * Mathf.Rad2Deg;
        AtomicSkill atomicSkill = CreateSkillByID(ATOMIC, pos, Quaternion.Euler(0, 0, angle)) as AtomicSkill;
        if (atomicSkill == null)
        {
            Debug.Log("Create Normal hit skill failed !");
            return null;
        }
        CharUseAtomicSkill?.Invoke(atomicSkill, owner);
        atomicSkill.SetOwner(owner);
        atomicSkill.SetTarget(target.transform.position);
        gameManager?.CreateEffectAtPos(EffectManager.ENERGY, owner.characterPos.EnergyEffectPos.position,
            atomicSkill.GetTimeCharg(), owner.transform);
        return atomicSkill;
    }
    public RaditzBombSkill UseRaditzBomSkill(Vector3 pos, Character owner, Character target)
    {
        if (owner == null || owner.GetIsDie() || target == null || target.GetIsDie()) return null;
        Vector2 direc = target.transform.position - pos;
        float angle = Mathf.Atan2(direc.y, direc.x) * Mathf.Rad2Deg;
        RaditzBombSkill raditzBombSkill = CreateSkillByID(PHAO_RADITZ, pos, Quaternion.Euler(0, 0, angle)) as RaditzBombSkill;
        if (raditzBombSkill == null)
        {
            Debug.Log("Create Normal hit skill failed !");
            return null;
        }
        CharUseAtomicSkill?.Invoke(raditzBombSkill, owner);
        raditzBombSkill.SetOwner(owner);
        raditzBombSkill.SetTarget(target.transform.position);
        return raditzBombSkill;
    }
    public GiantApeSkill UseGiantApeSkill(Character owner)
    {
        if (owner == null || owner.GetIsDie()) return null;
        GiantApeSkill giantApeSkill =
            CreateSkillByID(HOA_KHI_DOT_KHONG_LO, Vector3.zero, Quaternion.identity) as GiantApeSkill;
        if (giantApeSkill == null)
        {
            Debug.Log("Create Giant Ape skill failed !");
            return null;
        }
        giantApeSkill.SetOwner(owner);
        giantApeSkill.SetGameManager(gameManager);
        CharacterAttack characterAttack =  owner.GetComponent<CharacterAttack>();
        owner.SetIsActive(false);
        if (characterAttack != null) characterAttack.SetIsAttack(false);
        return giantApeSkill;
    }
    public SaiyanBloodSkill UseSaiyanBloodSkill(Character owner)
    {
        if (owner == null || owner.GetIsDie()) return null;
        SaiyanBloodSkill saiyanBloodSkill =
            CreateSkillByID(DONG_MAU_SAIYAN, Vector3.zero, Quaternion.identity) as SaiyanBloodSkill;
        if (saiyanBloodSkill == null)
        {
            Debug.Log("Create saiyan Blood Skill failed !");
            return null;
        }
        saiyanBloodSkill.SetOwner(owner);
        saiyanBloodSkill.SetGameManager(gameManager);
        saiyanBloodSkill.SetWarriorManager(warriorManager);
        return saiyanBloodSkill;
    }
    public FullAttackSkill UseFullAttackSkill(Vector3 pos, Character owner, Character target)
    {
        if(owner == null || owner.GetIsDie() || target == null || target.GetIsDie()) return null;
        FullAttackSkill fullAttackSkill = CreateSkillByID(CONG_KICH_TOAN_LUC, pos, Quaternion.identity) 
            as FullAttackSkill;
        if (fullAttackSkill == null) return null;
        fullAttackSkill.SetOwner(owner);
        fullAttackSkill.SetTarget(target);
        return fullAttackSkill;
    }
    public RecoveryEnergySkill UseRecoveryEnergySkill(Vector3 pos, Character owner)
    {
        if (owner == null || owner.GetIsDie()) return null;
        RecoveryEnergySkill recoveryEnergySkill = CreateSkillByID(TAI_TAO_NANG_LUONG, pos, Quaternion.identity)
            as RecoveryEnergySkill;
        if (recoveryEnergySkill == null) return null;
        recoveryEnergySkill.SetOwner(owner);
        recoveryEnergySkill.SetGameManager(gameManager);
        return recoveryEnergySkill;
    }
    public NormalHit GetNormalHitFromPool()
    {
        foreach (var normalHit in poolNormalHit)
        {
            if(normalHit.gameObject.activeSelf == false)
            {
                normalHit.gameObject.SetActive(true);
                return normalHit;
            }
        }
        NormalHit newNormalHit = CreateSkillByID(DAM_GALICK, Vector3.zero, Quaternion.identity) as NormalHit;
        if (newNormalHit != null)
        {
            poolNormalHit.Add(newNormalHit);
        }
        return newNormalHit;
    }
    public BodyStrengthen UseBodyStrengthenSkill(Character owner)
    {
        if (owner == null || owner.GetIsDie()) return null;
        BodyStrengthen BodyStrengthenSkill = CreateSkillByID(BODY_STRENGTHEN, Vector3.zero, Quaternion.identity)
            as BodyStrengthen;
        if (BodyStrengthenSkill == null) return null;
        BodyStrengthenSkill.SetOwner(owner);
        return BodyStrengthenSkill;
    }
    public Skill FindSkillRefabsByID(int skillID)
    {
        foreach (var skill in skillRefabs)
        {
            if(skill == null) continue;
            if(skill.GetSkillID() == skillID)
            {
                return skill;
            }
        }
        return null;
    }

}