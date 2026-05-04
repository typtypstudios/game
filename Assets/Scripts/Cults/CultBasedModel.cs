using TypTyp.Cults;
using UnityEngine;

public class CultBasedModel : MonoBehaviour
{
    [SerializeField] private GameObject placeholder;
    [SerializeField] private ModelType modelType;
    private GameObject currentObject;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;

    private enum ModelType
    {
        Cultist,
        MenuCultist,
        Grimoire
    }

    private void Awake()
    {
        position = placeholder.transform.position;
        rotation = placeholder.transform.rotation;
        scale = placeholder.transform.localScale;
        currentObject = placeholder;
        RuntimeVariables.Instance.OnUpdated += UpdateModel;
    }

    private void OnDestroy()
    {
        if(RuntimeVariables.Instance) 
            RuntimeVariables.Instance.OnUpdated -= UpdateModel;
    }

    private void UpdateModel()
    {
        Destroy(currentObject);
        GameObject objToCreate = GetObjToCreate();
        currentObject = Instantiate(objToCreate, this.transform);
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        currentObject.transform.localScale = scale;
        currentObject.transform.SetPositionAndRotation(position, rotation);
    }

    private GameObject GetObjToCreate()
    {
        CultDefinition currentCult = RuntimeVariables.Instance.CurrentCult;
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
}