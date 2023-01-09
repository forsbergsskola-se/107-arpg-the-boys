using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmoryScriptableObject", menuName = "Armory/Weapons", order = 0)]
public class Armory : ScriptableObject
{
    public ArmoryWeapon[] armoryWeapons;
    
    [Serializable]
    public class ArmoryWeapon
    {
        public BaseWeapon weapon;
        public bool isBought;
        public int cost;
    }
}
