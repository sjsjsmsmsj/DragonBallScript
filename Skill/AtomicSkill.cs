using System;
using System.Collections.Generic;
using UnityEngine;

public class AtomicSkill : Skill
{
    protected override int id => 3; 
    [SerializeField] private float rateDamage = 1.5f;
    [SerializeField] private float speedMove = 20f;
    [SerializeField] private float damageZone = 5f;
    [SerializeField] private float timeCharg = 0.3f;
    private Vector3 target;
    private Animator animator;
    private bool isExplosion = false;
    private LayerMask targetLayer;
    private float timeFinishCharg;
    private CharacterAttack characterAttack;
    private CharacterMove characterMove;
    private bool isMove = false;
    private bool isTakeDame = false;
    public static event Action<AtomicSkill> DestroyAtomicSkill;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        if(owner != null) {
            damage = owner.GetDamage() * (1 + rateDamage);
            characterAttack = owner.GetComponent<CharacterAttack>();
            characterMove = owner.GetComponent<CharacterMove>();
            targetLayer = characterAttack.GetTargetLayer();
            characterMove.isMove = false;
            timeFinishCharg = Time.time + timeCharg;
        }
    }
    protected override void Update()
    {
        if (Time.time >= timeFinishCharg && !isMove)
        {
            characterMove.isMove = true;
            isMove = true;
        }
        if (!isExplosion && isMove)
        {
            transform.position =  Vector3.MoveTowards(transform.position, target, speedMove * Time.deltaTime);
            if(Vector3.Distance(transform.position, target) <= 0.001f)
            {
                isExplosion = true;
                animator.SetBool("isExplosion", true);
                transform.rotation = Quaternion.identity;
                lastTimeUse = Time.time;
            }
        }
        if(isExplosion)
        {
            if (Time.time >= lastTimeUse + lifeTime)
            {
                DestroySkill();
            }
            if (Time.time >= lastTimeUse + lifeTime / 2 && !isTakeDame)
            {
                EffectSkill();
                isTakeDame = true;
            }
        }
    }
    public override void EffectSkill()
    {
        if(owner == null) return;
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, damageZone, targetLayer);
        foreach (var target in collider2Ds)
        {
            Character character = target.GetComponent<Character>();
            if (character != null)
            {
                character.TakeDamage(new DamageData(damage), owner);
            }
        }
    }
    public override void DestroySkill()
    {
        DestroyAtomicSkill?.Invoke(this);
        Destroy(gameObject);
    }
    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }
    public Vector3 GetTarget()
    {
        return this.target;
    }
    public float GetTimeCharg()
    {
        return timeCharg;
    }
    public void SetSpeedMove(float speedMove)
    {
        this.speedMove = speedMove;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, damageZone);
    }
}