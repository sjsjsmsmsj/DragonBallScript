using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
[System.Serializable]
public class GameData
{
    public DataPlayer dataPlayer;
    public List<int> warriorIDBattle = new List<int>();
    public List<DataWarrior> warriorPossessed = new List<DataWarrior>();
}
[System.Serializable]
public class DataPlayer
{
    public string playerName;
    public int gold;
    public int diamond;
    public int level;
    public DataPlayer(string playerName, int gold, int diamond, int level)
    {
        this.playerName = playerName;
        this.gold = gold;
        this.diamond = diamond;
        this.level = level;
    }
}
[System.Serializable]
public class DataWarrior
{
    public int id;
    public int maxPower;
    public int power;
    public int star;
    public float maxHP;
    public float maxMana;
    public float damage;
    public DataWarrior(int id, int maxPower, int power, int star, float maxHP, float maxMana, float damage)
    {
        this.id = id;
        this.maxPower = maxPower;
        this.power = power;
        this.star = star;
        this.maxHP = maxHP;
        this.maxMana = maxMana;
        this.damage = damage;
    }   
}
public class SaveGameManager : MonoBehaviour
{
    [SerializeField] WarriorManager warriorManager;
    [SerializeField] Player player;
    private string pathDataFile;
    private string fileName = "/GameData.json";
    private float timeAutoSaveGame = 5f;
    private float countTimeSaveGame;
    private void Awake()
    {
        pathDataFile = Application.persistentDataPath + fileName;
        LoadGame();
    }
    private void Update()
    {
        countTimeSaveGame += Time.deltaTime;
        if (countTimeSaveGame >= timeAutoSaveGame)
        {
            SaveGame();
            countTimeSaveGame = 0f;
        }
    }
    public void SaveGame()
    {
        if (warriorManager == null || player == null)
        {
            Debug.Log("SaveGameManager: Can't save game because warriorManager or Player is null");
            return;
        }
        GameData gameData = new GameData();
        gameData.dataPlayer = player.GetDataPlayer();
        gameData.warriorIDBattle = warriorManager.GetDataWarriorIDBattle();
        gameData.warriorPossessed = warriorManager.GetDataWarriorPossessed();
        string json = JsonUtility.ToJson(gameData,true);
        File.WriteAllText(pathDataFile, json);
    }
    public void LoadGame()
    {
        if (warriorManager == null || player == null)
        {
            Debug.Log("SaveGameManager: Can't load game because warriorManager or Player is null");
            return;
        }
        if (File.Exists(pathDataFile))
        {
            string json = File.ReadAllText(pathDataFile);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            foreach (var dataWarrior in gameData.warriorPossessed)
            {
                if (dataWarrior != null)
                {
                    warriorManager.AddWarriorPossess(dataWarrior);
                }
            }
            warriorManager.GetWarriorList().Clear();
            foreach (var warriorID in gameData.warriorIDBattle)
            {
                warriorManager.WarriorByIdBattle(warriorID);
            }
            player.SetUpPlayer(gameData.dataPlayer);
        }
        else //new game
        {
            warriorManager.AddWarriorPossess(CharacterByID.VEGETA);
            warriorManager.AddWarriorPossess(CharacterByID.RADITZ);
        }
    }
    public void ResetGame()
    {
        File.Delete(pathDataFile);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}