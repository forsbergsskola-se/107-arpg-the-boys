using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask roomLayer;
    public int desiredRoomAmount;
    public int desiredShopAmount;
    public int desiredExitAmount;
    private int _generatedRoomAmount;
    
    [Header("Rubble, usually")]
    public DungeonRoom deadEnd;
    
    [Header("Shops")]
    public DungeonRoom[] Shops;
    
    [Header("BossRooms")]
    public DungeonRoom[] Exits;
    
    [Header("Rooms")]
    public List<RoomList> allRooms;
    
    private Transform[] _currentPivots;
    [Header("Debug")]
    public bool debugOn = false;
    public GameObject debugSphere;
    public GameObject debugCube;
    void Awake()
    {
        GenerateRooms(desiredRoomAmount,Vector3.zero, Quaternion.identity);
    }

    public DungeonRoom GetRoom(int amountOfDoors)
    {
        return allRooms[amountOfDoors].rooms[Random.Range(0,allRooms[amountOfDoors].rooms.Count)];
    }

    void GenerateRooms(int desiredRooms, Vector3 startPos, Quaternion startRot)
    {
        Queue<Transform> pivotsProcessing = new();
        _generatedRoomAmount=1;
        int from = desiredRooms >= 2 ? 1 : 0;
        int to = Math.Clamp(desiredRooms - 2, 0, allRooms.Count);
        int doorAmount = Random.Range(from, to);
        _currentPivots = SpawnRoom(doorAmount, startPos, startRot);
        foreach (Transform pivot in _currentPivots)
        {
            pivotsProcessing.Enqueue(pivot);
        }
        while (pivotsProcessing.Count > 0 && _generatedRoomAmount < desiredRooms)
        {
            Transform currentTransform = pivotsProcessing.Dequeue();
            int remainingRooms = desiredRooms - _generatedRoomAmount; 
            //Ordnar så att rummen som spawnar måste fungera med så många rum som vi vill ha.
            from = remainingRooms == pivotsProcessing.Count + 1 ? 0 : 1; 
            to = Mathf.Clamp(remainingRooms - pivotsProcessing.Count, 0, allRooms.Count);
            doorAmount = Random.Range(from, to);
            do
            {
                _currentPivots = PlaceValidRoom(currentTransform.position, currentTransform.rotation, doorAmount);
                doorAmount--;
            } while (_currentPivots == null && doorAmount >= 0);
            if (_currentPivots==null && wasDestroyed)
            {
                Debug.Log("We couldn't spawn a room with any amount of doors under the required, spawning a dead end.");
                SpawnRoom(deadEnd, currentTransform.position, currentTransform.rotation);
            }
            else
            {
                if (_currentPivots != null)
                {
                    foreach (Transform pivot in _currentPivots)
                    {
                        pivotsProcessing.Enqueue(pivot);
                    }
                }
                
                Debug.Log("We've created a room");
                _generatedRoomAmount++;
            }
        }
    }

    Transform[] SpawnRoom(int doors, Vector3 pos,Quaternion rot)
    {
        DungeonRoom currentRoom = Instantiate(GetRoom(doors),pos, Quaternion.identity);
        Debug.Log("Instantiated + " + currentRoom.name);
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
        Debug.Log("Instantiated + " + currentRoom.name);

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
            if (points==null)
            {
                currentNotChecked.RemoveAt(rand);
            }
            else
            {
                return points;
                //Debug.Log("Placed a valid room with " + points + " points.");
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
    private BoxCollider[] _colBuffer = new BoxCollider[1111];
    private bool wasDestroyed = false;
    private List<Transform> oneExitRooms;
    Transform[] SpawnRoomCheckDoors(DungeonRoom spawnRoom, Vector3 fromRoomPos,Quaternion fromRoomRot)
    {
        wasDestroyed = false;
        DungeonRoom currentRoom = Instantiate(spawnRoom,fromRoomPos, Quaternion.identity);
        Debug.Log("Instantiated + " + currentRoom.name);

        if (currentRoom.pivotPoints.Length == 1)
        {
            //oneExitRooms.Add(currentRoom.pivotPoints[0]);
        }
        //we start with this random value in order to start at a random entrance-point.
        int randomEntranceStart = Random.Range(0, currentRoom.pivotPoints.Length);
        
        //We loop through all possible entrance-orientations.
        for (int j = 0; j < currentRoom.pivotPoints.Length; j++)
        {
            //We randomize the pivot given by chosenPivot
            int jumpedIndex = (j + randomEntranceStart) % currentRoom.pivotPoints.Length; //sick line of code :sunglasses:
            //We set the current pivot to the randomly assigned one.
            Transform currentPivotPoint = currentRoom.pivotPoints[jumpedIndex];
            //Align the room to the current pivot and the spawn-location.
            Quaternion roomRot = Quaternion.FromToRotation(currentPivotPoint.rotation * Vector3.right, fromRoomRot * -Vector3.right);
            roomRot = Quaternion.LookRotation(roomRot * Vector3.forward, Vector3.up);
            currentRoom.transform.rotation *= roomRot;
            
            Vector3 move = fromRoomPos - currentPivotPoint.position;
            currentRoom.transform.position += move;

            BoxCollider[] colliders = currentRoom.GetComponents<BoxCollider>();
            foreach (BoxCollider col in colliders)
            {
                col.enabled = false;
            }
            
            //Loops through all colliders of the current room.
            for (int i = 0; i < colliders.Length; i++)
            {
                //Gets the amount of colliders overlapping with our own colliders and stores them in buffer.
                int count = Physics.OverlapBoxNonAlloc(
                    currentRoom.transform.position + currentRoom.transform.TransformVector(colliders[i].center),
                    colliders[i].size / 2, _colBuffer, currentRoom.transform.rotation, roomLayer,
                    QueryTriggerInteraction.Collide);

                //If we collide with any room, try another orientation.
                if (count>0)
                {
                    if (debugOn)
                    {
                        StoringShitThatDoesntExist shit = Instantiate(debugSphere, _colBuffer[0].transform.position,
                            Quaternion.identity).GetComponent<StoringShitThatDoesntExist>();
                        shit.name = "I tried being a + " + currentRoom.name + " but I collided with: " + _colBuffer[0].gameObject.name;
                        shit.collided = _colBuffer[0].gameObject;
                    } 
                    break;
                }
                
                if (debugOn) Instantiate(debugCube, currentRoom.transform.position + currentRoom.transform.TransformVector(colliders[i].center),
                    currentRoom.transform.rotation).transform.localScale = colliders[i].size;
                
                //If we didn't hit any rooms with any of the rooms' colliders.
                if (i == colliders.Length - 1)
                {
                    //Re-enable colliders of the room.
                    foreach (BoxCollider col in colliders)
                    {
                        col.enabled = true;
                    }
                    
                    //getting all the pivots that aren't the currently selected entrance.
                    Transform[] unusedPivots = new Transform[currentRoom.pivotPoints.Length-1];
                    for (int k = 0; k < unusedPivots.Length;k++)
                    {
                        if (k < jumpedIndex)
                        {
                            unusedPivots[k] = currentRoom.pivotPoints[k];
                        }
                        else
                        {
                            unusedPivots[k] = currentRoom.pivotPoints[k+1];
                        }
                    }
                    
                    return unusedPivots;
                }
            }
        }
        Destroy(currentRoom.gameObject);
        if (debugOn) Instantiate(debugCube, currentRoom.transform.position, Quaternion.identity).name = "I was destroyed.";

        wasDestroyed = true;
        
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
