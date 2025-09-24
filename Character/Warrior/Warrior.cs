using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public enum WarriorRank
{
    Blue, Gold, Red
}
public class WarriorRankValue
{
    public WarriorRank rank;
    public Color bg;
    public int power;
    public WarriorRankValue(WarriorRank rank, Color bg, int power)
    {
        this.rank = rank;
        this.bg = bg;
        this.power = power;
    }
}
public class WarriorRankManager
{
    public Color blueBg = new Color(0, 1, 1, 1);
    public Color goldBg = Color.yellow;
    public Color redBg = Color.red;
    public int powerBlue = 600;
    public int powerGold = 1200;
    public int powerRed = 2400;
    public static int CompareRank(WarriorRank w1, WarriorRank w2)
    {
        int rank1 = GetRankIndex(w1);
        int rank2 = GetRankIndex(w2);
        return rank1 - rank2;
    }
    public static int GetRankIndex(WarriorRank rank)
    {
        if(rank == WarriorRank.Red) return 2;
        else if(rank == WarriorRank.Gold) return 1;
        else return 0;
    }
}
public class Warrior : Character
{
    [SerializeField] protected float maxMana = 100f;
    [SerializeField] protected Image manaFill;
    [SerializeField] protected int maxPower = 1000000;
    [SerializeField] protected int power = 1200;
    [SerializeField] protected GameObject manaBar;
    [SerializeField] protected float manaRecoverySpeed = 2f;
    [SerializeField] protected List<Sprite> warriorSprites = new List<Sprite>();
    [SerializeField] protected WarriorRank rank;
    protected float hpRecoverySpeed = 0.25f;
    protected int star;
    protected float hpOrigin;
    protected float damageOrigin;
    protected float mana = 0f;
    protected float sec = 1f;
    protected float timePerSecond;
    protected float scaleHp = 0.01f;
    protected float scaleDamage = 0.005f;
    protected float hpPowerScale;
    protected float damagePowerScale;
    protected override void Start()
    {
        hpOrigin = maxHP;
        damageOrigin = damage;
        hpPowerScale = (float)power * scaleHp;
        damagePowerScale = (float)power * scaleDamage;
        maxHP = maxHP + hpPowerScale;
        damage = damage + damagePowerScale;
        base.Start();
        UpdateManaFill();
        manaBar.SetActive(false);
    }
    protected override void Update()
    {
        manaBar.SetActive(hpBar.activeSelf);
        if (!isDie && !gameManager.isPauseGame)
        {
            timePerSecond += Time.deltaTime;
            if (timePerSecond >= sec)
            {
                timePerSecond = 0f;
                RecoveryMana(manaRecoverySpeed);
                Healing(hpRecoverySpeed);
            }
        }
    }
    public void TakeMana(float manaUse)
    {
        mana = Mathf.Max(mana - manaUse, 0);
        UpdateManaFill();
    }
    public void RecoveryMana(float manaRecovery)
    {
        if (isDie || gameManager.isPauseGame) return;
        mana = Mathf.Min(mana + manaRecovery, maxMana);
        UpdateManaFill();
    }
    public void UpdateManaFill()
    {
        if (!manaFill)
        {
            Debug.Log("ManaFill is null !");
            return;
        }
        manaFill.fillAmount = mana / maxMana;
    }
    public void PowerUp(int exp)
    {
        if (exp <= 0) {
            Debug.Log("Exp must greater than 0");
            return;
        }
        power = Mathf.Min(power + exp, maxPower);
        maxHP = maxHP - hpPowerScale;
        damage = damage - damagePowerScale;
        hpPowerScale = power * scaleHp;
        damagePowerScale = power * scaleDamage;
        maxHP = maxHP + hpPowerScale;
        damage = damage + damagePowerScale;
        gameManager.CreateExpText(characterPos.DamageTextPos.position, exp);
    }
    public bool isFullMana()
    {
        return (int)mana == (int)maxMana;
    }
    public int GetCurrentPower()
    {
        return power;
    }
    public void SetPower(int power)
    {
        this.power = power;
    }
    public void ResetMana()
    {
        mana = 0f;
    }
    public override void SetIsDie(bool isDie)
    {
        base.SetIsDie(isDie);
        if (isDie)
        {
            SetIsActive(false);
            mana = 0;
        }
        manaBar.SetActive(!isDie);
    }
    public void SetMana(float mana)
    {
        if (mana < 0) return;
        this.mana = mana;
    }
    public float GetMana()
    {
        return mana;
    }
    public void SetMaxMana(float maxMana)
    {
        this.maxMana = maxMana;
    }
    public float GetMaxMana()
    {
        return maxMana;
    }
    public void SetMaxPower(int maxPower)
    {
        this.maxPower = maxPower;
    }
    public int GetMaxPower()
    {
        return maxPower;
    }
    public float GetHpOrigin()
    {
        return hpOrigin;
    }
    public float GetDamageOrigin()
    {
        return damageOrigin;
    }
    public DataWarrior GetDataWarrior()
    {
        return new DataWarrior(GetID(), maxPower, power, star, hpOrigin, maxMana, damageOrigin);
    }
    public int GetStar()
    {
        return star;
    }
    public void SetStar(int star)
    {
        if(star < 0) star = 0;
        this.star = star;
    }
    public void SetWarriorSprite(List<Sprite> warriorSprites)
    {
        this.warriorSprites = warriorSprites;
    }
    public List<Sprite> GetWarriorSprites()
    {
        return warriorSprites;
    }
    public WarriorRank GetRank()
    {
        return rank;
    }
}
