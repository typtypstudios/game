using UnityEngine;

public class CorruptionAudioPlayer : MonoBehaviour
{
    private float damageDelay = 0.0f;
    private float healDelay = 0.0f;

    private CorruptionGainManager corruptionManager;

    public void Bind(CorruptionGainManager manager)
    {
        if (corruptionManager != null)
        {
            corruptionManager.OnDamageReceived -= HandleDamage;
            corruptionManager.OnHealReceived -= HandleHeal;
        }

        corruptionManager = manager;
        if (corruptionManager == null) return;

        corruptionManager.OnDamageReceived += HandleDamage;
        corruptionManager.OnHealReceived += HandleHeal;
    }

    private void OnDisable()
    {
        if (corruptionManager != null)
        {
            corruptionManager.OnDamageReceived -= HandleDamage;
            corruptionManager.OnHealReceived -= HandleHeal;
        }
    }

    private void HandleDamage()
    {
        AudioManager.Instance.PlayGame(GameSound.Damage, damageDelay);
    }

    private void HandleHeal()
    {
        AudioManager.Instance.PlayGame(GameSound.Heal, healDelay);
    }
}