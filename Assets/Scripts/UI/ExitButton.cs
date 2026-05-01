using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Transform[] posiblePositions;
    [SerializeField] private Transform startPosition;
    private Transform currentTransform;

    private void Awake()
    {
        transform.position = startPosition.position;
        currentTransform = startPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Transform[] posibleDestinations = posiblePositions.Where(p => p != currentTransform).ToArray();
        int randIdx = Random.Range(0, posibleDestinations.Length);
        transform.position = posibleDestinations[randIdx].position;
        currentTransform = posibleDestinations[randIdx];
    }
}
