using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public DungeonRoom[] rooms;
    public int generatedRoomAmount;
    public int desiredRoomAmount;
    public Queue<GameObject> pivotsProcessing;
    private DungeonRoom currentRoom;
    void Awake()
    {
        SpawnFirstRoom();
        while (generatedRoomAmount < desiredRoomAmount)
        {
            SpawnRoom();
        }
    
    }

    void SpawnRoom()
    {
        transform.position = pivotsProcessing.Dequeue().transform.position;
        currentRoom = rooms[Random.Range(0, rooms.Length)];
        if (generatedRoomAmount + currentRoom.pivotPoints.Length + pivotsProcessing.Count < desiredRoomAmount)
        {
            Instantiate(currentRoom, transform.position, transform.rotation);
            generatedRoomAmount++;
        }

        for (int i = 0; i < currentRoom.pivotPoints.Length; i++)
        {
            pivotsProcessing.Enqueue(currentRoom.pivotPoints[i]);
        }
        
    }

    void SpawnFirstRoom()
    {
        
        while (generatedRoomAmount <1)
        {
            currentRoom = rooms[Random.Range(0, rooms.Length)];
            if (currentRoom.pivotPoints.Length >1)
            {
                Instantiate(currentRoom, transform.position, transform.rotation);
                generatedRoomAmount++;
            }
        }
        for (int i = 0; i < currentRoom.pivotPoints.Length; i++)
        {
            pivotsProcessing.Enqueue(currentRoom.pivotPoints[i]);
        }
    }
}
