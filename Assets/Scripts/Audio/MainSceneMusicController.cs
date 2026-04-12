using UnityEngine;

public class MainSceneMusicController : MonoBehaviour
{
    [SerializeField] private MusicTrack track;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(track);
    }
}