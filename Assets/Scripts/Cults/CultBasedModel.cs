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
    private int fixedCultId = -1; //Por si se le quiere fijar un culto

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
    }

    private void OnDestroy()
    {
        if(RuntimeVariables.Instance) 
            RuntimeVariables.Instance.OnUpdated -= UpdateModel;
    }

    public void FixCult(int cultId)
    {
        fixedCultId = cultId;
        UpdateModel();
    }

    private void UpdateModel()
    {
        string name = currentObject.name;
        Destroy(currentObject);
        GameObject objToCreate = GetObjToCreate();
        currentObject = Instantiate(objToCreate, this.transform);
        currentObject.name = name;
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        currentObject.transform.localScale = scale;
        currentObject.transform.SetLocalPositionAndRotation(position, rotation);
    }

    private GameObject GetObjToCreate()
    {
        CultDefinition currentCult = fixedCultId == -1 ? RuntimeVariables.Instance.CurrentCult : 
            CultRegister.Instance.GetById(fixedCultId);
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