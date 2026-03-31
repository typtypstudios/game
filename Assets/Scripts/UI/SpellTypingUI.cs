using UnityEngine;
using TMPro;
using Unity.Collections;
using TypTyp;

public class SpellTypingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text userSpellingText;
    [SerializeField] private TMP_Text enemySpellingText;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDelay = 2.0f;
    [SerializeField] private float fadeSpeed = 3.0f;

    private float userIdleTimer;
    private float enemyIdleTimer;

    private void Awake()
    {
        ClearTextAndAlpha(userSpellingText);
        ClearTextAndAlpha(enemySpellingText);
    }

    private void OnEnable()
    {
        GameUIConfigurator.OnUIConfigurated += SetupListeners;
    }

    private void OnDisable()
    {
        GameUIConfigurator.OnUIConfigurated -= SetupListeners;
        RemoveListeners();
    }

    private void Update()
    {
        ProcessFading(userSpellingText, ref userIdleTimer);
        ProcessFading(enemySpellingText, ref enemyIdleTimer);
    }

    private void UpdateEnemyText(FixedString32Bytes previous, FixedString32Bytes current)
    {
        ApplyNewText(enemySpellingText, current.ToString(), ref enemyIdleTimer);
    }

    private void UpdateUserTextLocal(string current)
    {
        ApplyNewText(userSpellingText, current, ref userIdleTimer);
    }

    #region addListeners

    private void SetupListeners()
    {
        AddUserListener();
        AddEnemyListener();
    }

    private void AddUserListener()
    {
        if (Player.User != null && Player.User.SpellTypingTracker != null)
        {
            if (Settings.Instance.ChatActive)
            {
                Player.User.SpellTypingTracker.OnLocalRawTextChanged += UpdateUserTextLocal;
                UpdateUserTextLocal(Player.User.SpellTypingTracker.CurrentLocalRawText);
            }
            else
            {
                Player.User.SpellTypingTracker.OnLocalFilteredTextChanged += UpdateUserTextLocal;
                UpdateUserTextLocal(Player.User.SpellTypingTracker.CurrentLocalFilteredText);
            }
        }
    }

    private void AddEnemyListener()
    {
        if (Player.Enemy != null && Player.Enemy.SpellTypingTracker != null)
        {
            bool iWantRaw = Settings.Instance.ChatActive;
            bool enemyAllowsRaw = Player.Enemy.SpellTypingTracker.AllowRawChat.Value;

            if (iWantRaw && enemyAllowsRaw)
            {
                Player.Enemy.SpellTypingTracker.RawText.OnValueChanged += UpdateEnemyText;
                UpdateEnemyText("", Player.Enemy.SpellTypingTracker.RawText.Value);
            }
            else
            {
                Player.Enemy.SpellTypingTracker.FilteredText.OnValueChanged += UpdateEnemyText;
                UpdateEnemyText("", Player.Enemy.SpellTypingTracker.FilteredText.Value);
            }

            Player.Enemy.SpellTypingTracker.AllowRawChat.OnValueChanged += OnEnemySettingChanged;
        }
    }

    private void OnEnemySettingChanged(bool previous, bool current)
    {
        RemoveEnemyListener();
        AddEnemyListener();
    }

    #endregion

    #region text
    private void ApplyNewText(TMP_Text textComponent, string newText, ref float timer)
    {
        textComponent.text = newText;

        if (string.IsNullOrEmpty(newText))
        {
            SetTextAlpha(textComponent, 0f);
            timer = 0f;
        }
        else
        {
            SetTextAlpha(textComponent, 1f);
            timer = fadeDelay;
        }
    }

    private void ProcessFading(TMP_Text textComponent, ref float timer)
    {
        if (string.IsNullOrEmpty(textComponent.text) || textComponent.color.a <= 0f) return;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            Color c = textComponent.color;
            c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * fadeSpeed);
            textComponent.color = c;
        }
    }

    private void ClearTextAndAlpha(TMP_Text textComponent)
    {
        textComponent.text = "";
        SetTextAlpha(textComponent, 0f);
    }

    private void SetTextAlpha(TMP_Text textComponent, float alpha)
    {
        Color c = textComponent.color;
        c.a = alpha;
        textComponent.color = c;
    }
    #endregion

    #region removeListeners
    private void RemoveListeners()
    {
        RemoveUserListener();
        RemoveEnemyListener();
    }

    private void RemoveUserListener()
    {
        if (Player.User != null && Player.User.SpellTypingTracker != null)
        {
            Player.User.SpellTypingTracker.OnLocalRawTextChanged -= UpdateUserTextLocal;
            Player.User.SpellTypingTracker.OnLocalFilteredTextChanged -= UpdateUserTextLocal;
        }
    }

    private void RemoveEnemyListener()
    {
        if (Player.Enemy != null && Player.Enemy.SpellTypingTracker != null)
        {
            Player.Enemy.SpellTypingTracker.FilteredText.OnValueChanged -= UpdateEnemyText;
            Player.Enemy.SpellTypingTracker.RawText.OnValueChanged -= UpdateEnemyText;
            Player.Enemy.SpellTypingTracker.AllowRawChat.OnValueChanged -= OnEnemySettingChanged;
        }
    }
    #endregion
}