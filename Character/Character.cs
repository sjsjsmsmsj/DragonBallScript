using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
public enum Status
{
    Attack, BackHome, Moving, Relax, Protected
}
public static class CharacterByID
{
    //warrior
    public const int VEGETA = 1;
    public const int RADITZ = 2;
    public const int NAPPA = 3;
    //enemy
    public const int NAPPA_BOSS = 100;
    public const int SAIBAMEN = 101;
}
public class DamageData
{
    public float damage;
    public DamageData(float damage)
    {
        this.damage = damage;
    }
}
public abstract class Character : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] protected float maxHP = 100;
    [SerializeField] protected float damage;
    [SerializeField] protected Image hpFill;
    [SerializeField] protected float speedMove = 15f;
    [SerializeField] protected List<Skill> skillPrefaps = new List<Skill>();
    [SerializeField] protected float rangeHit = 0f;
    [SerializeField] protected float timeRevive;
    [SerializeField] protected GameObject hpBar;
    protected float hp;
    protected CharacterMove characterMove;
    protected bool isDie = false;
    protected Animator animator;
    protected CharacterAttack characterAttack;
    public GameManager gameManager;
    public CharacterPos characterPos { get; protected set; }
    public static event Action<Character> CharacterDie;
    public static event Action<Character, DamageData> CharTakeDame;
    protected virtual void Start()
    {
        hp = maxHP;
        UpdateHPFill();
        characterMove = GetComponent<CharacterMove>();
        if (gameManager != null) characterMove.gameManager = gameManager;
        animator = GetComponent<Animator>();
        characterPos = GetComponent<CharacterPos>();
        characterAttack = GetComponent<CharacterAttack>();
        hpBar.SetActive(false);
    }
    protected virtual void Update() { }
    public virtual void TakeDamage(DamageData damageData, Character charAttack)
    {
        CharTakeDame?.Invoke(this, damageData);
        hp = Mathf.Max(hp - damageData.damage, 0);
        UpdateHPFill();
        gameManager.CreateEffectAtPos(EffectManager.BLOOD, transform.position, 0.3334f, transform);
        gameManager.CreateDamageText(characterPos.DamageTextPos.position, damageData.damage);
        if ((int)hp == 0)
        {
            Die();
        }
        animator.SetTrigger("takeDame");
    }
    public void Healing(float hpHeal)
    {
        if (isDie) return;
        hp = Mathf.Min(hp + hpHeal, maxHP);
        UpdateHPFill();
    }
    public void UpdateHPFill()
    {
        if (!hpFill)
        {
            Debug.Log("HPFill is null !");
            return;
        }
        hpBar.SetActive(true);
        hpFill.fillAmount = hp / maxHP;
    }
    public virtual void Die()
    {
        CharacterDie?.Invoke(this);
        SetIsDie(true);
        SetIsActive(false);
        gameManager.ReviveCharacterAfterTime(this, timeRevive);
    }
    public virtual void SetIsActive(bool isActive)
    {
        if (characterAttack == null || characterMove == null) return;
        characterMove.isMove = isActive;
        characterAttack.SetIsAttack(isActive);
    }
    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    public float GetRangeHit()
    {
        return rangeHit;
    }
    public virtual void SetDamage(float damage)
    {
        if (damage > 0)
            this.damage = damage;
    }
    public virtual float GetDamage()
    {
        return damage;
    }
    public float GetSpeedMove()
    {
        return speedMove;
    }
    public void SetSpeedMove(float speedMove)
    {
        if (speedMove >= 0)
            this.speedMove = speedMove;
    }
    public float GetTimeRevive()
    {
        return timeRevive;
    }
    public void SetTimeRevive(float timeRevive)
    {
        if (timeRevive >= 0)
            this.timeRevive = timeRevive;
    }
    public virtual void SetIsDie(bool isDie)
    {
        this.isDie = isDie;
        animator.SetBool("isDie", isDie);
        hpBar.SetActive(!isDie);
        GetComponent<Collider2D>().enabled = !isDie;
    }
    public bool GetIsDie()
    {
        return isDie;
    }
    public virtual void SetHP(float hp)
    {
        if (hp <= 0) return;
        this.hp = hp;
    }
    public virtual float GetHP()
    {
        return hp;
    }
    public virtual void SetMaxHP(float maxHP)
    {
        if (maxHP <= 0) return;
        this.maxHP = maxHP;
    }
    public virtual float GetMaxHP()
    {
        return maxHP;
    }
    public int GetID()
    {
        return id;
    }
    public GameObject GetHpBar()
    {
        return hpBar;
    }
    public List<Skill> GetSkillPrefaps()
    {
        return skillPrefaps;
    }
    public Character GetTarget()
    {
        if (characterAttack == null) return null;
        return characterAttack.GetTarget().GetComponent<Character>();
    }
}
