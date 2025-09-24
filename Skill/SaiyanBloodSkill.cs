using UnityEngine;
using System.Collections.Generic;
public class BuffDameWarriorID
{
    public int id;
    public float dameBounus;
}
public class SaiyanBloodSkill : Skill
{
    protected override int id => 5;
    [SerializeField] private float dameBonus = 0.6f;
    [SerializeField] private float restoneHP = 5f;
    private WarriorManager warriorManager;
    private GameManager gameManager;
    private List<BuffDameWarriorID> dameBonusList = new List<BuffDameWarriorID>();
    protected override void Start()
    {
        lastTimeUse = Time.time;
        EffectSkill();
    }
    protected override void Update()
    {
        if (Time.time >= lastTimeUse + lifeTime)
        {
            DestroySkill();
        }
        else
        {
            owner?.Healing(restoneHP * Time.deltaTime);
        }
    }
    public override void EffectSkill()
    {
        List<GameObject> warriorList = warriorManager.GetWarriorList();
        foreach (var warriorObject in warriorList)
        {
            Character warrior = warriorObject.GetComponent<Character>();
            if (warrior != null && !warrior.GetIsDie())
            {
                float damageWarrior = warrior.GetDamage();
                BuffDameWarriorID buffDameWarriorID = new BuffDameWarriorID();
                buffDameWarriorID.id = warrior.GetID();
                buffDameWarriorID.dameBounus = damageWarrior * dameBonus;
                dameBonusList.Add(buffDameWarriorID);
                warrior.SetDamage(damageWarrior * (1 + dameBonus));
                CharacterPos characterPos = warriorObject.GetComponent<CharacterPos>();
                if (characterPos != null)
                {
                    gameManager.CreateEffectAtPos(EffectManager.BUFFDAMAGE, characterPos.EnergyEffectPos.position,
                        lifeTime, warrior.transform);
                }
            }
        }
    }
    public override void DestroySkill()
    {
        foreach (var warriorBuffDame in dameBonusList)
        {
            GameObject warriorObject = warriorManager.FindWarriorInList(warriorBuffDame.id,
                warriorManager.GetWarriorList());
            Character warrior = warriorObject.GetComponent<Character>();
            if (warrior != null)
            {
                warrior.SetDamage(warrior.GetDamage() - warriorBuffDame.dameBounus);
            }
        }
        Destroy(gameObject);
    }
    public float GetDameBonus()
    {
        return dameBonus;
    }
    public void SetDameBonus(float dameBonus)
    {
        this.dameBonus = dameBonus;
    }
    public float GetRestoneHP()
    {
        return restoneHP;
    }
    public void SetRestoneHP(float restoneHP)
    {
        this.restoneHP = restoneHP;
    }
    public void SetWarriorManager(WarriorManager warriorManager)
    {
        this.warriorManager = warriorManager;
    }
    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}