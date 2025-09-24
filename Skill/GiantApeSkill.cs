using UnityEngine;

public class GiantApeSkill : Skill
{
    protected override int id => 2;
    [SerializeField] private float dameBonus = 2.5f;
    [SerializeField] private float hpBonus = 2f;
    [SerializeField] private float timeTransform = 2f;
    [SerializeField] Animator giantApeHitAnimator;
    [SerializeField] private Animator giantAtomicAimator;
    [SerializeField] private float lifeTimeGiantApeHit = 0.9f;
    [SerializeField] private float timeChargingGiantApeHit = 0.2f;
    [SerializeField] private float sizeOfPowerEffect = 2f;
    private GameManager gameManager;
    private float timeNextTransform;
    private RuntimeAnimatorController ownerController;
    private Animator ownerAnimator;
    private Animator skillAnimator;
    private bool isCharged = false;

    private RuntimeAnimatorController normalHitController;
    private Animator normalHitAnimator;
    private float lifeTimeNormalHit;
    private float nextTimeDestroyGiantApeHit;
    private Animator atomicSkillAnimator;
    private RuntimeAnimatorController atomicSKillController;
    private float dameUp;
    private float hpUp;
    private CharacterAttack characterAttack;
    protected override void Awake()
    {
        base.Awake();
        skillAnimator = GetComponent<Animator>();
        skillAnimator.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
    protected override void Start()
    {
        base.Start();
        if (!owner) return;
        characterAttack = owner.GetComponent<CharacterAttack>();
        lastTimeUse = Time.time;
        ownerAnimator = owner.GetComponent<Animator>();
        ownerController = ownerAnimator?.runtimeAnimatorController;
        timeNextTransform = Time.time + timeTransform;
        gameManager?.CreateEffectAtPos(EffectManager.POWER, owner.characterPos.PowerEffectPos.position, 
            timeTransform, owner.transform);
        gameManager?.CreateEffectAtPos(EffectManager.ENERGY, owner.characterPos.EnergyEffectPos.position,
            timeTransform, owner.transform);
    }
    protected override void Update()
    {
        base.Update();
        if(owner == null)
        {
            Destroy(gameObject);
            return;
        }
        if (Time.time >= lastTimeUse + lifeTime || owner.GetIsDie())
        {
            DestroySkill();
        }
        if (Time.time >= nextTimeDestroyGiantApeHit)
        {
            HandleDestroyGiantApeHitSkill();
        }
        if (!isCharged && Time.time >= timeNextTransform)
        {   
            owner.SetIsActive(true);
            if(!characterAttack) characterAttack.SetIsAttack(true);
            EffectSkill();
            isCharged = true;
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Character.CharacterDie += HandleCharacterDie;
        SkillManager.CharUseNormalHitSkill += BecomeGiantApeHitSkill;
        SkillManager.CharUseAtomicSkill += BecomeGiantApeAtomic;
        AtomicSkill.DestroyAtomicSkill += HandleDestroyGiantApeAtomicSkill;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Character.CharacterDie -= HandleCharacterDie;
        SkillManager.CharUseNormalHitSkill -= BecomeGiantApeHitSkill;
        SkillManager.CharUseAtomicSkill -= BecomeGiantApeAtomic;
        AtomicSkill.DestroyAtomicSkill -= HandleDestroyGiantApeAtomicSkill;
    }
    public void HandleCharacterDie(Character character)
    {
        if(character == owner)
        {
            DestroySkill();
        }
    }
    private void BecomeGiantApeHitSkill(NormalHit normalHitSkill, Character characterUseSkill)
    {
        if (characterUseSkill != owner) return;
        nextTimeDestroyGiantApeHit = Time.time + lifeTimeGiantApeHit;
        normalHitAnimator = normalHitSkill.GetComponent<Animator>();
        normalHitController = normalHitAnimator?.runtimeAnimatorController;
        normalHitAnimator.runtimeAnimatorController = giantApeHitAnimator?.runtimeAnimatorController;
        lifeTimeNormalHit = normalHitSkill.GetLifeTime();
        normalHitSkill.SetlifeTime(lifeTimeGiantApeHit);
        GameObject powerEffect = gameManager?.CreateEffectAtPos(EffectManager.POWER,
            owner.characterPos.PowerEffectPos.position, timeChargingGiantApeHit, owner.transform);
        powerEffect.transform.localScale = powerEffect.transform.localScale * sizeOfPowerEffect;
    }
    private void BecomeGiantApeAtomic(AtomicSkill atomicSkill, Character characterUseSkill)
    {
        if (characterUseSkill != owner) return;
        atomicSkillAnimator = atomicSkill.GetComponent<Animator>();
        atomicSKillController = atomicSkillAnimator?.runtimeAnimatorController;
        atomicSkillAnimator.runtimeAnimatorController = giantAtomicAimator.runtimeAnimatorController;
    }
    public void HandleDestroyGiantApeHitSkill()
    {
        if (normalHitAnimator != null)
        {
            normalHitAnimator.runtimeAnimatorController = normalHitController;
            NormalHit normalHit = normalHitAnimator.GetComponent<NormalHit>();
            normalHit?.SetlifeTime(lifeTimeNormalHit);
            normalHitAnimator = null;
            normalHitController = null;
        }
    }
    public void HandleDestroyGiantApeAtomicSkill(AtomicSkill atomicSkill)
    {
        if(atomicSkillAnimator != null && atomicSkillAnimator.GetComponent<AtomicSkill>() == atomicSkill)
        {
            atomicSkillAnimator.runtimeAnimatorController = atomicSKillController;
            atomicSKillController = null;
            atomicSKillController = null;
        }
    }
    public override void EffectSkill()
    {
        ownerAnimator.runtimeAnimatorController = skillAnimator?.runtimeAnimatorController;
        dameUp = dameBonus * owner.GetDamage();
        hpUp = hpBonus * owner.GetMaxHP();
        owner.SetMaxHP(owner.GetMaxHP() + hpUp);
        owner.Healing(hpUp);
        owner.SetDamage(owner.GetDamage() + dameUp);
    }
    public override void DestroySkill()
    {
        HandleDestroyGiantApeHitSkill();
        ownerAnimator.runtimeAnimatorController = ownerController;
        owner.SetMaxHP(owner.GetMaxHP() - hpUp);
        owner.SetHP(Mathf.Min(owner.GetHP(), owner.GetMaxHP()));
        owner.SetDamage(owner.GetDamage() - dameUp);
        Destroy(gameObject);
    }
    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
}