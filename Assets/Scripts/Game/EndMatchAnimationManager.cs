using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class EndMatchAnimationManager : MonoBehaviour
{
    public void HandleEndMatch(bool isUserWinner)
    {
        SpawnLocalPlayerCopy(Player.User, isUserWinner, true);
        SpawnLocalPlayerCopy(Player.Enemy, !isUserWinner, false);
        if (TryGetComponent(out Animator anim)) anim.SetTrigger("FadeOut");
    }

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
