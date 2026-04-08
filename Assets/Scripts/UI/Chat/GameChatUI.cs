using UnityEngine;
using TMPro;
using Unity.Collections;
using TypTyp;

public class GameChatUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text userSpellingText;
    [SerializeField] private CanvasGroup userChatGroup;

    [SerializeField] private TMP_Text enemySpellingText;
    [SerializeField] private CanvasGroup enemyChatGroup;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDelay = 2.0f;
    [SerializeField] private float fadeSpeed = 3.0f;

    private float userIdleTimer;
    private float enemyIdleTimer;

    private void Awake()
    {
        ClearTextAndAlpha(userSpellingText, userChatGroup);
        ClearTextAndAlpha(enemySpellingText, enemyChatGroup);
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
        ProcessFading(userSpellingText, userChatGroup, ref userIdleTimer);
        ProcessFading(enemySpellingText, enemyChatGroup, ref enemyIdleTimer);
    }

    private void UpdateEnemyText(FixedString64Bytes previous, FixedString64Bytes current)
    {
        ApplyNewText(enemySpellingText, enemyChatGroup, current.ToString(), ref enemyIdleTimer);
    }

    private void UpdateUserTextLocal(string current)
    {
        ApplyNewText(userSpellingText, userChatGroup, current, ref userIdleTimer);
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
                Debug.Log("Todo el chat sin filtro");
                Player.User.SpellTypingTracker.OnLocalRawTextChanged += UpdateUserTextLocal;
                UpdateUserTextLocal(Player.User.SpellTypingTracker.CurrentLocalRawText);
            }
            else
            {
                Debug.Log("Solo chat filtrado");
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

    private void ApplyNewText(TMP_Text textComponent, CanvasGroup group, string newText, ref float timer)
    {
        textComponent.text = newText;

        if (string.IsNullOrEmpty(newText))
        {
            group.alpha = 0f;
            timer = 0f;
        }
        else
        {
            group.alpha = 1f;
            timer = fadeDelay;
        }
    }

    private void ProcessFading(TMP_Text textComponent, CanvasGroup group, ref float timer)
    {
        if (string.IsNullOrEmpty(textComponent.text) || group.alpha <= 0f) return;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            group.alpha = Mathf.MoveTowards(group.alpha, 0f, Time.deltaTime * fadeSpeed);
        }
    }

    private void ClearTextAndAlpha(TMP_Text textComponent, CanvasGroup group)
    {
        textComponent.text = "";
        group.alpha = 0f;
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