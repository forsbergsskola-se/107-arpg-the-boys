using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    public DungeonRoom deadEnd;
    public LayerMask roomLayer;
    public List<RoomList> allRooms;
    public int generatedRoomAmount;
    public int desiredRoomAmount;
    public Transform currentPivot;
    public DungeonRoom currentRoom;
    public DungeonRoom latestRoom;//s
    private Transform[] currentPivots;
    private List<DungeonRoom> allCollectedRooms = new List<DungeonRoom>(); 
    void Awake()
    {
        foreach (RoomList list in allRooms)
        {
            allCollectedRooms.AddRange(list.rooms);
        }
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
            // om vi har tre rum och tre punkter:
            // om vi har fyra rum och tre punkter:
            // skapa ett 2-rum och sedan fylla med 1-rum
            
            // om vi har fem rum och tre punkter:
            // vi kan skapa ett 3-rum som leder till...
            // fyra rum och fyra punkter...         WE GOOD
            
            // om vi har sex rum och tre punkter
            // vi kan skapa ett 4-rum som leder till...
            // fem rum och fem punkter
            
            //om vi har sju rum och tre punkter
            //vi kan skapa ett 5-rum NEJ
            //vi kan skapa ett 4-rum som leder till...
            //sex rum och sex punkter!
            //vi kan också skapa ett 3-rum som leder till...
            //sex rum och fem punkter, sen kan vi...
            //ev två sen WE GOOD
            
            //om vi har sju rum och sex punkter...
            //vi FÅR INTE SKAPA ETT 1-RUM ALDRIG NEJNEJ
            
            //när får vi skapa ett 1-rum?
            //när vi har lika många rum som punkter.
            
            //rummet 
            int remainingRooms = desiredRoomAmount - generatedRoomAmount;
            int from = remainingRooms == pivotsProcessing.Count ? 0 : 1;
            int to = Mathf.Clamp(remainingRooms - pivotsProcessing.Count, 0, allRooms.Count);
            doorAmount = Random.Range(from, to);
            do
            {
                currentPivots = PlaceValidRoom(currentTransform.position, currentTransform.rotation, doorAmount);
                doorAmount--;
            } while (currentPivots == null && doorAmount >= 0);
            //currentPivots = PlaceValidRoom(currentTransform.position, currentTransform.rotation, doorAmount); // SpawnRoom(doorAmount, currentTransform.position, currentTransform.rotation);
            if (currentPivots==null)
            {
                Debug.Log("We coudn't spawn a room with any amount of doors under the required, spawning a dead end.");
                SpawnRoom(deadEnd, currentTransform.position, currentTransform.rotation);
            }
            else
            {
                foreach (Transform pivot in currentPivots)
                {
                    pivotsProcessing.Enqueue(pivot);
                }
                Debug.Log("We've created a room");
                generatedRoomAmount++;
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
    Transform[] SpawnRoom(DungeonRoom room, Vector3 pos,Quaternion rot)
    {
        DungeonRoom currentRoom = Instantiate(room,pos, Quaternion.identity);
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

    private List<DungeonRoom> currentNotChecked;
    private Transform[] PlaceValidRoom(Vector3 pos, Quaternion rot, int doorAmount)
    {
        currentNotChecked = GetBiasedRoomList(doorAmount, true);
        for (int i = 0; i < currentNotChecked.Count; i++)
        {
            int rand = Random.Range(0, currentNotChecked.Count);
            Transform[]points = SpawnRoomCheckDoors(currentNotChecked[rand], pos, rot);
            if (points==null|| points.Length==0)
            {
                currentNotChecked.RemoveAt(rand);
            }
            else
            {
                return points;
            }
            
        }

        return null;
    }   
    private List<DungeonRoom> GetBiasedRoomList(int doors, bool include)
    {
        //List<DungeonRoom> localRooms = include? allRooms[doors].rooms : allCollectedRooms;
        List<DungeonRoom> localRooms = new();

        foreach (DungeonRoom room in allRooms[doors].rooms)
        {
            localRooms.Add(room);
        }
        if (!include)
        {
            for (int i = 0; i < allRooms.Count; i++)
            {
                if (i == doors) continue;
                
                for (int j = 0; j < allRooms[i].rooms.Count; j++)
                {
                    localRooms.Add(allRooms[i].rooms[j]);
                }
            }
        }

        return localRooms;
    }
    private Collider[] _colBuffer = new Collider[1111];
    Transform[] SpawnRoomCheckDoors(DungeonRoom room, Vector3 pos,Quaternion rot)
    {
        DungeonRoom currentRoom = Instantiate(room,pos, Quaternion.identity);
        for (int j = 0; j < currentRoom.pivotPoints.Length; j++)
        {
            //currentRoom.gameObject.SetActive(false);
            Transform currentPivotPoint = currentRoom.pivotPoints[j];
            Quaternion roomRot = Quaternion.FromToRotation(currentPivotPoint.rotation * -Vector3.right, rot * Vector3.right);
            roomRot = Quaternion.LookRotation(roomRot * Vector3.forward, Vector3.up);
            currentRoom.transform.rotation *= roomRot;
            //rot = currentPivotPoint.rotation;
            
            Vector3 move = pos - currentPivotPoint.position;
            currentRoom.transform.position += move;

            Collider[] colliders = currentRoom.GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            for (int i = 0; i < colliders.Length; i++)
            {
                
                int count = Physics.OverlapBoxNonAlloc(currentRoom.transform.position + colliders[i].bounds.center, colliders[i].bounds.extents, _colBuffer, currentRoom.transform.rotation, roomLayer, QueryTriggerInteraction.Collide);
                
                if (count>0)
                {
                    break;
                }
                else
                {
                    Transform[] unusedPivots = new Transform[currentRoom.pivotPoints.Length-1];
                    for (int k = 0; k < unusedPivots.Length;k++)
                    {
                        if (k < j)
                        {
                            unusedPivots[k] = currentRoom.pivotPoints[k];
                        }
                        else
                        {
                            unusedPivots[k] = currentRoom.pivotPoints[k+1];
                        }
                    }
                    foreach (Collider col in colliders)
                    {
                        col.enabled = true;
                    }
                    return unusedPivots;
                }
            }
            
        }
        Destroy(currentRoom);

        return null;

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
