using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PlayerCombat> allPlayers = new List<PlayerCombat>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnPlayersInDifferentRooms();
    }

    public void SpawnPlayersInDifferentRooms()
    {
        List<Transform> occupiedRooms = new List<Transform>();

        foreach (PlayerCombat player in allPlayers)
        {
            Transform room;
            if (player.currentRole == PlayerRole.Seeker)
            {
                // Seeker must not spawn in rooms with hiders
                room = RoomManager.Instance.GetAvailableRoom(occupiedRooms);
            }
            else
            {
                room = RoomManager.Instance.GetRandomRoom();
            }

            if (room != null)
            {
                player.transform.position = room.position;
                occupiedRooms.Add(room);
            }
            else
            {
                Debug.LogWarning("No available room for " + player.name);
            }
        }
    }

    /// <summary>
    /// Call this when a player turns into a seeker.
    /// </summary>
    public void RespawnAsSeeker(PlayerCombat seeker)
    {
        List<Transform> occupiedRooms = new List<Transform>();
        foreach (var p in allPlayers)
        {
            if (p != seeker && p.currentRole == PlayerRole.Hider)
                occupiedRooms.Add(p.transform);
        }

        Transform safeRoom = RoomManager.Instance.GetAvailableRoom(occupiedRooms);
        if (safeRoom != null)
            seeker.transform.position = safeRoom.position;
        else
            Debug.LogWarning("No safe room available for seeker respawn.");
    }
}
