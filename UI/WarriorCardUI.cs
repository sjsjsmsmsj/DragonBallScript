using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Diagnostics;
public class WarriorCardUI : MonoBehaviour
{
    private int id;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private Image warriorImageUI;
    [SerializeField] private List<Image> startList = new List<Image>();
    [SerializeField] private float timePerFrame = 0.15f;
    private float countTime = 0f;
    private int currentFrame = 0;
    public WarriorRank rank {get; private set;}
    private string power_str = "Sức mạnh: ";
    private string notPossess_str = "Chưa sở hữu";
    private List<Sprite> warriorSpriteList;
    private Stopwatch Stopwatch = new Stopwatch();
    private float lastElapsed;
    private void OnEnable()
    {
        Stopwatch.Restart();
        currentFrame = 0;
        countTime = 0f;
        lastElapsed = 0f;
        if (warriorSpriteList != null)
        {
            warriorImageUI.sprite = warriorSpriteList[currentFrame];
        }
    }
    private void OnDisable()
    {
        Stopwatch.Stop();
    }
    private void Update()
    {
        if (warriorSpriteList == null)
        {
            UnityEngine.Debug.Log("WarriorSpriteList is null in WarriorCardUI class ");
            return;
        }
        float timeNow = (float)Stopwatch.Elapsed.TotalSeconds;
        float deltaTime = timeNow - lastElapsed;
        lastElapsed = timeNow;
        countTime += deltaTime;
        if (countTime >= timePerFrame)
        {
            currentFrame = (currentFrame + 1) % warriorSpriteList.Count;
            warriorImageUI.sprite = warriorSpriteList[currentFrame];
            countTime = 0f;
        }
    }
    public void SetPowerText(bool isPossessed,float power)
    {
        if (powerText == null)
        {
            UnityEngine.Debug.Log("PowerText attribute is null reference!");
            return;
        }
        if (isPossessed)
        {
            int powerInt = Mathf.FloorToInt(power);
            powerText.text = power_str + powerInt.ToString();
            powerText.alignment = TextAlignmentOptions.MidlineLeft;
        }
        else
        {
            powerText.text = notPossess_str;
            powerText.alignment = TextAlignmentOptions.Center;
        }
    }
    public void SetStartWarrior(int numStart)
    {
        for (int i = 0; i < startList.Count; i++)
        {
            if (startList[i] == null)
            {
                UnityEngine.Debug.Log("start object in " + i.ToString() + " is null !");
                return;
            }
            startList[i].color = i < numStart ? Color.white : Color.black;
        }
    }
    public void SetWarriorImageUI(bool isPossessed)
    {
        warriorImageUI.color = isPossessed ? Color.white : Color.black;
    }
    public void SetUpWarriorCardUI(Warrior warrior ,WarriorRankValue rankValue)
    {
        if(warrior == null)
        {
            UnityEngine.Debug.Log("Warrior is null");
            return;
        }
        id = warrior.GetID();
        warriorSpriteList = warrior.GetWarriorSprites();
        warriorImageUI.sprite = warriorSpriteList.First();
        rank = rankValue.rank;
        SetWarriorImageUI(warrior.GetStar() > 0);
        GetComponent<Image>().color = rankValue.bg;
        SetStartWarrior(warrior.GetStar());
        SetPowerText(warrior.GetStar() > 0, warrior.GetCurrentPower());
    }
    public int GetID()
    {
        return id;
    }
}
