using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

//Este script entero es un crimen de guerra. Pido perdón.
public class EndMatchAnimationManager : MonoBehaviour
{
    [SerializeField] private float showResultsDelay = 2.0f;
    [SerializeField] private float transitionCanvasNewDist = 1.0f;
    [SerializeField] private float newNear = 0.2f;
    private readonly List<GameObject> grimoires = new();
    private float prevPlaneDistance;

    private void OnDestroy()
    {
        CanvasTransitionManager.OnDissolved -= OnDissolve;
    }

    public void HandleEndMatch(bool isUserWinner, MatchEndReason reason)
    {
        FindFirstObjectByType<CanvasTransitionManager>().FadeOut();
        Invoke(nameof(GoToResults), showResultsDelay);
        if (reason == MatchEndReason.Disconnection) return;
        ConfigureCam(transitionCanvasNewDist);
        HandlePlayer(Player.User, isUserWinner, true);
        HandlePlayer(Player.Enemy, !isUserWinner, false);
        CanvasTransitionManager.OnDissolved += OnDissolve;
    }

    private void HandlePlayer(Player player, bool isWinner, bool setAsFollowTarget)
    {
        HandleCultistModel(player, isWinner, setAsFollowTarget);
        HandleGrimoireModel(player);
        HandleCastingCard(player);
        Destroy(player.gameObject);
    }

    private void HandleCultistModel(Player player, bool isWinner, bool setAsFollowTarget)
    {
        GameObject cultistModel = GetGO(player, ModelType.Cultist);
        cultistModel.transform.SetParent(null);
        string animTrigger = "Victory";
        if (!isWinner) animTrigger = "Defeat";
        cultistModel.GetComponentInChildren<Animator>().SetTrigger(animTrigger);
        if (setAsFollowTarget)
        {
            FindFirstObjectByType<CinemachineCamera>().enabled = false;
            Camera.main.transform.SetParent(Utils.FindChildrenWithTag(cultistModel.transform, "CultistHead"));
        }
    }

    private void HandleGrimoireModel(Player player)
    {
        GameObject grimoireModel = GetGO(player, ModelType.Grimoire);
        grimoires.Add(grimoireModel);
        grimoireModel.transform.SetParent(null);
        Utils.ChangeLayerToHierarchy(grimoireModel.transform, LayerMask.NameToLayer("UI"));
    }

    private void HandleCastingCard(Player player)
    {
        CastingCard card = player.GetComponentInChildren<CastingCard>();
        if (!card) return;
        Transform canvas = card.GetComponentInParent<Canvas>().transform;
        canvas.GetPositionAndRotation(out Vector3 pos, out Quaternion rot);
        canvas.SetParent(null);
        canvas.SetPositionAndRotation(pos, rot);
        card.FadeOut();
    }

    private void ConfigureCam(float planeDistance)
    {
        Canvas c = Utils.FindChildrenWithTag(Camera.main.transform, "TransitionCanvas").
            GetComponent<Canvas>();
        prevPlaneDistance = c.planeDistance;
        c.planeDistance = planeDistance;
        Utils.FindChildrenWithTag(Camera.main.transform, "UICam").
            GetComponent<Camera>().fieldOfView = Camera.main.fieldOfView;
        Camera.main.nearClipPlane = newNear;
    }

    private void GoToResults() =>
       FindFirstObjectByType<NavigationController>().GoTo(Screens.Results, this.gameObject);

    private void OnDissolve()
    {
        foreach (GameObject grimoire in grimoires) grimoire.SetActive(false);
        ConfigureCam(prevPlaneDistance);
        CanvasTransitionManager.OnDissolved -= OnDissolve;
    }

    private GameObject GetGO(Player player, ModelType type)
    {
        return player.GetComponentsInChildren<CultBasedModel>().
            Where(c => c.Type == type).First().gameObject;
    }
}
