using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropLogic : MonoBehaviour
{
   [Header("variables for item drop table logic:")]
   [Header("DropTable for this type of enemy")]
   //drop table slots, one of which will be dropped at call
   public GameObject itemOne;
   public GameObject itemTwo;
   public GameObject itemThree;
   public GameObject itemFour;
   public GameObject itemFive;

   [Header("drop weights for items (higher = more likely)")]
   public int iOneDw;
   public int iTwoDw;
   public int iThreeDw;
   public int iFourDw;
   public int iFiveDw;
   private int _weightTotal;

   [Header("chance of any item dropping (percentage, 0=0%, 100=100%")]
   public int chanceOfDrop;
   private int _dropRoll;
   private int _selector;
   
   
   
   [Header("Variables for potion drop logic:")]
   public GameObject potionDrop;
   public int chanceOfPotion;
   private int _potionRoll;



   [Header("Variables for money drop logic:")]
   public GameObject moneyDrop;
   public int chanceOfMoney;
   private int _moneyRoll;

   private void Start()
   {
      //item drop logic:
      _dropRoll = Random.Range(1, 100);
      _weightTotal = iOneDw + iTwoDw + iThreeDw + iFourDw + iFiveDw;
      if (_dropRoll < chanceOfDrop) { _selector = Random.Range(1, _weightTotal); }

      
      /*//potion drop logic:
      _potionRoll = Random.Range(1, 100);
      if (_potionRoll < chanceOfPotion)
      {
         Vector3 variation = new Vector3(Random.Range(0.1f, 1f), 0, Random.Range(0.1f, 1f));
         Instantiate(potionDrop, transform.position+variation, quaternion.identity);
      }

      
      //money drop logic:
      _moneyRoll = Random.Range(1, 100);
      if (_moneyRoll < chanceOfMoney)
      {
         Vector3 variation = new Vector3(Random.Range(0.1f, 1f), 0, Random.Range(0.1f, 1f));
         Instantiate(moneyDrop, transform.position+variation, quaternion.identity);
      }*/
   }

   public void DropItem()
   {
      DropPotion();
      DropMoney();
      Vector3 variation = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
      switch (_selector)
      {
         case var n when n >=iOneDw:
            Instantiate(itemOne, transform.position+variation, quaternion.identity); 
            break;
         case var n when n <iOneDw && n>=iTwoDw:
            Instantiate(itemTwo, transform.position+variation, quaternion.identity); 
            break;
         case var n when n <iTwoDw && n>=iThreeDw:
            Instantiate(itemThree, transform.position+variation, quaternion.identity); 
            break;
         case var n when n<iThreeDw && n>=iFourDw:
            Instantiate(itemFour, transform.position+variation, quaternion.identity); 
            break;
         case var n when n<iFourDw:
            Instantiate(itemFive, transform.position+variation, quaternion.identity); 
            break;
      }
   }

   public void DropPotion()
   {
      _potionRoll = Random.Range(1, 100);
      if (_potionRoll < chanceOfPotion)
      {
         Vector3 variation = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
         Instantiate(potionDrop, transform.position+variation, quaternion.identity);
      }
   }

   public void DropMoney()
   {
      _moneyRoll = Random.Range(1, 100);
      if (_moneyRoll < chanceOfMoney)
      {
         Vector3 variation = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
         Instantiate(moneyDrop, transform.position+variation, quaternion.identity);
      }
   }
}
