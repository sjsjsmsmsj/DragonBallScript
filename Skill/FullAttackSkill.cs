using UnityEngine;

public class FullAttackSkill : Skill
{
    protected override int id => 6;
    [SerializeField] private float rateDamage = 1f;
    private Character target;
    protected override void Start()
    {
        base.Start();
        if (owner == null || owner.GetIsDie())
        {
            DestroySkill();
            return;
        }
        damage = owner.GetDamage() * (1 + rateDamage);
        lastTimeUse = Time.time;
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
    public override void EffectSkill()
    {
        if (target == null || owner == null || target.GetIsDie() || owner.GetIsDie()) return;
        target.TakeDamage(new DamageData(damage), owner);
        target = null;
    }
    public override void DestroySkill()
    {
        Destroy(gameObject);
    }
    public void SetTarget(Character target)
    {
        this.target = target;
    }
    public Character GetTarget() 
    { 
        return target; 
    }
}