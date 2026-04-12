using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private GameObject statusImagePrefab;
    private Player player;
    private readonly List<Image> addedStatus = new();

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
            if (status.sprite == effectSprite) return; //Cada efecto �nicamente sale una vez
        }
        Image newEffect = Instantiate(statusImagePrefab, this.transform).GetComponent<Image>();
        newEffect.sprite = effectSprite;
        addedStatus.Add(newEffect);
        //Efectos buenos a la izquierda, malos a la derecha:
        if (effect.Definition.EffectPolarityType == EffectPolarityType.Good)
            newEffect.transform.SetAsFirstSibling(); //Por defecto est� en �ltima posici�n
    }

    private void RemoveStatusEffect(StatusEffect effect)
    {
        if (effect.Definition.DurationType == EffectDurationType.Immediate) return;
        Sprite effectSprite = effect.Definition.Image;
        for (int i = 0; i < addedStatus.Count; i++)
        {
            if (addedStatus[i].sprite == effectSprite)
            {
                Image statusToRemove = addedStatus[i];
                addedStatus.Remove(statusToRemove);
                Destroy(statusToRemove.gameObject);
                return;
            }
        }
    }
}
