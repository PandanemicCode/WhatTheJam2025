using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameSceneSetup : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public Transform defaultSpawnPoint;

    private void Start()
    {
        StartCoroutine(DelayedSpawn());
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.1f);  // Let the scene finish loading
        SpawnSelectedPlayers();
    }

    private void SpawnSelectedPlayers()
    {
        var selectedCharacters = CharacterSelectionData.selectedCharacterIndices;
        var devices = CharacterSelectionData.playerInputDevices;

        if (selectedCharacters.Count != devices.Count)
        {
            Debug.LogError("Mismatch in selected characters and input devices.");
            return;
        }

        for (int i = 0; i < selectedCharacters.Count; i++)
        {
            int characterIndex = selectedCharacters[i];
            var device = devices[i];

            GameObject prefab = characterPrefabs[characterIndex];
            if (prefab == null)
            {
                Debug.LogError($"Missing prefab for character index {characterIndex}");
                continue;
            }

            PlayerInput playerInput = PlayerInput.Instantiate(
                prefab,
                controlScheme: device is Gamepad ? "Gamepad" : "Keyboard&Mouse",
                pairWithDevice: device
            );

            GameObject playerObj = playerInput.gameObject;

            if (defaultSpawnPoint != null)
                playerObj.transform.position = defaultSpawnPoint.position;

            PlayerCombat combat = playerObj.GetComponent<PlayerCombat>();
            if (combat != null)
            {
                GameManager.Instance.allPlayers.Add(combat);
            }
            else
            {
                Debug.LogWarning("Spawned player is missing PlayerCombat script!");
            }
        }
    }
}
