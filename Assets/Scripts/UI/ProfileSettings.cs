using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using TypTyp.Input;
using TypTyp.TextSystem.Typable;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ProfileSettings : MonoBehaviour, INavigationLeaveReceiver
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI helpText;
    [SerializeField] private TMP_Text numGames;
    [SerializeField] private TMP_Text numVictories;
    [SerializeField] private WritableButton goBackButton;
    [SerializeField] private TypableController goBackButtonTC;

    [SerializeField] private TextMeshProUGUI usernameButtonText;
    [SerializeField] private TextMeshProUGUI typingNameText;

    private readonly string defaultUsername = "AverageCultist";

    private string currentName = string.Empty;
    private bool isTyping;
    private WritableButton usernameButton;
    private TypableController usernameTypCont;
    private WritableButton[] allWritableButtons;
    private TypableController[] allWritableTexts;
    private readonly int minNameLength = 4;
    private readonly int maxNameLength = 15;

    private void Awake()
    {
        allWritableButtons = GetComponentsInChildren<Button>().Select(b => b.GetComponent<WritableButton>()).ToArray();
        allWritableTexts = GetComponentsInChildren<TypableController>().ToArray();
        usernameButton = usernameText.GetComponentInParent<WritableButton>();
        usernameTypCont = usernameButton.GetComponent<TypableController>();
        if (RuntimeVariables.Instance.IsLoaded) ApplyProfile(SaveManager.Instance.GetState());
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

        usernameButtonText.enabled = true;
        typingNameText.enabled = false;
        typingNameText.text = "";
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
            typingNameText.text = currentName;
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            SubmitName();
            return;
        }

        if (isTyping)
        {
            ToggleBlockWritableButtons(true);
        }
    }

    public void ChangeName()
    {
        if (isTyping) return;
        isTyping = true;
        currentName = string.Empty;
        usernameButton.OverrideText(currentName);

        usernameButtonText.enabled = false;
        typingNameText.enabled = true;
        typingNameText.text = "";

        helpText.enabled = true;
        helpText.text = "Press enter to save name";

        ToggleBlockWritableButtons(true);
        InputHandler.Instance.AddListener(OnCharacterTyped);
    }

    private void OnCharacterTyped(char c)
    {
        if (!isTyping || currentName.Length >= maxNameLength) return;

        currentName += c;
        typingNameText.text = currentName;

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

        ToggleBlockWritableButtons(false);
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

        usernameButtonText.enabled = true;
        typingNameText.enabled = false;

        usernameButton.Block = false;
    }

    private void CancelTyping()
    {
        if (!isTyping) return;
        isTyping = false;

        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.RemoveListener(OnCharacterTyped);
        }

        ToggleBlockWritableButtons(false);

        // Restablecer el nombre que se tenía guardado
        if (SaveManager.Instance != null && SaveManager.Instance.HasLoadedState)
        {
            SaveState state = SaveManager.Instance.GetState();
            ApplyProfile(state);
        }
        else
        {
            currentName = GenerateDisplayName(defaultUsername);
            if (usernameButton != null)
            {
                usernameButton.OverrideText(currentName);
            }
        }

        usernameButtonText.enabled = true;
        typingNameText.enabled = false;

        if (usernameButton != null) usernameButton.Block = false;
        if (helpText != null) helpText.enabled = false;
    }

    public void OnLeave()
    {
        CancelTyping();
    }

    private void ToggleBlockWritableButtons(bool blockState)
    {
        for (int i = 0; i < allWritableButtons.Length; i++)
        {
            if (allWritableButtons[i] == goBackButton) continue;
            else allWritableButtons[i].CompletelyBlock(blockState);
        }

        for (int i = 0; i < allWritableTexts.Length; i++)
        {
            if (allWritableTexts[i] == goBackButtonTC) continue;
            else allWritableTexts[i].enabled = !blockState;
        }

        if (goBackButton != null)
        {
            goBackButton.BlockTypContButNotClick(blockState);
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

        if (!Regex.IsMatch(textToCheck, @"^[\p{L}\p{N}]+$"))
        {
            helpText.text = "Name can only contain letters and numbers.";
            return false;
        }

        return true;
    }


    private void OnDestroy()
    {
        if (isTyping && InputHandler.Instance != null)
        {
            InputHandler.Instance.RemoveListener(OnCharacterTyped);
        }
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

        numGames.text = numGames.text.Replace("<value>", state.slot.profile.numGames.ToString());
        numVictories.text = numVictories.text.Replace("<value>", state.slot.profile.gamesWon.ToString());
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