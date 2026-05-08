using System.Collections;
using System.Collections.Generic;
using TypTyp.Cults;
using UnityEngine;
using UnityEngine.Rendering;

public class CultBasedModel : MonoBehaviour
{
    [SerializeField] private GameObject placeholder;
    [SerializeField] private ModelType modelType;
    [SerializeField] private TransitionWithUIType transitionType;
    private LayerMask initLayer;
    private int currentCultId = -1; 
    private bool isFixed = false; //Por si se le quiere fijar un culto
    private readonly List<GameObject> models = new();
    private GameObject currentActiveObject;
    public ModelType Type => modelType;

    private void Start()
    {
        LoadModels();
        initLayer = gameObject.layer;
        currentActiveObject = placeholder;
        RuntimeVariables.Instance.OnUpdated += UpdateModel;
        if (RuntimeVariables.Instance.IsLoaded) UpdateModel();
    }

    private void LoadModels()
    {
        placeholder.transform.GetLocalPositionAndRotation(out Vector3 position, out Quaternion rotation);
        Vector3 scale = placeholder.transform.localScale;
        string name = placeholder.name;
        for(int i = 0; i < CultRegister.Instance.Count; i++)
        {
            CultDefinition cult = CultRegister.Instance.GetById(i);
            GameObject cultModel = Instantiate(GetCultObject(cult), this.transform);
            cultModel.name = name;
            cultModel.transform.localScale = scale;
            cultModel.transform.SetLocalPositionAndRotation(position, rotation);
            cultModel.SetActive(false);
            models.Add(cultModel);
        }
    }

    private void OnDestroy()
    {
        if(RuntimeVariables.Instance) 
            RuntimeVariables.Instance.OnUpdated -= UpdateModel;
    }

    public void FixCult(int cultId)
    {
        currentCultId = cultId;
        isFixed = true;
        UpdateModel();
        RuntimeVariables.Instance.OnUpdated -= UpdateModel;
    }

    private bool isFirstTime = true;
    private void UpdateModel()
    {
        if(!isFixed)
        {
            int prevCultId = currentCultId;
            currentCultId = RuntimeVariables.Instance.CurrentCultID;
            if (currentCultId == prevCultId) return;
        }
        if (transitionType == TransitionWithUIType.Never || 
            (transitionType == TransitionWithUIType.AlwaysExceptFirst && isFirstTime)) PerformChange();
        else
        {
            Utils.ChangeLayerToHierarchy(this.transform, LayerMask.NameToLayer("UI"));
            CanvasTransitionManager.OnDissolved += PerformChange;
            CanvasTransitionManager.OnTransitionFinished += RestoreLayers;
        }
        isFirstTime = false;
    }

    private void PerformChange()
    {
        currentActiveObject.SetActive(false);
        currentActiveObject = models[currentCultId];
        currentActiveObject.SetActive(true);
        currentActiveObject.transform.SetAsFirstSibling();
        Utils.ChangeLayerToHierarchy(currentActiveObject.transform, this.gameObject.layer);
        StartCoroutine(UpdateAnimators());
        CanvasTransitionManager.OnDissolved -= PerformChange;
    }

    private void RestoreLayers()
    {
        Utils.ChangeLayerToHierarchy(this.transform, initLayer);
        CanvasTransitionManager.OnTransitionFinished -= RestoreLayers;
    }

    private GameObject GetCultObject(CultDefinition cult)
    {
        switch (modelType)
        {
            case ModelType.Cultist:
                return cult.CultistModel;
            case ModelType.MenuCultist:
                return cult.MenuModel;
            default:
                return cult.GrimoireModel;
        }
    }

    IEnumerator UpdateAnimators()
    {
        yield return null;
        Animator[] animators = GetComponentsInChildren<Animator>(true);
        foreach (var anim in animators)
            anim.Rebind();
    }

    private enum TransitionWithUIType
    {
        Never,
        Always,
        AlwaysExceptFirst
    }
}

public enum ModelType
{
    Cultist,
    MenuCultist,
    Grimoire
}