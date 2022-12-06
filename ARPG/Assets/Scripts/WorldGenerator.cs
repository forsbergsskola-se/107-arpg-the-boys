using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    
    public List<RoomList> allRooms;
    public int generatedRoomAmount;
    public int desiredRoomAmount;
    public Transform currentPivot;
    public DungeonRoom currentRoom;
    public DungeonRoom latestRoom;//s
    private Transform[] currentPivots;
    void Awake()
    {
        GenerateRooms(desiredRoomAmount,Vector3.zero, Quaternion.identity);
    }

    public DungeonRoom GetRoom(int amountOfDoors)
    {
        return allRooms[amountOfDoors].rooms[Random.Range(0,allRooms[amountOfDoors].rooms.Count)];
    }

    void GenerateRooms(int desiredRoomAmount, Vector3 startPos, Quaternion startRot)
    {
        Queue<Transform> pivotsProcessing = new();
        generatedRoomAmount =1;
        int doorAmount = Random.Range(1, 4);
        currentPivots = SpawnRoom(doorAmount, startPos, startRot);
        foreach (Transform pivot in currentPivots)
        {
            pivotsProcessing.Enqueue(pivot);
        }
        
        while (pivotsProcessing.Count > 0 && generatedRoomAmount < desiredRoomAmount)
        {
            Transform currentTransform = pivotsProcessing.Dequeue();
            Debug.Log(currentTransform);
            ///////
            doorAmount = Random.Range(1, 4);
            while (doorAmount + pivotsProcessing.Count + generatedRoomAmount >= desiredRoomAmount)
            {
                doorAmount = Random.Range(0, 4);
            }
            ///////
            currentPivots = SpawnRoom(doorAmount, currentTransform.position, currentTransform.rotation);
            generatedRoomAmount++;
            foreach (Transform pivot in currentPivots)
            {
                pivotsProcessing.Enqueue(pivot);
            }
        }
    }

    Transform[] SpawnRoom(int doors, Vector3 pos,Quaternion rot)
    {
        DungeonRoom currentRoom = Instantiate(GetRoom(doors),pos, Quaternion.identity);
        int randomPivot = Random.Range(0,currentRoom.pivotPoints.Length);
        Transform currentPivotPoint = currentRoom.pivotPoints[randomPivot];
        Quaternion roomRot = Quaternion.FromToRotation(currentPivotPoint.rotation * -Vector3.right, rot * Vector3.right);
        roomRot = Quaternion.LookRotation(roomRot * Vector3.forward, Vector3.up);
        currentRoom.transform.rotation *= roomRot;

        Vector3 move = pos - currentPivotPoint.position;
        currentRoom.transform.position += move;

        Transform[] unusedPivots = new Transform[currentRoom.pivotPoints.Length-1];
        for (int i = 0; i < unusedPivots.Length;i++)
        {
            if (i < randomPivot)
            {
                unusedPivots[i] = currentRoom.pivotPoints[i];
            }
            else
            {
                unusedPivots[i] = currentRoom.pivotPoints[i+1];
            }
        }
        
        return unusedPivots;
    }

    #region old Code, unused and unnecessary atm
/*
    void SpawnRoom2()
    {
        currentPivot = pivotsProcessing.Dequeue();
        transform.position = currentPivot.transform.position;
        do
        {
            //currentRoom = rooms[Random.Range(0, rooms.Length)];
        } 
        while (currentRoom.pivotPoints.Length - 1 + generatedRoomAmount + pivotsProcessing.Count > desiredRoomAmount);

        latestRoom = Instantiate(currentRoom, transform.position - currentRoom.pivotPoints[0].transform.localPosition,
            Quaternion.Euler(0, 180 - currentRoom.pivotPoints[0].transform.localRotation.y, 0));
        generatedRoomAmount++;


        for (int i = 0; i < latestRoom.pivotPoints.Length; i++)
        {
            pivotsProcessing.Enqueue(latestRoom.pivotPoints[i]);
        }

        if (generatedRoomAmount < desiredRoomAmount || pivotsProcessing.Count > 0)
        {
            SpawnRoom();
        }
    }
    void SpawnFirstRoom()
    {
        
        while (generatedRoomAmount <1)
        {
            if (currentRoom.pivotPoints.Length >1)
            {
                latestRoom = Instantiate(currentRoom, transform.position, transform.rotation);
                generatedRoomAmount++;
            }
        }
        for (int i = 0; i < latestRoom.pivotPoints.Length; i++)
        {
            Debug.Log(i);
            Debug.Log(latestRoom.pivotPoints[i]);
            Debug.Log("trying enqueue");
        }
    }
    */
    #endregion
    
}
[System.Serializable]
public class RoomList
{
    public List<DungeonRoom> rooms;
}
