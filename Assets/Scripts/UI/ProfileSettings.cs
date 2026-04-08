using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using TypTyp.Input;
using TypTyp.TextSystem.Typable;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ProfileSettings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI helpText;
    private readonly string defaultUsername = "AverageCultist";

    private string currentName = string.Empty;
    private bool isTyping;
    private WritableButton usernameButton;
    private TypableController usernameTypCont;
    private WritableButton[] allWritableButtons;
    private readonly int minNameLength = 4;
    private readonly int maxNameLength = 15;

    private void Awake()
    {
        allWritableButtons = GetComponentsInChildren<Button>().Select(b => b.GetComponent<WritableButton>()).ToArray();
        usernameButton = usernameText.GetComponentInParent<WritableButton>();
        usernameTypCont = usernameButton.GetComponent<TypableController>();
    }

    private void OnEnable()
    {
        SaveManager.Instance.OnBeforeSave += HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad += HandleAfterLoad;
    }

    private void Start()
    {
        if (SaveManager.Instance.HasLoadedState)
        {
            SaveState state = SaveManager.Instance.GetState();
            ApplyProfile(state);
        }
        else
        {
            currentName = GenerateDisplayName(defaultUsername);
            usernameButton.OverrideText(currentName);
        }
    }

    private void OnDisable()
    {
        if (SaveManager.Instance == null) return;

        SaveManager.Instance.OnBeforeSave -= HandleBeforeSave;
        SaveManager.Instance.OnAfterLoad -= HandleAfterLoad;
    }

    private void Update()
    {
        if (!isTyping || Keyboard.current == null) return;

        if (Keyboard.current.backspaceKey.wasPressedThisFrame && currentName.Length > 0)
        {
            currentName = currentName.Substring(0, currentName.Length - 1);
            usernameButton.OverrideText(currentName);
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            SubmitName();
        }

        if (isTyping)
        {
            for (int i = 0; i < allWritableButtons.Length; i++)
            {
                allWritableButtons[i].CompletelyBlock(true);
            }
        }
    }
    public void ChangeName()
    {
        if (isTyping) return;
        isTyping = true;
        currentName = string.Empty;
        usernameButton.OverrideText(currentName);

        helpText.enabled = true;
        helpText.text = "Press enter to save name";

        ToggleWritableButtons(true);
        InputHandler.Instance.AddListener(OnCharacterTyped);
    }

    private void OnCharacterTyped(char c)
    {
        if (!isTyping || currentName.Length >= maxNameLength) return;

        currentName += c;
        usernameButton.OverrideText(currentName);

        if (usernameTypCont != null)
            usernameTypCont.enabled = false;
    }

    private void SubmitName()
    {
        isTyping = false;

        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.RemoveListener(OnCharacterTyped);
        }

        ToggleWritableButtons(false);
        currentName = currentName.Trim();
        if (CheckText(currentName))
        {
            usernameText.text = currentName;
            helpText.text = "Name saved!";
            SaveManager.Instance.Save();
        }
        else if (SaveManager.Instance.HasLoadedState)
        {
            SaveState state = SaveManager.Instance.GetState();
            ApplyProfile(state);
        }
        else
        {
            currentName = GenerateDisplayName(defaultUsername);
        }

        usernameButton.OverrideText(currentName);
        usernameButton.Block = false;
    }

    private void ToggleWritableButtons(bool blockState)
    {
        for (int i = 0; i < allWritableButtons.Length; i++)
        {
            allWritableButtons[i].CompletelyBlock(blockState);
            if (allWritableButtons[i].TryGetComponent<HoverEffect>(out HoverEffect he))
            {
                he.enabled = !blockState;
            }
        }
    }

    private bool CheckText(string textToCheck)
    {
        if (string.IsNullOrWhiteSpace(textToCheck))
        {
            helpText.text = "Name cannot be empty.";
            return false;
        }

        if (textToCheck.Length < minNameLength || textToCheck.Length > maxNameLength)
        {
            helpText.text = $"Name must be between {minNameLength} and {maxNameLength} characters.";
            return false;
        }

        if (!Regex.IsMatch(textToCheck, @"^[a-zA-Z0-9]+$"))
        {
            helpText.text = "Name can only contain letters and numbers.";
            return false;
        }

        return true;
    }

    public void ExitProfile()
    {
        helpText.enabled = false;
    }

    private void OnDestroy()
    {
        if (isTyping && InputHandler.Instance != null)
        {
            InputHandler.Instance.RemoveListener(OnCharacterTyped);
        }
    }

    public void LinkAccount()
    {
        Debug.Log("Not implemented yet");
    }

    private void HandleBeforeSave(SaveState state)
    {
        state.slot.profile.username = currentName.Trim();
    }

    private void HandleAfterLoad(SaveState state)
    {
        ApplyProfile(state);
    }

    private void ApplyProfile(SaveState state)
    {
        currentName = GenerateDisplayName(state?.slot?.profile?.username);

        if (usernameButton != null)
        {
            usernameButton.OverrideText(currentName);
        }
    }

    private string GenerateDisplayName(string candidate)
    {
        string normalized = string.IsNullOrWhiteSpace(candidate) ? defaultUsername : candidate.Trim();
        if (!normalized.Equals(defaultUsername))
        {
            return normalized;
        }

        string generated = normalized + "#";
        for (int i = 0; i < 4; i++)
        {
            generated += Random.Range(0, 10).ToString();
        }

        return generated;
    }
}
