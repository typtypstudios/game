using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TypTyp.Cults
{
    [CreateAssetMenu(fileName = "CultDefinition", menuName = "TypTyp/Cults/CultDefinition")]
    public class CultDefinition : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Abbreviation { get; private set; }
        [field: SerializeField] public string[] RankNames { get; private set; }
        [field: SerializeField] public Sprite Image { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }


        public IEnumerable<CardDefinition> GetCards()
        {
            return CardRegister.Instance.RegisteredItems.
                Where(x => x.Cult == this).
                Distinct();
        }

        public string GetRank(int rank)
        {
            var clampedRank = Math.Clamp(rank, 0, RankNames.Length);
            if(clampedRank != rank)
            {
                Debug.LogWarning("The rank is outside bounds. Rank used is clamped");
            }
            return RankNames[clampedRank];
        }
    }
}