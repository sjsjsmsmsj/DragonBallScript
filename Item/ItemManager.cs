using UnityEngine;
using System.Collections.Generic;
public static class ItemByID
{
    public const int NAPPA_SOUL = 1;
    public const int GOLD = 2;
    public const int DIAMOND = 3;
}

public class ItemManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemPrefapList = new List<GameObject>();
    public GameObject FindItemPrefaps(int id)
    {
        foreach (var item in itemPrefapList)
        {
            Item itemScript = item.GetComponent<Item>();
            if (itemScript != null && itemScript.GetID() == id)
            {
                return item;
            }
        }
        return null;
    }
    public List<GameObject> GetItemPrefapList() { return itemPrefapList; }
}