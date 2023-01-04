using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropLogic : MonoBehaviour
{
   //drop table slots, one of which will be dropped at call
   public GameObject itemOne;
   public GameObject itemTwo;
   public GameObject itemThree;
   public GameObject itemFour;
   public GameObject itemFive;

   private int _dropRoll;

   private void Start()
   {
      _dropRoll = Random.Range(1, 5);
   }

   public void DropItem()
   {
      switch (_dropRoll)
      {
         case 1:
            Instantiate(itemOne, transform.position , quaternion.identity);
            break;
         case 2:
            Instantiate(itemTwo, transform.position, quaternion.identity);
            break;
         case 3:
            Instantiate(itemThree, transform.position, quaternion.identity);
            break;
         case 4:
            Instantiate(itemFour, transform.position, quaternion.identity);
            break;
         case 5:
            Instantiate(itemFive, transform.position, quaternion.identity);
            break;
      }
   }
}
