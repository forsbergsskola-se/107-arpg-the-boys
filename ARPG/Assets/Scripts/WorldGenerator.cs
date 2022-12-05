using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public DungeonRoom[] rooms;
    public int generatedRoomAmount;
    public int desiredRoomAmount;
    public DungeonRoom currentRoom;
    void Awake()
    {
        if (enabled)
        {
            Instantiate(rooms[Random.Range(0, rooms.Length)], transform.position, transform.rotation);
        }
        while (generatedRoomAmount < desiredRoomAmount)
        {
            
        }
    
    }
}
