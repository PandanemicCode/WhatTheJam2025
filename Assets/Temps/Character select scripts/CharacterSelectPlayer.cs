using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectPlayer : MonoBehaviour
{
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public GameObject readyIndicator;

    private int characterIndex = 0;
    private bool isReady = false;

    private CharacterSelectionManager selectionManager;
    private PlayerInput playerInput;

    private void Start()
    {
        selectionManager = FindObjectOfType<CharacterSelectionManager>();
        playerInput = GetComponent<PlayerInput>();
        selectionManager.RegisterPlayer(this);
        UpdateVisual();
    }

    public void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || isReady) return;

        Vector2 input = ctx.ReadValue<Vector2>();
        if (input.x > 0.5f) ChangeCharacter(1);
        else if (input.x < -0.5f) ChangeCharacter(-1);
    }

    public void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!isReady && !selectionManager.IsCharacterTaken(characterIndex))
        {
            isReady = true;
            readyIndicator.SetActive(true);
        }
    }

    public void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !isReady) return;
        isReady = false;
        readyIndicator.SetActive(false);
    }

    private void ChangeCharacter(int direction)
    {
        int count = selectionManager.characterOptions.Length;
        int tries = 0;

        do
        {
            characterIndex = (characterIndex + direction + count) % count;
            tries++;
        } while (selectionManager.IsCharacterTaken(characterIndex) && tries < count);

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        var data = selectionManager.characterOptions[characterIndex];
        portraitImage.sprite = data.characterPortrait;
        nameText.text = data.characterName;
    }

    public bool IsReady() => isReady;
    public int GetCharacterIndex() => characterIndex;
}
