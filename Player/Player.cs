using UnityEngine;
public class Player : MonoBehaviour
{
    private string playerName = "Player1412";
    private int gold = 0;
    private int diamond = 0;
    private int level = 1;
    public string GetplayerName()
    {
        return playerName;
    }
    public void SetplayerName(string value)
    {
        if(string.IsNullOrWhiteSpace(value) || value.Contains(" "))
        {
            Debug.Log("Ten khong hop le");
            return;
        }
        playerName = value;
    }
    public int GetGold()
    {
        return gold;
    }
    public void SetGold(int value)
    {
        if(value < 0) return;
        gold = value;
    }
    public int GetDiamond()
    {
        return diamond;
    }
    public void SetDiamond(int value)
    {
        if (value < 0) return;
        diamond = value;
    }
    public int GetLevel()
    {
        return level;
    }
    public void SetLevel(int value)
    {
        if (value < 0) return;
        level = value;
    }
    public DataPlayer GetDataPlayer()
    {
        return new DataPlayer(playerName, gold, diamond, level);
    }
    public void SetUpPlayer(string name, int gold, int diamond, int level)
    {
        SetplayerName(name);
        SetGold(gold);
        SetDiamond(diamond);
        SetLevel(level);
    }
    public void SetUpPlayer(DataPlayer dataPlayer)
    {
        SetplayerName(dataPlayer.playerName);
        SetGold(dataPlayer.gold);
        SetDiamond(dataPlayer.diamond);
        SetLevel(dataPlayer.level);
    }
}