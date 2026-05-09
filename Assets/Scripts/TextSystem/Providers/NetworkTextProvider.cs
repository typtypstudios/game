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
        [field: SerializeField] public TMP_Text[] Texts { get; private set; }
        [SerializeField] TextAsset textSource;
        private static List<string> phrases = new();
        private System.Random random;
        private int textIdx = 0;
        private bool initialTextsRequested;
        private RitualManager ritualManager; //Referencia circular
        private ITextPipeline textPipeline;
        public event Action OnLineRequested;
        public event Action OnNextText;

        public override void OnNetworkSpawn()
        {
            if (IsServer && IsOwner && random != null) LoadSource();
            if (IsOwner)
            {
                MatchManager.OnMatchStarted += EnableTexts;
            }
        }


        private void Awake()
        {
            ritualManager = GetComponentInChildren<RitualManager>(true);
            textPipeline = GetComponentInChildren<ITextPipeline>(true);
            foreach (var t in Texts)
            {
                t.text = string.Empty;
            }
        }

        public void EnableTexts()
        {
            MatchManager.OnMatchStarted -= EnableTexts;
            if (!IsOwner || initialTextsRequested) return;

            for (int i = 0; i < Texts.Length; i++) RequestNextTextRpc(textIdx++);
            initialTextsRequested = true;
        }

        private void LoadSource()
        {
            if (random == null)
            {
                Debug.LogError("NetworkTextProvider random not configured. Call SetRandom before loading source.");
                return;
            }

            phrases.Clear();
            List<string> allPhrases = textSource != null
                ? textSource.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()
                : new();
            for (int i = 0; i < Settings.Instance.MaxTextsProvided; i++)
            {
                int randomIndex = random.Next(allPhrases.Count);
                string phrase = allPhrases[randomIndex];
                allPhrases.Remove(phrase);
                phrases.Add(phrase.Trim());
            }
        }

        public string GetNextText()
        {
            for (int i = 0; i < Texts.Length - 1; i++) Texts[i].text = Texts[i + 1].text;
            Texts[Texts.Count() - 1].text = string.Empty;
            RequestNextTextRpc(textIdx++);
            //ejecutado en cliente de manera instantanea, esto ejecuta corutina de animacion
            OnNextText?.Invoke();
            return Texts[0].text;
        }

        public string GetText(int index) => default;

        public void SetRandom(System.Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            if (IsServer && IsOwner && phrases.Count == 0)
            {
                LoadSource();
            }
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
            for (int i = 0; i < Texts.Length; i++)
            {
                //Coge el ultimo texto vacio, me parece un mierdon, deberia siempre ponerse el ultimo y ya esta
                if (Texts[i].text.Equals(string.Empty))
                {
                    Texts[i].text = text;
                    if (i == 0) ritualManager.OriginalText = text;
                    break;
                }
            }
            OnLineRequested?.Invoke();
        }
    }
}
