using UnityEngine;

public class MatchSceneMusicController : MonoBehaviour
{
    [SerializeField] private MusicTrack track;

    private void OnEnable()
    {
        MatchManager.OnCountdownStarted += HandleCountdownStarted;
    }

    private void OnDisable()
    {
        MatchManager.OnCountdownStarted -= HandleCountdownStarted;
    }

    private void HandleCountdownStarted()
    {
        AudioManager.Instance.PlayMusic(track);
    }
}