using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject damageTextRefap;
    [SerializeField] private GameObject expTextRefap;
    private List<CharacterWaitRevive> characterWaitRevives = new List<CharacterWaitRevive>();
    private SkillManager skillManager;
    private ItemManager itemManager;
    private EffectManager effectManager;
    private WarriorRankManager warriorRankManager = new WarriorRankManager();
    public WarriorManager warriorManager;
    public bool isPauseGame { set; get; }
    public Camera cam;
    private void Start()
    {
        skillManager = GetComponent<SkillManager>();
        itemManager = GetComponent<ItemManager>();
        effectManager = GetComponent<EffectManager>();
    }
    private void Update()
    {
        characterWaitRevives.RemoveAll(x => x.isRevive == true);
        foreach (var item in characterWaitRevives)
        {
            item.UpdateTimeRevive();
        }
    }
    public void ReviveCharacter(Character character, Vector3 posRevive)
    {
        if (!character.GetIsDie()) return;
        character.SetIsDie(false);
        character.Healing(character.GetMaxHP());
        character.transform.position = posRevive;
        character.GetHpBar().SetActive(false);
        character.GetComponent<Collider2D>().enabled = true;
        character.SetIsActive(true);
    }
    public void ReviveCharacterAfterTime(Character character, float timeRevive)
    {
        CharacterWaitRevive characterWaitRevive = new CharacterWaitRevive(character, timeRevive, this);
        characterWaitRevives.Add(characterWaitRevive);
    }
    public Skill UseSkill(int skillID, Character owner, Vector3 pos)
    {
        if (owner == null || owner.GetIsDie() == true)
        {
            Debug.Log("Owner == null");
            return null;
        }
        Skill skill = skillManager.FindSkillRefabsByID(skillID);
        CharacterAttack characterAttack = owner.GetComponent<CharacterAttack>();
        Character target = owner.GetTarget();
        if(skill == null || !characterAttack.IsTargetInAttackZone(target.transform, skill.GetRangeAttack())) 
            return null;
        if(skill.isUlti)
        {
            Warrior warrior = owner as Warrior;
            if(warrior != null)
            {
                if(warrior.isFullMana()) warrior.ResetMana();
                else return null;
            }
        }
        if (skillID == SkillManager.DAM_GALICK)
        {
            return skillManager.UseNormalHitSkill(pos, owner, target);
        }
        else if (skillID == SkillManager.HOA_KHI_DOT_KHONG_LO)
        {
            return skillManager.UseGiantApeSkill(owner);
        }
        else if (skillID == SkillManager.ATOMIC)
        {
            return skillManager.UseAtomicSkill(pos, owner, target);
        }
        else if (skillID == SkillManager.PHAO_RADITZ)
        {
            return skillManager.UseRaditzBomSkill(pos, owner, target);
        }
        else if (skillID == SkillManager.DONG_MAU_SAIYAN)
        {
            return skillManager.UseSaiyanBloodSkill(owner);
        }
        else if (skillID == SkillManager.CONG_KICH_TOAN_LUC) 
        { 
            return skillManager.UseFullAttackSkill(pos, owner, target);
        }
        else if (skillID == SkillManager.TAI_TAO_NANG_LUONG)
        {
            return skillManager.UseRecoveryEnergySkill(pos, owner);
        }
        else if (skillID == SkillManager.BODY_STRENGTHEN)
        {
            return skillManager.UseBodyStrengthenSkill(owner);
        }
        else
        {
            return null;
        }
    }
    public GameObject CreateEffectAtPos(int iD, Vector3 pos, float timeLife)
    {
        if (iD == EffectManager.POWER)
        {
            return effectManager.InitPowerEffectObject(pos, timeLife);

        }
        else if (iD == EffectManager.ENERGY)
        {
            return effectManager.InitEnergyEffectObject(pos, timeLife);
        }
        else if (iD == EffectManager.CLICK)
        {
            return effectManager.InitClickEffectObject(pos, timeLife);
        }
        else if (iD == EffectManager.BUFFDAMAGE)
        {
            return effectManager.InitBuffDameEffect(pos, timeLife);
        }
        else if (iD == EffectManager.BLOOD)
        {
            return effectManager.InitBloodEffect(pos, timeLife);
        }
        else
        {
            return null;
        }
    }
    public GameObject CreateEffectAtPos(int iD, Vector3 pos, float timeLife, Transform owner)
    {
        if (iD == EffectManager.POWER)
        {
            GameObject gameObject = effectManager.InitPowerEffectObject(pos, timeLife);
            gameObject?.transform.SetParent(owner);
            return gameObject;

        }
        else if (iD == EffectManager.ENERGY)
        {
            GameObject gameObject = effectManager.InitEnergyEffectObject(pos, timeLife);
            gameObject?.transform.SetParent(owner);
            return gameObject;
        }
        else if (iD == EffectManager.BUFFDAMAGE)
        {
            GameObject gameObject = effectManager.InitBuffDameEffect(pos, timeLife);
            gameObject?.transform.SetParent(owner);
            return gameObject;
        }
        else if (iD == EffectManager.BLOOD)
        {
            GameObject gameObject = effectManager.InitBloodEffect(pos, timeLife);
            gameObject?.transform.SetParent(owner);
            return gameObject;
        }
        else
        {
            return null;
        }
    }
    public GameObject CreateItemByID(int id, Vector3 pos)
    {
        GameObject itemRefab = itemManager.FindItemPrefaps(id);
        if (itemRefab == null)
        {
            Debug.Log("Not exist this item in prefabs list");
            return null;
        }
        GameObject itemInit = Instantiate(itemRefab, pos, Quaternion.identity);
        Item itemScript = itemInit.GetComponent<Item>();
        if (itemScript == null) return null;
        itemScript.SetGameManager(this);
        if(itemScript.GetID() == ItemByID.NAPPA_SOUL)
        {
            NappaSoul nappaSoul = itemScript as NappaSoul;
            nappaSoul.SetWarriorManager(warriorManager);
        }
        return itemInit;
    }
    public GameObject AddNewWarriorPossess(int id)
    {
        if (warriorManager == null)
        {
            Debug.Log("Warrior Manager in class GameManager is null !");
            return null;
        }
        return warriorManager.AddWarriorPossess(id);
    }
    public void WarriorPowerUp(int exp)
    {
        List<GameObject> warriorList = warriorManager.GetWarriorList();
        foreach (var warriorObject in warriorList)
        {
            Warrior warrior = warriorObject.GetComponent<Warrior>();
            if (warrior != null || !warrior.GetIsDie())
            {
                warrior.PowerUp(exp);
            }
        }
    }
    public void AddItemForPlayer(int id, int quantity)
    {
        if(player == null)
        {
            Debug.Log("Class Player is null in GameManager !");
            return;
        }
        if(id == ItemByID.GOLD)
        {
            player.SetGold(player.GetGold() + quantity);
        }
        else if (id == ItemByID.DIAMOND)
        {
            player.SetDiamond(player.GetDiamond() + quantity);
        }
    }
    public void CreateDamageText(Vector3 pos, float damage)
    {
        if (damageTextRefap == null) return;
        GameObject damageTextObj = Instantiate(damageTextRefap, pos, Quaternion.identity);
        damageTextObj.GetComponent<DameTextPopUp>().SetUpTextPopUp(damage);
    }
    public void CreateExpText(Vector3 pos, float exp)
    {
        if (expTextRefap == null) return;
        GameObject expTextObj = Instantiate(expTextRefap, pos, Quaternion.identity);
        expTextObj.GetComponent<DameTextPopUp>().SetUpTextPopUp(exp);
    }
    public WarriorRankValue GetWarriorRankValue(WarriorRank rank)
    {
        if (rank == WarriorRank.Red)
        {
            return new WarriorRankValue(WarriorRank.Red, warriorRankManager.redBg, warriorRankManager.powerRed);
        }
        else if (rank == WarriorRank.Gold)
        {
            return new WarriorRankValue(WarriorRank.Gold, warriorRankManager.goldBg, warriorRankManager.powerGold);
        }
        else
        {
            return new WarriorRankValue(WarriorRank.Blue, warriorRankManager.blueBg, warriorRankManager.powerBlue);
        }
    }
}
public class CharacterWaitRevive
{
    private Character character;
    private float nextTimeRevive;
    private GameManager gameManager;
    public bool isRevive {get; private set;}
    public CharacterWaitRevive(Character character, float timeRevive, GameManager gameManager)
    {
        this.character = character;
        this.gameManager = gameManager;
        nextTimeRevive = Time.time + timeRevive;
        isRevive = false;
    }
    public void UpdateTimeRevive()
    {
        if (isRevive) return;
        if (Time.time >= nextTimeRevive)
        {
            CharacterPos characterPos = character.GetComponent<CharacterPos>();
            Vector3 posRevive;
            if (characterPos != null)
                posRevive = new Vector3(characterPos.RevivePos.position.x, characterPos.RevivePos.position.y);
            else
                posRevive = new Vector3(character.transform.position.x, character.transform.position.y);
            gameManager.ReviveCharacter(character, posRevive);
            isRevive = true;
        }
    }
    
}