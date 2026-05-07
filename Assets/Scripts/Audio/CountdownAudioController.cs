using UnityEngine;

public class CountdownAudioController : MonoBehaviour
{
    private StartGameCanvas startEndCanvas;

    private void Awake()
    {
        startEndCanvas = FindFirstObjectByType<StartGameCanvas>();
    }

    private void OnEnable()
    {
        startEndCanvas.OnCountdownTick += HandleTick;
        startEndCanvas.OnCountdownGo += HandleGo;
    }

    private void OnDisable()
    {
        startEndCanvas.OnCountdownTick -= HandleTick;
        startEndCanvas.OnCountdownGo -= HandleGo;
    }

    private void HandleTick(int second)
    {
        if (second <= 0)
        {
            AudioManager.Instance.PlayCountdown(CountdownSound.None);
            return;
        }
        CountdownSound sound = second % 2 != 0 ? CountdownSound.One : CountdownSound.Two;
        AudioManager.Instance.PlayCountdown(sound);
    }

    private void HandleGo()
    {
        AudioManager.Instance.PlayCountdown(CountdownSound.Go);
    }
}