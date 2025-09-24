using UnityEngine;

public abstract class Skill : MonoBehaviour
{

    protected virtual int id => 0;
    [SerializeField] protected float cooldown;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected float rangeAttack;
    protected float damage;
    protected float lastTimeUse;
    protected Character owner;
    public bool isUlti;
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void OnDestroy() { }
    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    public abstract void EffectSkill();
    public abstract void DestroySkill();
    public void SetOwner(Character owner)
    {
        this.owner = owner;
    }
    public void SetDamage(float damage)
    {
        if (damage < 0) return ;
        this.damage = damage;
    }
    public Character GetOwner()
    {
        return owner;
    }
    public float GetDamage()
    {
        return damage;
    }
    public float GetCoolDown()
    {
        return cooldown;
    }
    public int GetSkillID()
    {
        return id;
    }
    public void SetlifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
    }
    public float GetLifeTime()
    {
        return lifeTime;
    }
    public float GetRangeAttack()
    {
        return rangeAttack;
    }
    public void SetRangeAttack(float rangeAttack)
    {
        if (rangeAttack < 0) return;
        this.rangeAttack = rangeAttack;
    }
}
