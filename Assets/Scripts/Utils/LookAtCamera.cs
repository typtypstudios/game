using System.Collections;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private ConfigurationType type;
    [SerializeField] private bool invert;
    private Transform cam;

    void Awake()
    {
        cam = Camera.main.transform;
        if (type == ConfigurationType.Awake)
        {
            UpdateLookAt();
            this.enabled = false;
        }
        else if (type == ConfigurationType.OnUIConfig)
            StartCoroutine(UpdateAfterCoroutine());
    }

    private void OnDestroy()
    {
        if (type == ConfigurationType.OnUIConfig)
            GameUIConfigurator.OnUIConfigurated -= UpdateLookAt;
    }

    private void Update()
    {
        UpdateLookAt();   
    }

    private void UpdateLookAt()
    {
        transform.forward = (invert ? -1 : 1) * transform.position - cam.position;
    }

    IEnumerator UpdateAfterCoroutine()
    {
        yield return null;
        GameUIConfigurator.OnUIConfigurated += UpdateLookAt;
        this.enabled = false;
    }

    private enum ConfigurationType{ Awake, OnUIConfig, Always };
}
