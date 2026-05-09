using TMPro;
using TypTyp.Cults;
using UnityEngine;

public class PlayerLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerRank;
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        if(player) player.OnPlayerConfigurated += SetContent;
    }

    private void OnDestroy()
    {
        if (player) player.OnPlayerConfigurated -= SetContent;
    }

    private void SetContent()
    {
        playerRank.text = $"Rank {player.CultRank}:\n" +
            $"{CultRegister.Instance.GetById(player.CultID).RankNames[player.CultRank]}";
        playerName.text = player.Name;
    }
}
