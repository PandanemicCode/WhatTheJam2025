using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class CharacterSelectPlayer : MonoBehaviour
{
    
    public Image portraitImage;
    public TMP_Text nameText;
    public GameObject readyIndicator;

    [Header("Optional: Only used if layout group isn't found")]
    public string layoutGroupName = "PlayerPanelLayout";  // <- Set this to match the GameObject in the scene
    public string fallbackCanvasName = "Canvas";          // <- Used if layout group isn't found

    private int characterIndex = 0;
    private bool isReady = false;
    private CharacterSelectionManager selectionManager;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public InputDevice GetInputDevice()
    {
        if (playerInput != null && playerInput.devices.Count > 0)
            return playerInput.devices[0];
        return null;
    }

    private void Start()
    {
        selectionManager = FindObjectOfType<CharacterSelectionManager>();
        selectionManager.RegisterPlayer(this);
        UpdateVisual();

        // Try to find layout group by name
        Transform layoutGroup = GameObject.Find(layoutGroupName)?.transform;
        Transform fallbackCanvas = GameObject.Find(fallbackCanvasName)?.transform;

        // Choose where to parent the UI panel
        Transform parentToUse = layoutGroup != null ? layoutGroup : fallbackCanvas;

        if (parentToUse != null)
        {
            transform.SetParent(parentToUse, false);
            RectTransform rt = GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            Debug.LogWarning("No layout group or fallback canvas found! Panel may not appear correctly.");
        }
    }

    public void Navigate()
    {
        if (isReady) return;

        Vector2 input = UnityEngine.InputSystem.Gamepad.current.leftStick.ReadValue();
        if (input.x > 0.5f) ChangeCharacter(1);
        else if (input.x < -0.5f) ChangeCharacter(-1);
    }

    public void Submit()
    {
        if (!isReady && !selectionManager.IsCharacterTaken(characterIndex))
        {
            isReady = true;
            readyIndicator.SetActive(true);
        }
    }

    public void Cancel()
    {
        if (!isReady) return;
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
