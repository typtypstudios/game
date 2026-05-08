using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

//Este script entero es un crimen de guerra. Pido perdón.
public class EndMatchAnimationManager : MonoBehaviour
{
    [SerializeField] private float showResultsDelay = 2.0f;
    [SerializeField] private float transitionCanvasNewDist = 1.0f;
    private readonly List<GameObject> grimoires = new();

    private void OnDestroy()
    {
        CanvasTransitionManager.OnDissolved -= DisableGrimoires;
    }

    public void HandleEndMatch(bool isUserWinner)
    {
        FindFirstObjectByType<CanvasTransitionManager>().FadeOut();
        Invoke(nameof(GoToResults), showResultsDelay);
        ConfigureCam();
        HandlePlayer(Player.User, isUserWinner, true);
        HandlePlayer(Player.Enemy, !isUserWinner, false);
    }

    private void ConfigureCam()
    {
        Utils.FindChildrenWithTag(Camera.main.transform, "TransitionCanvas").
            GetComponent<Canvas>().planeDistance = transitionCanvasNewDist;
        Utils.FindChildrenWithTag(Camera.main.transform, "UICam").
            GetComponent<Camera>().fieldOfView = Camera.main.fieldOfView;
    }

    private void GoToResults() =>
       FindFirstObjectByType<NavigationController>().GoTo(Screens.Results, this.gameObject);

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
        if (!isWinner) cultistModel.GetComponentInChildren<Animator>().SetTrigger("Defeat");
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
        CanvasTransitionManager.OnDissolved += DisableGrimoires;
    }

    private void DisableGrimoires()
    {
        foreach(GameObject grimoire in grimoires) grimoire.SetActive(false);
        CanvasTransitionManager.OnDissolved -= DisableGrimoires;
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

    private GameObject GetGO(Player player, ModelType type)
    {
        return player.GetComponentsInChildren<CultBasedModel>().
            Where(c => c.Type == type).First().gameObject;
    }
}
