using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public bool isPlayingAction = false;
    private string currentState = "";




    void Update()
    {
        if (isPlayingAction)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(currentState) && stateInfo.normalizedTime >= 1f)
            {
                animator.Play("Idle");
                isPlayingAction = false;
                currentState = "Idle";
            }
        }
    }

    public void PlayAction(string actionName)
    {
        if (isPlayingAction) return;

        animator.Play(actionName);
        currentState = actionName;
        isPlayingAction = true;
    }

    public bool IsPlayingAction()
    {
        return isPlayingAction;
    }

    

}

