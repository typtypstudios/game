using UnityEngine;

public class DefeatSoundTrigger : MonoBehaviour
{
    public void PlayDefeatSound()
    {
        AudioManager.Instance.PlayGame(GameSound.Defeat);
    }
}
