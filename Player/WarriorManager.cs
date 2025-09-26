using UnityEngine;
using System.Collections.Generic;
using System;
public class WarriorManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private List<GameObject> warriorRefapsList =  new List<GameObject>();
    [SerializeField] private List<GameObject> warriorPossessList = new List<GameObject>(); // danh sach tuong da so huu
    [SerializeField] private List<GameObject> warriorList = new List<GameObject>(); //Tuong dang xuat tran
    private int maxWarriorBattle = 5;
    public int warriorLayer { get; private set; }
    public bool isIgnoreCol { set; get; }
    private void Awake()
    {
        warriorLayer = LayerMask.NameToLayer("Warrior");
    }
    public GameObject CreateWarriorByID(int id, Vector3 pos)
    {
        GameObject warriorRefab = FindWarriorInList(id, warriorRefapsList);
        GameObject warriorInit = Instantiate(warriorRefab, pos, Quaternion.identity);
        return warriorInit;
    }
    public GameObject AddWarriorPossess(int id)
    {
        if (IsExistWarriorInList(id, warriorPossessList))
        {
            Debug.Log("Warrior has possessed in warriorListProssess");
            return null;
        }
        GameObject warriorInit = CreateWarriorByID(id, transform.position);
        Warrior warriorScript = warriorInit.GetComponent<Warrior>();
        if (warriorScript == null) return null;
        warriorScript.gameManager = gameManager;
        warriorScript.SetPower(gameManager.GetWarriorRankValue(warriorScript.GetRank()).power);
        CharacterPos warriorPos = warriorInit.GetComponent<CharacterPos>();
        if (warriorPos != null)
        {
            warriorPos.RevivePos = transform;
        }
        warriorInit.SetActive(false);
        warriorPossessList.Add(warriorInit);
        UpWarriorStar(id);
        if (warriorList.Count < maxWarriorBattle)
        {
            WarriorByIdBattle(warriorScript.GetID());
        }
        return warriorInit;
    }
    public GameObject AddWarriorPossess(DataWarrior dataWarrior)
    {
        if (IsExistWarriorInList(dataWarrior.id, warriorPossessList))
        {
            Debug.Log("Warrior has possessed in warriorListProssess");
            return null;
        }
        GameObject warriorInit = CreateWarriorByID(dataWarrior.id, transform.position);
        Warrior warriorScript = warriorInit.GetComponent<Warrior>();
        if (warriorScript != null)
        {
            warriorScript.gameManager = gameManager;
            warriorScript.SetStar(dataWarrior.star);
            warriorScript.SetMaxHP(dataWarrior.maxHP);
            warriorScript.SetMaxMana(dataWarrior.maxMana);
            warriorScript.SetDamage(dataWarrior.damage);
            warriorScript.SetPower(dataWarrior.power);
            warriorScript.SetMaxPower(dataWarrior.maxPower);
        }
        CharacterPos warriorPos = warriorInit.GetComponent<CharacterPos>();
        if (warriorPos != null)
        {
            warriorPos.RevivePos = transform;
        }
        warriorInit.SetActive(false);
        warriorPossessList.Add(warriorInit);
        if (warriorList.Count < maxWarriorBattle)
        {
            WarriorByIdBattle(warriorScript.GetID());
        }
        return warriorInit;
    }
    public GameObject WarriorByIdBattle(int id)
    {
        if (IsExistWarriorInList(id, warriorList) || warriorList.Count >= maxWarriorBattle) return null;
        GameObject warrior = FindWarriorInList(id, warriorPossessList);
        if (warrior == null)
        {
            Debug.Log("Warrior by id: " + id.ToString() + " not exist !");
            return null;
        }
        warrior.SetActive(true);
        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);
        warrior.transform.position = position;
        warriorList.Add(warrior);
        return warrior;
    }
    public GameObject FindWarriorInList(int id, List<GameObject> warriorList)
    {
        foreach (var warrior in warriorList)
        {
            Warrior warriorScript = warrior.GetComponent<Warrior>();
            if (warriorScript != null && warriorScript.GetID() == id)
            {
                return warrior;
            }
        }
        return null;
    }
    public bool IsExistWarriorInList(int id, List<GameObject> warriorList)
    {
        foreach (var item in warriorList)
        {
            Warrior warrior = item.GetComponent<Warrior>();
            if(warrior != null && warrior.GetID() == id)
            {
                return true;
            }
        }
        return false;
    }
    public void UpWarriorStar(int id)
    {
        GameObject warriorObj = FindWarriorInList(id, warriorPossessList);
        Warrior warrior = warriorObj?.GetComponent<Warrior>();
        warrior?.SetStar(warrior.GetStar() + 1);
    }
    public List<GameObject> GetWarriorList()
    {
        return warriorList;
    }
    public List<GameObject> GetWarriorRefapList()
    {
        return warriorRefapsList;
    }
    public List<GameObject> GetWarriorPossessList()
    {
        return warriorPossessList;
    }
    public void SetMaxWarriorBattle(int maxWarriorBattle)
    {
        if (maxWarriorBattle <= 0) return;
        this.maxWarriorBattle = maxWarriorBattle;
    }
    public int GetmaxWarriorBattle()
    {
        return maxWarriorBattle;
    }
    public List<DataWarrior> GetDataWarriorPossessed()
    {
        List<DataWarrior> dataWarriorList = new List<DataWarrior>();
        foreach (var warriorObject in warriorPossessList)
        {
            Warrior warrior = warriorObject.GetComponent<Warrior>();
            if (warrior != null)
            {
                dataWarriorList.Add(warrior.GetDataWarrior());
            }
        }

        return dataWarriorList;
    }
    public List<int> GetDataWarriorIDBattle()
    {
        List<int> warriorID = new List<int>();
        foreach (var warriorObject in warriorList)
        {
            Warrior warrior = warriorObject.GetComponent<Warrior>();
            if(warrior != null)
            {
                warriorID.Add(warrior.GetID());
            }
        }
        return warriorID;
    }
    public void SetIgnoreCol(bool isIgnore)
    {
        isIgnoreCol = isIgnore;
        Physics2D.IgnoreLayerCollision(warriorLayer, warriorLayer, isIgnore);
    }
}
