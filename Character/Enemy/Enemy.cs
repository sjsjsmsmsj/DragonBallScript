using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private int exp = 1;
    [SerializeField] private int gold = 10;
    [SerializeField] private float manaRecovery = 20f;
    protected override void Start()
    {
        base.Start();
        GameObject reviveObj = new GameObject("RevivePos");
        reviveObj.transform.position = new Vector3(transform.position.x, transform.position.y);
        characterPos.RevivePos = reviveObj.transform;
    }
    public int GetExp()
    {
        return exp;
    }
    public void SetExp(int exp)
    {
        this.exp = exp;
    }
    public int Getgold()
    {
        return gold;
    }
    public void Setgold(int gold)
    {
        this.gold = gold;
    }
    public float GetManaRecovery()
    {
        return manaRecovery;
    }
    public void SetManaRecovery(float manaRecovery)
    {
        this.manaRecovery = manaRecovery;
    }
    public override void TakeDamage(DamageData damageData, Character charAttack)
    {
        hp = Mathf.Max(hp - damageData.damage, 0);
        UpdateHPFill();
        gameManager.CreateEffectAtPos(EffectManager.BLOOD, transform.position, 0.333334f, transform);
        gameManager.CreateDamageText(characterPos.DamageTextPos.position, damageData.damage);
        if ((int)hp == 0)
        {
            Warrior warrior = charAttack as Warrior;
            if (warrior != null)
            {
                warrior.RecoveryMana(manaRecovery);
                warrior.PowerUp(exp);
                gameManager.WarriorPowerUp(exp);
                gameManager.AddItemForPlayer(ItemByID.GOLD, gold);
            }
            Die();
        }
        animator.SetTrigger("takeDame");
    }
}
