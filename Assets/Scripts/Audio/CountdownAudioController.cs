using UnityEngine;

public class CountdownAudioController : MonoBehaviour
{
    [SerializeField] private StartEndCanvas startEndCanvas;

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
        CountdownSound sound = second switch
        {
            3 => CountdownSound.Three,
            2 => CountdownSound.Two,
            1 => CountdownSound.One,
            _ => CountdownSound.None
        };
        AudioManager.Instance.PlayCountdown(sound);
    }

    private void HandleGo()
    {
        AudioManager.Instance.PlayCountdown(CountdownSound.Go);
    }
}