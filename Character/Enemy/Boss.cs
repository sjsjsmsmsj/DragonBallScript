using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : Warrior
{
    [SerializeField] private List<Item> itemDrop = new List<Item>();
    private float maxTimeHealingFullHp = 15f;
    private float countTimeTakeDame;
    private bool isTakeDame;
    protected override void Start()
    {
        base.Start();
        isTakeDame = false;
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
            }
            if (isTakeDame)
            {
                countTimeTakeDame += Time.deltaTime;
                if (countTimeTakeDame >= maxTimeHealingFullHp)
                {
                    Healing(maxHP);
                    isTakeDame = false;
                    countTimeTakeDame = 0f;
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(-1.2f, 1.2f, 1f);
        }
    }
    public override void TakeDamage(DamageData damage, Character charAttack)
    {
        base.TakeDamage(damage, charAttack);
        isTakeDame = true;
        countTimeTakeDame = 0f;
    }
    public override void Die()
    {
        base.Die();
        int count = -itemDrop.Count / 2;
        float distance = 2f;
        foreach (Item item in itemDrop)
        {
            if (item != null)
            {
                Vector3 pos = new Vector3((float)count * distance + transform.position.x, transform.position.y, 0f);
                gameManager.CreateItemByID(item.GetID(), transform.position);
                count++;
            }
        }
    }
}