using Unity.Cinemachine;
using UnityEngine;

public class EndMatchAnimationManager : MonoBehaviour
{
    [SerializeField] private float showResultsDelay = 2.0f;

    public void HandleEndMatch(bool isUserWinner)
    {
        FindFirstObjectByType<CanvasTransitionManager>().FadeOut();
        Invoke(nameof(GoToResults), showResultsDelay);
        SpawnLocalPlayerCopy(Player.User, isUserWinner, true);
        SpawnLocalPlayerCopy(Player.Enemy, !isUserWinner, false);
        if (TryGetComponent(out Animator anim)) anim.SetTrigger("FadeOut");
    }

    private void GoToResults() =>
       FindFirstObjectByType<NavigationController>().GoTo(Screens.Results, this.gameObject);

    private void SpawnLocalPlayerCopy(Player player, bool isWinner, bool setAsFollowTarget)
    {
        GameObject cultistModel = Utils.FindChildrenWithTag(player.transform, "CultistModel").gameObject;
        GameObject copy = Instantiate(cultistModel, cultistModel.transform.position, cultistModel.transform.rotation);
        if (copy.TryGetComponent(out CultBasedModel cbm)) Destroy(cbm);
        Animator originalAnimator = cultistModel.GetComponent<Animator>();
        Animator copyAnimator = copy.GetComponent<Animator>();
        AnimatorStateInfo stateInfo = originalAnimator.GetCurrentAnimatorStateInfo(0);
        copyAnimator.Play(stateInfo.fullPathHash, 0, stateInfo.normalizedTime);
        if (!isWinner) copyAnimator.SetTrigger("Defeat");
        if (setAsFollowTarget)
        {
            FindFirstObjectByType<CinemachineCamera>().enabled = false;
            Camera.main.transform.SetParent(Utils.FindChildrenWithTag(copy.transform, "CultistHead"));
        }
        player.gameObject.SetActive(false);
    }
}
