using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

// Este script entero es un crimen de guerra. Pido perdón.
public class EndMatchAnimationManager : MonoBehaviour
{
    [SerializeField] private float showResultsDelay = 2.0f;
    [SerializeField] private float transitionCanvasNewDist = 1.0f;
    [SerializeField] private float newNear = 0.2f;
    private readonly List<GameObject> grimoires = new();
    private float prevPlaneDistance;

    /* Pues ahora el código lo que hace es cachear los modelos antes de que 
     * se eliminen porque si no, las animaicones nunca llegaban a ejecutarse
     * en casos de desconexión, u ocurrían cosas raras.
     * 
     * Los cachea al comienzo de la partida y hace el detach de los gameobjects
     * justo antes de ser eliminados.
     * 
     * También, se pasa la lógica de la animación del NetworkAnimator al animator 
     * local para que siempre funcione.
     */
    private GameObject userCultist, userGrimoire, enemyCultist, enemyGrimoire;
    private CastingCard userCard, enemyCard;
    private bool modelsCached;

    private void OnDestroy()
    {
        CanvasTransitionManager.OnDissolved -= OnDissolve;
    }

    public void CachePlayerModels()
    {
        if (modelsCached) return;
        modelsCached = true;

        // Guardar los cosos al principio por si las moscas
        userCultist = GetGO(Player.User, ModelType.Cultist);
        userGrimoire = GetGO(Player.User, ModelType.Grimoire);
        userCard = Player.User?.GetComponentInChildren<CastingCard>();

        enemyCultist = GetGO(Player.Enemy, ModelType.Cultist);
        enemyGrimoire = GetGO(Player.Enemy, ModelType.Grimoire);
        enemyCard = Player.Enemy?.GetComponentInChildren<CastingCard>();
    }

    public void DetachCachedModels()
    {
        Detach(userCultist);
        Detach(userGrimoire);
        if (userCard) HandleCastingCard(userCard);

        Detach(enemyCultist);
        Detach(enemyGrimoire);
        if (enemyCard) HandleCastingCard(enemyCard);
    }

    public void HandleEndMatch(bool isUserWinner, MatchEndReason reason)
    {
        if(isUserWinner) AudioManager.Instance.PlayGame(GameSound.Victory);

        FindFirstObjectByType<CanvasTransitionManager>().FadeOut();
        Invoke(nameof(GoToResults), showResultsDelay);
        // if (reason == MatchEndReason.Disconnection) return;
        ConfigureCam(transitionCanvasNewDist);

        // Usamos los cosos cacheados
        HandleSide(userCultist, userGrimoire, userCard, isUserWinner, true);
        HandleSide(enemyCultist, enemyGrimoire, enemyCard, !isUserWinner, false);

        if (Player.User) Destroy(Player.User.gameObject);
        if (Player.Enemy) Destroy(Player.Enemy.gameObject);

        CanvasTransitionManager.OnDissolved += OnDissolve;
    }

    private void HandleSide(GameObject cultist, GameObject grimoire, CastingCard card, bool isWinner, bool setAsFollowTarget)
    {
        if (cultist) HandleCultistModel(cultist, isWinner, setAsFollowTarget);
        if (grimoire) HandleGrimoireModel(grimoire);
        if (card) HandleCastingCard(card);
    }

    private void HandleCultistModel(GameObject cultist, bool isWinner, bool setAsFollowTarget)
    {
        Detach(cultist);
        cultist.SetActive(true);

        // Quitar el NetworkAnimator y encender el local
        if (cultist.TryGetComponent<Unity.Netcode.Components.NetworkAnimator>(out var netAnim))
            Destroy(netAnim);

        Animator anim = cultist.GetComponentInChildren<Animator>();
        if (anim)
        {
            anim.enabled = true;
            anim.SetTrigger(isWinner ? "Victory" : "Defeat");
        }

        if (setAsFollowTarget)
        {
            FindFirstObjectByType<CinemachineCamera>().enabled = false;
            Camera.main.transform.SetParent(Utils.FindChildrenWithTag(cultist.transform, "CultistHead"));
        }
    }

    private void HandleGrimoireModel(GameObject grimoire)
    {
        if (grimoires.Contains(grimoire)) return;
        grimoires.Add(grimoire);
        Detach(grimoire);
        Utils.ChangeLayerToHierarchy(grimoire.transform, LayerMask.NameToLayer("UI"));
    }

    private void HandleCastingCard(CastingCard card)
    {
        Transform canvas = card.GetComponentInParent<Canvas>().transform;
        if (canvas.parent)
        {
            canvas.GetPositionAndRotation(out Vector3 pos, out Quaternion rot);
            canvas.SetParent(null);
            canvas.SetPositionAndRotation(pos, rot);
        }
        card.FadeOut();
    }

    private void Detach(GameObject go)
    {
        if (go && go.transform.parent) go.transform.SetParent(null, true);
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
        foreach (GameObject grimoire in grimoires)
            if (grimoire) grimoire.SetActive(false);

        ConfigureCam(prevPlaneDistance);
        CanvasTransitionManager.OnDissolved -= OnDissolve;
    }

    private GameObject GetGO(Player player, ModelType type)
    {
        // Devuelve null si no encuentra nada
        return player?.GetComponentsInChildren<CultBasedModel>().FirstOrDefault(c => c.Type == type)?.gameObject;
    }
}