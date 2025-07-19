using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public CharacterData[] characterOptions;

    private List<CharacterSelectPlayer> players = new List<CharacterSelectPlayer>();

    public void RegisterPlayer(CharacterSelectPlayer player)
    {
        players.Add(player);
    }

    public bool IsCharacterTaken(int index)
    {
        foreach (var p in players)
        {
            if (p.IsReady() && p.GetCharacterIndex() == index)
                return true;
        }
        return false;
    }

    private void Update()
    {
        if (players.Count > 0 && players.TrueForAll(p => p.IsReady()))
        {
            Debug.Log("All players ready!");
            // Pass character selections here or load game
            SceneManager.LoadScene("GameScene");
        }
    }
}
