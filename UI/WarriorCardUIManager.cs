using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
public class WarriorCardUIManager : MonoBehaviour
{
    [SerializeField] private WarriorManager warriorManager;
    [SerializeField] private WarriorCardUI warriorCardUIPrefap;
    [SerializeField] private RectTransform warriorPosOrigin;
    [SerializeField] private GameManager gameManager;
    private List <WarriorCardUI> warriorCardList = new List<WarriorCardUI>();
    private float paddingRight = 160f;
    private float paddingTop = 180f;
    private int numColumn = 6;
    private void Start()
    {
        if(warriorManager == null)
        {
            Debug.Log("Warrior manager in WarriorUIManager is null !");
            return;
        }
        CreateWarriorCardUIList();
        gameObject.SetActive(false);
    }
    public WarriorCardUI CreateWarriorCard(Vector3 pos)
    {
        GameObject warriorCardUI_init = Instantiate(warriorCardUIPrefap.gameObject, gameObject.transform);
        warriorCardUI_init.GetComponent<RectTransform>().anchoredPosition = pos;
        WarriorCardUI warriorCardUIScript = warriorCardUI_init.GetComponent<WarriorCardUI>();
        return warriorCardUIScript;
    }
    public void CreateWarriorCardUIList()
    {
        List<GameObject> warriorPrefapList = warriorManager.GetWarriorRefapList();
        int numRow = warriorPrefapList.Count / numColumn;
        int count = 0;
        for(int row = 0; row <= numRow; row++)
        {
            for (int col = 0; col < numColumn; col++, count++)
            {
                if (count == warriorPrefapList.Count) return;
                if (warriorPrefapList[count] == null)
                {
                    col--;
                    continue;
                }
                Vector3 warriorPos = new Vector3();
                warriorPos.x = warriorPosOrigin.anchoredPosition.x + col * paddingRight;
                warriorPos.y = warriorPosOrigin.anchoredPosition.y - row * paddingTop;
                WarriorCardUI warriorCardUI = CreateWarriorCard(warriorPos);
                Warrior warrior = warriorPrefapList[count].GetComponent<Warrior>();
                WarriorRankValue rankValue = gameManager.GetWarriorRankValue(warrior.GetRank());
                warriorCardUI.SetUpWarriorCardUI(warrior, rankValue);
                warriorCardList.Add(warriorCardUI);
            }
        }
    }
    public WarriorCardUI FindWarriorCardUIByID(int id)
    {
        foreach (var warriorCard in warriorCardList)
        {
            if (warriorCard != null && warriorCard.GetID() == id)
            {
                return warriorCard;
            }
        }
        return null;
    }
    public void OpenWarriorCardListUI()
    {
        if (gameObject.activeSelf)
        {
            CloseWarriorCardListUI();
            return;
        }
        gameObject.SetActive(true);
        int count = 0;
        bool isSwap = false;
        gameManager.isPauseGame = true;
        Time.timeScale = 0f;
        foreach (var warriorObj in warriorManager.GetWarriorPossessList())
        {
            if(warriorObj == null) continue;
            Warrior warrior = warriorObj.GetComponent<Warrior>();
            WarriorCardUI warriorCardUI = FindWarriorCardUIByID(warrior.GetID());
            warriorCardUI.SetWarriorImageUI(true);
            warriorCardUI.SetPowerText(true, warrior.GetCurrentPower());
            warriorCardUI.SetStartWarrior(warrior.GetStar());
            WarriorCardUI warriorCardAtPos = warriorCardList[count];
            if(warriorCardUI.GetID() != warriorCardAtPos.GetID())
            {
                int i = warriorCardList.IndexOf(warriorCardUI);
                SwapWarriorCardUI(warriorCardList[count], warriorCardList[i]);
                isSwap = true;
            }
            count++;
        }
        if (!isSwap) return;
        for (int i = count; i < warriorCardList.Count - 1; i++)
        {
            for (int j = i + 1; j < warriorCardList.Count; j++)
            {
                if (WarriorRankManager.CompareRank(warriorCardList[i].rank, warriorCardList[j].rank) < 0)
                {
                    SwapWarriorCardUI(warriorCardList[i], warriorCardList[j]);
                }
                else if (WarriorRankManager.CompareRank(warriorCardList[i].rank, warriorCardList[j].rank) == 0)
                {
                    if (warriorCardList[i].GetID() < warriorCardList[j].GetID())
                    {
                        SwapWarriorCardUI(warriorCardList[i], warriorCardList[j]);
                    }
                }
            }
        }
    }
    private void SwapWarriorCardUI(WarriorCardUI w1, WarriorCardUI w2)
    {
        Vector3 posTemp = w1.transform.position;
        w1.transform.position = w2.transform.position;
        w2.transform.position = posTemp;
        WarriorCardUI temp = w1;
        w1 = w2;
        w2 = temp;
    }
    public void CloseWarriorCardListUI()
    {
        if(!gameObject.activeSelf) return;
        gameObject.SetActive(false);
        gameManager.isPauseGame = false;
        Time.timeScale = 1f;
    }
}