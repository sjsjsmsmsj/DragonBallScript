using UnityEngine;

public class NormalHit : Skill
{
    protected override int id => 1;
    [SerializeField] private float rateDamage = 1f;
    private Character target;
    private Animator animator;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }
    protected override void Update()
    {
        base.Update();
        if (Time.time >= lastTimeUse + lifeTime || owner == null)
        {
            EffectSkill();
            DestroySkill();
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        lastTimeUse = Time.time;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        target = null;
        owner = null;
        lastTimeUse = -1;
    }
    public override void EffectSkill()
    {
        if (target == null || owner == null || target.GetIsDie() || owner.GetIsDie()) return;
        damage = owner.GetDamage() * (1 + rateDamage);
        target.TakeDamage(new DamageData(damage) , owner);
        target = null;
    }
    public override void DestroySkill()
    {
        animator.Play("NormalHit", 0, 0f);
        animator.Update(0f);
        gameObject.SetActive(false);
    }
    public void SetTarget(Character target)
    {
        this.target = target;
    }
    public Character GetTarget() { return target; }
}