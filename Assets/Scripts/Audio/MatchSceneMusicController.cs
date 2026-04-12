using System;
using UnityEngine;

public class MatchSceneMusicController : MonoBehaviour
{
    [SerializeField] private MusicTrack matchTrack;
    [SerializeField] private MusicTrack endGameTrack;

    private void OnEnable()
    {
        MatchManager.OnCountdownStarted += HandleCountdownStarted;
        MatchManager.OnMatchEnded += HandleMatchEnded;
    }
    private void OnDisable()
    {
        MatchManager.OnCountdownStarted -= HandleCountdownStarted;
        MatchManager.OnMatchEnded -= HandleMatchEnded;

    }

    private void HandleMatchEnded()
    {
        // Se va la musica de Geometry Dash :(
        //AudioManager.Instance.PlayMusic(endGameTrack);
    }


    private void HandleCountdownStarted()
    {
        AudioManager.Instance.PlayMusic(matchTrack);
    }
}