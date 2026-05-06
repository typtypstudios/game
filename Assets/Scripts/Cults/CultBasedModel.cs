using System.Collections;
using System.Runtime.CompilerServices;
using TypTyp.Cults;
using UnityEngine;

public class CultBasedModel : MonoBehaviour
{
    [SerializeField] private GameObject placeholder;
    [SerializeField] private ModelType modelType;
    [SerializeField] private TransitionWithUIType transitionType;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;
    private LayerMask initLayer;
    private int currentCultId = -1; 
    private GameObject currentObject;
    private bool isFixed = false; //Por si se le quiere fijar un culto

    private enum ModelType
    {
        Cultist,
        MenuCultist,
        Grimoire
    }

    private void Awake()
    {
        position = placeholder.transform.localPosition;
        rotation = placeholder.transform.localRotation;
        scale = placeholder.transform.localScale;
        currentObject = placeholder;
        RuntimeVariables.Instance.OnUpdated += UpdateModel;
        if (RuntimeVariables.Instance.IsLoaded) UpdateModel();
        initLayer = gameObject.layer;
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
        string name = currentObject.name;
        Destroy(currentObject);
        GameObject objToCreate = GetObjToCreate();
        currentObject = Instantiate(objToCreate, this.transform);
        currentObject.name = name;
        Utils.ChangeLayerToHierarchy(currentObject.transform, this.gameObject.layer);
        UpdateTransform();
        StartCoroutine(UpdateAnimators());
        CanvasTransitionManager.OnDissolved -= PerformChange;
    }

    private void RestoreLayers()
    {
        Utils.ChangeLayerToHierarchy(this.transform, initLayer);
        CanvasTransitionManager.OnTransitionFinished -= RestoreLayers;
    }

    private void UpdateTransform()
    {
        currentObject.transform.localScale = scale;
        currentObject.transform.SetLocalPositionAndRotation(position, rotation);
    }

    private GameObject GetObjToCreate()
    {
        CultDefinition currentCult = CultRegister.Instance.GetById(currentCultId);
        switch (modelType)
        {
            case ModelType.Cultist:
                return currentCult.CultistModel;
            case ModelType.MenuCultist:
                return currentCult.MenuModel;
            default:
                return currentCult.GrimoireModel;
        }
    }

    IEnumerator UpdateAnimators()
    {
        yield return null;
        Animator[] animators = GetComponentsInChildren<Animator>();
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