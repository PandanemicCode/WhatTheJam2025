using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Tooltip("Assign empty GameObjects representing each room's spawn point")]
    public List<Transform> roomSpawnPoints = new List<Transform>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Returns a random room NOT occupied by any of the given players.
    /// </summary>
    public Transform GetAvailableRoom(List<Transform> excludedPositions)
    {
        List<Transform> available = new List<Transform>();

        foreach (var room in roomSpawnPoints)
        {
            bool occupied = false;
            foreach (var ex in excludedPositions)
            {
                if (Vector3.Distance(ex.position, room.position) < 0.5f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                available.Add(room);
        }

        if (available.Count == 0)
            return null;

        return available[Random.Range(0, available.Count)];
    }

    public Transform GetRandomRoom()
    {
        if (roomSpawnPoints.Count == 0) return null;
        return roomSpawnPoints[Random.Range(0, roomSpawnPoints.Count)];
    }
}
