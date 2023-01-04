using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //programmer beware - black magic inspired by google follows.
    private readonly Dictionary<string, int> _itemDict = new(); //holds all item value pairs

    private void Start()
    {
        DirectoryInfo itemsDir = new DirectoryInfo("Assets/item-related/Item Prefabs/Item ScriptableObjects"); //points at directory
        FileInfo[] files = itemsDir.GetFiles("*.asset"); //pulls appropriate files from directory
        foreach (FileInfo file in files) //runs through array initializing entries in dictionary
        {
            Debug.Log("file found: " + file.Name + "\n"); //tells dev what's been found
            _itemDict.Add(file.Name.TrimEnd(".asset"),0); //prunes .asset from file found, initializes entry
        }

        foreach (KeyValuePair<string, int> item in _itemDict)
        {
            Debug.Log("item: " + item.Key +" value: " + item.Value); //tells dev what entries have been made
        }
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
}
