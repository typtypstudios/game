using UnityEngine;
using System.Collections.Generic;

public class StatusEffectUIBar : MonoBehaviour
{
    [SerializeField] private GameObject statusImagePrefab;
    [SerializeField] private bool sortByPolarity = false;
    private Player player;
    private readonly List<StatusEffectUI> addedStatus = new();

    public void BindToPlayer(Player player)
    {
        this.player = player;
        if (player)
        {
            player.StatusEffectController.OnEffectApplied.AddListener(AddStatusEffect);
            player.StatusEffectController.OnEffectRemoved.AddListener(RemoveStatusEffect);
        }
    }

    private void OnDestroy()
    {
        if (player)
        {
            player.StatusEffectController.OnEffectApplied.RemoveListener(AddStatusEffect);
            player.StatusEffectController.OnEffectRemoved.RemoveListener(RemoveStatusEffect);
        }
    }

    private void AddStatusEffect(StatusEffect effect)
    {
        if (effect.Definition.DurationType == EffectDurationType.Immediate
            || effect.Definition.DurationType == EffectDurationType.Permanent) return;
        Sprite effectSprite = effect.Definition.Image;
        foreach (var status in addedStatus)
        {
            if (status.Sprite == effectSprite) return; //Cada efecto unicamente sale una vez
        }
        StatusEffectUI newEffect = Instantiate(statusImagePrefab, this.transform).GetComponent<StatusEffectUI>();
        newEffect.Sprite = effectSprite;
        addedStatus.Add(newEffect);
        //Efectos buenos a la izquierda, malos a la derecha:
        if (!sortByPolarity || effect.Definition.EffectPolarityType == EffectPolarityType.Good)
            newEffect.transform.SetAsFirstSibling(); //Por defecto esta en ultima posicion
    }

    private void RemoveStatusEffect(StatusEffect effect)
    {
        if (effect.Definition.DurationType == EffectDurationType.Immediate) return;
        Sprite effectSprite = effect.Definition.Image;
        for (int i = 0; i < addedStatus.Count; i++)
        {
            if (addedStatus[i].Sprite == effectSprite)
            {
                StatusEffectUI statusToRemove = addedStatus[i];
                addedStatus.Remove(statusToRemove);
                statusToRemove.Destroy();
                return;
            }
        }
    }
}