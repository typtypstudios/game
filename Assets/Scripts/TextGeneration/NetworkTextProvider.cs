using System;
using TMPro;
using TypTyp;
using TypTyp.TextSystem;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NetworkTextProvider : NetworkBehaviour, ITextProvider
{
    [SerializeField] private TextMeshPro[] texts;
    [SerializeField] TextAsset textSource;
    private static List<string> phrases = new();
    private int textIdx = 0;
    private RitualManager ritualManager;

    private void Awake() => ritualManager = GetComponentInChildren<RitualManager>();

    public override void OnNetworkSpawn()
    {
        if (IsServer) LoadSource();
        if (IsOwner) for (int i = 0; i < texts.Length; i++) RequestNextTextRpc(textIdx++);
    }

    private void LoadSource()
    {
        if (phrases.Count > 0) return;
        List<string> allPhrases = textSource != null
            ? textSource.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()
            : new();
        for (int i = 0; i < Settings.Instance.MaxTextsProvided; i++)
        {
            string phrase = allPhrases[UnityEngine.Random.Range(0, allPhrases.Count)];
            allPhrases.Remove(phrase);
            phrases.Add(phrase);
        }
    }

    public string GetNextText()
    {
        for (int i = 0; i < texts.Length - 1; i++) texts[i].text = texts[i + 1].text;
        texts[texts.Count() - 1].text = string.Empty;
        RequestNextTextRpc(textIdx++);
        return texts[0].text;
    }

    [Rpc(SendTo.Server)]
    private void RequestNextTextRpc(int numCompleted)
    {
        ReceiveTextRpc(numCompleted >= phrases.Count ? string.Empty : phrases[numCompleted]);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ReceiveTextRpc(string text)
    {
        if (Settings.Instance.DottedText) text = text.Replace(" ", "-");
        for(int i = 0; i < texts.Length; i++)
        {
            if (texts[i].text.Equals(string.Empty))
            {
                texts[i].text = text;
                if (i == 0) ritualManager.OriginalText = text;
                break;
            }
        }
    }
}
