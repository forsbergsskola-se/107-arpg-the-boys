using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //programmer beware - black magic inspired by google follows.
    private readonly Dictionary<string, int> _itemDict = new(); //holds all item value pairs
    public ItemScriptableObject[] itemsThatExist;
    private void Start()
    {
        foreach (ItemScriptableObject item in itemsThatExist) //runs through array initializing entries in dictionary
        {
            Debug.Log("file found: " + item + "\n"); //tells dev what's been found
            _itemDict.Add(item.name,0); //initializes entry
        }

        foreach (KeyValuePair<string, int> item in _itemDict)
        {
            Debug.Log("item: " + item.Key +" value: " + item.Value); //tells dev what entries have been made
        }
        ShopSellMoney("Money", PlayerPrefs.GetInt("Money-Count", 50));

    }

    public int GetItemCount(string itemName)
    { 
        return _itemDict[itemName];
    }

    public void UpdateItemCount(string itemName) //gets called by item script, item name = scriptable object file name
    {
        _itemDict[itemName]++; //increments with flat +1
        Debug.Log("incrementing: " + itemName + " new amount held: " + _itemDict[itemName]);
    }

    public void SellItem(string itemName)
    {
        _itemDict[itemName]--; //increments with flat -1
        Debug.Log("selling: " + itemName + "new amount held: " + _itemDict[itemName]);
    }
    
    public void ShopSellMoney(string itemName, int valuechange)
    {
        _itemDict[itemName] =+ valuechange;
    }
}