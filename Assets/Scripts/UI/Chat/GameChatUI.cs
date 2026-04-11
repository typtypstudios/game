using UnityEngine;
using Unity.Collections;
using TypTyp;

public class GameChatUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private BubbleMaxWidth userBubble;
    [SerializeField] private CanvasGroup userChatGroup;

    [SerializeField] private BubbleMaxWidth enemyBubble;
    [SerializeField] private CanvasGroup enemyChatGroup;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDelay = 2.0f;
    [SerializeField] private float fadeSpeed = 3.0f;

    private float userIdleTimer;
    private float enemyIdleTimer;

    private string lastUserText = "";
    private string lastEnemyText = "";

    private void Awake()
    {
        ClearBubble(userBubble, userChatGroup);
        ClearBubble(enemyBubble, enemyChatGroup);
        lastUserText = "";
        lastEnemyText = "";
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
        ProcessFading(userBubble, userChatGroup, ref userIdleTimer);
        ProcessFading(enemyBubble, enemyChatGroup, ref enemyIdleTimer);
    }

    private void UpdateEnemyText(FixedString64Bytes previous, FixedString64Bytes current)
    {
        string s = current.ToString();
        if (s == lastEnemyText) return;
        lastEnemyText = s;
        ApplyNewText(enemyBubble, enemyChatGroup, s, ref enemyIdleTimer);
    }

    private void UpdateUserTextLocal(string current)
    {
        if (current == lastUserText) return;
        lastUserText = current;
        ApplyNewText(userBubble, userChatGroup, current, ref userIdleTimer);
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

    private void ApplyNewText(BubbleMaxWidth bubble, CanvasGroup group, string newText, ref float timer)
    {
        if (bubble == null || group == null) return;

        bubble.SetText(newText);

        if (bubble.IsEmpty)
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

    private void ProcessFading(BubbleMaxWidth bubble, CanvasGroup group, ref float timer)
    {
        if (bubble == null || group == null) return;
        if (bubble.IsEmpty || group.alpha <= 0f) return;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            group.alpha = Mathf.MoveTowards(group.alpha, 0f, Time.deltaTime * fadeSpeed);
        }
    }

    private void ClearBubble(BubbleMaxWidth bubble, CanvasGroup group)
    {
        if (bubble != null) bubble.Clear();
        if (group != null) group.alpha = 0f;
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