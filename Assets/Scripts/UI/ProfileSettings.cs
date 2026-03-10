using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ProfileSettings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI helpText;
    private string defaultUsername = "AverageCultist";
    
    private string currentName = "";
    private bool isTyping = false;
    private WritableButton usernameButton;
    private WritableButton[] allWritableButtons;
    private int minNameLength = 4;
    private int maxNameLength = 15;

    private void Start()
    {
        allWritableButtons = GetComponentsInChildren<Button>().Select(b => b.GetComponent<WritableButton>()).ToArray();
        usernameButton = usernameText.GetComponentInParent<WritableButton>();
        currentName = PlayerPrefs.GetString("Username", defaultUsername);

        if (string.IsNullOrWhiteSpace(currentName))
        {
            currentName = defaultUsername;
        }

        if (currentName.Equals(defaultUsername))
        {
            currentName += "#";
            for (int i = 0; i < 4; i++) currentName += Random.Range(0, 10).ToString();
        }

        usernameButton.OverrideText(currentName);

        if (!PlayerPrefs.HasKey("Username"))
        {
            PlayerPrefs.SetString("Username", currentName);
            PlayerPrefs.Save();
        }
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
    }

    public void ChangeName()
    {
        if (isTyping) return;
        usernameButton.Block = true;
        isTyping = true;
        currentName = "";
        usernameButton.OverrideText(currentName);

        helpText.enabled = true;
        helpText.text = "Press enter to save name";

        // Deshabilitar el resto de botones
        ToggleWritableButtons(true);

        // Usar el listener del input handler
        InputHandler.Instance.AddListener(OnCharacterTyped);
    }

    private void OnCharacterTyped(char c)
    {
        if (!isTyping || currentName.Length >= maxNameLength) return;

        currentName += c;
        usernameButton.OverrideText(currentName);
    }

    private void SubmitName()
    {
        isTyping = false;

        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.RemoveListener(OnCharacterTyped);
        }

        // habilitar el resto de botones
        ToggleWritableButtons(false);
        currentName = currentName.Trim();
        // Comprobar si el usuario es válido y guardar
        if (CheckText(currentName))
        {
            helpText.text = "Name saved!";

            PlayerPrefs.SetString("Username", currentName);
            PlayerPrefs.Save();
        }
        else
        {
            currentName = PlayerPrefs.GetString("Username", defaultUsername);
        }
        usernameButton.OverrideText(currentName);
        usernameButton.Block = false;
    }

    private void ToggleWritableButtons(bool blockState)
    {
        for (int i = 0; i < allWritableButtons.Length; i++)
        {
            allWritableButtons[i].GetComponent<Button>().interactable = !blockState;
            allWritableButtons[i].Block = blockState;
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
        // Seguridad por si el objeto se destruye a mitad de la escritura
        if (isTyping && InputHandler.Instance != null)
        {
            InputHandler.Instance.RemoveListener(OnCharacterTyped);
        }
    }

    public void LinkAccount()
    {
        Debug.Log("Not implemented yet");
    }
}
