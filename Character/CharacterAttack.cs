using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public static class SkillType
{
    public const int HIT = 0;
    public const int ULTI = 1;
    public const int SKILL_SECOND = 2;
    public const int NUM_TYPE = 3;
}
public class SkillCooldown
{
    public int skillByID { get; set; }
    public float cooldown { get; set; }
    public float lastTimeUse { get; set; }
    public SkillCooldown()
    {
        skillByID = -1;
    }
    public SkillCooldown(int skillByID, float cooldown)
    {
        this.skillByID = skillByID;
        this.cooldown = cooldown;
        lastTimeUse = 0f;
    }
    public bool IsSkillReady()
    {
        if (skillByID == -1) return false;
        if (lastTimeUse > 0f)
            return Time.time >= lastTimeUse + cooldown;
        return true;
    }
}
public class CharacterAttack : MonoBehaviour
{
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected float attackZone;
    protected GameManager gameManager;
    protected Character character;
    protected Transform target;
    protected CharacterMove characterMove;
    protected List<SkillCooldown> skillCooldowns = new List<SkillCooldown>();
    protected Animator animator;
    protected bool isAttack;
    public float minRangeSkill { set; get; }
    public CharacterPos characterPos { get; protected set; }
    protected virtual void Start()
    {
        character = GetComponent<Character>();
        characterMove = GetComponent<CharacterMove>();
        animator = GetComponent<Animator>();
        characterPos = GetComponent<CharacterPos>();
        gameManager = character.gameManager;
        //init SkillCooldown class
        minRangeSkill = float.PositiveInfinity;
        foreach (var skill in character.GetSkillPrefaps())
        {
            SkillCooldown skillCooldown = new SkillCooldown(skill.GetSkillID(), skill.GetCoolDown());
            skillCooldowns.Add(skillCooldown);
            if(minRangeSkill > skill.GetRangeAttack()) minRangeSkill = skill.GetRangeAttack();
        }
        if (skillCooldowns.Count < SkillType.NUM_TYPE)
        {
            int n = SkillType.NUM_TYPE - skillCooldowns.Count;
            for (int i = 0; i < n; i++)
            {
                skillCooldowns.Add(new SkillCooldown());
            }
        }
    }
    public virtual void Update()
    {
        if (character.GetIsDie() || !isAttack || gameManager.isPauseGame) return;
        // choose target in attack zone
        if (!IsTargetInAttackZone(target, attackZone)) GetNearestTarget(transform, attackZone);
        if (target == null || target.GetComponent<Character>().GetIsDie()) return;
        characterMove.SetTarget(target, minRangeSkill);
        //Handle use skill
        if (skillCooldowns.Count >= SkillType.ULTI && skillCooldowns[SkillType.ULTI].IsSkillReady())
        {
            if (gameManager.UseSkill(skillCooldowns[SkillType.ULTI].skillByID, character,
                characterPos.UltiPos.position))
            {
                skillCooldowns[SkillType.ULTI].lastTimeUse = Time.time;
                animator.SetTrigger("useUlti");
                return;
            }
        }
        if (skillCooldowns.Count >= SkillType.SKILL_SECOND && skillCooldowns[SkillType.SKILL_SECOND].IsSkillReady())
        {
            if (gameManager.UseSkill(skillCooldowns[SkillType.SKILL_SECOND].skillByID, character,
                characterPos.SkillSecondPos.position))
            {
                skillCooldowns[SkillType.SKILL_SECOND].lastTimeUse = Time.time;
                animator.SetTrigger("useSkillSecond");
                return;
            }
        }
        if (skillCooldowns.Count >= SkillType.HIT && skillCooldowns[SkillType.HIT].IsSkillReady())
        {
            if (gameManager.UseSkill(skillCooldowns[SkillType.HIT].skillByID, character,
                characterPos.NormalHitPos.position))
            {
                skillCooldowns[SkillType.HIT].lastTimeUse = Time.time;
                animator.SetTrigger("onHit");
                return;
            }
        }
    }
    public virtual Transform GetNearestTarget(Transform transform, float rangeAttack)
    {
        target = null;
        Collider2D[] targetCollider = Physics2D.OverlapCircleAll(transform.position, rangeAttack, targetLayer);
        float minDis = float.PositiveInfinity;
        foreach (var warrior in targetCollider)
        {
            float distance = Vector3.Distance(transform.position, warrior.transform.position);
            if (distance < minDis && !warrior.GetComponent<Character>().GetIsDie())
            {
                minDis = distance;
                target = warrior.transform;
            }
        }
        return target;
    }
    public virtual bool IsTargetInAttackZone(Transform target, float attackZone)
    {
        if (target == null || target.GetComponent<Character>().GetIsDie() == true) return false;
        return Vector3.Distance(transform.position, target.position) <= attackZone;
    }
    public LayerMask GetTargetLayer()
    {
        return targetLayer;
    }
    public Transform GetTarget()
    {
        return target;
    }
    public void SetIsAttack(bool isAttack)
    {
        this.isAttack = isAttack;
    }
    public bool GetIsAttack()
    {
        return isAttack;
    }
    protected virtual void OnEnable() 
    {
        isAttack = true;
    }
    protected virtual void OnDisable() { }
}