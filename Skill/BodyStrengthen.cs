using UnityEngine;

public class BodyStrengthen : Skill
{
    protected override int id => 8;
    public float damageReduce;
    protected override void Start()
    {
        base.Start();
        DestroySkill();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Character.CharTakeDame += ReduceTakeDamage;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Character.CharTakeDame -= ReduceTakeDamage;
    }
    public void ReduceTakeDamage(Character character, DamageData damageData)
    {
        if (character == owner)
        {
            damageData.damage -= damageData.damage * damageReduce;
        }
    }
    public override void EffectSkill()
    {
        throw new System.NotImplementedException();
    }
    public override void DestroySkill()
    {
        Destroy(gameObject, lifeTime);
    }
}