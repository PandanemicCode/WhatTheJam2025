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

            CharacterSelectionData.selectedCharacterIndices.Clear();
            CharacterSelectionData.playerInputDevices.Clear();

            CharacterSelectionData.Clear();

                foreach (var player in players)  // Replace 'players' with your actual list of joined players
                {
                     CharacterSelectionData.selectedCharacterIndices.Add(player.GetCharacterIndex());
                    CharacterSelectionData.playerInputDevices.Add(player.GetInputDevice());
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // or "GameScene"

    }
}


}
