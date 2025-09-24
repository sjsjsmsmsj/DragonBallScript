using UnityEngine;

public class RecoveryEnergySkill : Skill
{
    [SerializeField] private float restonePercentHP = 0.5f;
    [SerializeField] private float restonePercentMana = 1f;
    protected override int id => 7;
    private GameManager gameManager;
    private GameObject energyEffect;
    Warrior ownerWarrior;
    protected override void Start()
    {
        lastTimeUse = Time.time;
        energyEffect = gameManager.CreateEffectAtPos(EffectManager.ENERGY, transform.position, lifeTime, owner.transform);
    }
    protected override void Update()
    {
        if (Time.time >= lastTimeUse + lifeTime)
        {
            DestroySkill();
            return;
        }
        EffectSkill();
    }
    public override void EffectSkill()
    {
        if(owner == null || owner.GetIsDie())
        {
            DestroySkill();
            if (energyEffect != null)
            {
                Destroy(energyEffect);
            }
            return;
        }
        owner.Healing(owner.GetMaxHP() * restonePercentHP / lifeTime * Time.deltaTime);
        ownerWarrior?.RecoveryMana(ownerWarrior.GetMaxMana() * restonePercentMana / lifeTime * Time.deltaTime);
    }
    public override void DestroySkill()
    {
        ownerWarrior?.SetMana(ownerWarrior.GetMaxMana());
        Destroy(gameObject);
    }
    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}