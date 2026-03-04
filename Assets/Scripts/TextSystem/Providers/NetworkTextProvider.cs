using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TypTyp.TextSystem
{
    public class NetworkTextProvider : NetworkBehaviour, ITextProvider
    {
        [SerializeField] private TMP_Text[] texts;
        [SerializeField] TextAsset textSource;
        private static List<string> phrases = new();
        private int textIdx = 0;
        private RitualManager ritualManager; //Referencia circular
        private ITextPipeline textPipeline;

        public override void OnNetworkSpawn()
        {
            if (IsServer) LoadSource();
            if (IsOwner) for (int i = 0; i < texts.Length; i++) RequestNextTextRpc(textIdx++);
        }


        private void Awake()
        {
            ritualManager = GetComponentInChildren<RitualManager>();
            textPipeline = GetComponentInChildren<ITextPipeline>();
            foreach (var t in texts)
            {
                t.text = string.Empty;
                t.gameObject.SetActive(false);
            }
        }

        public void SetTextsActive(bool value)
        {
            foreach (var t in texts) t.gameObject.SetActive(value);
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
                phrases.Add(phrase.Trim());
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
            var text = numCompleted >= phrases.Count ? string.Empty : phrases[numCompleted];
            if(textPipeline != null)
            {
                text = textPipeline.ProcessText(text);
            }
            ReceiveTextRpc(text);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ReceiveTextRpc(string text)
        {
            if (Settings.Instance.ShowSpaces) text = text.Replace(" ", "-");
            for (int i = 0; i < texts.Length; i++)
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
}
