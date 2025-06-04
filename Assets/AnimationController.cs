using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    private bool isPlayingAction = false;
    private string currentState = "";

    public Unit target;
    public int dmg;




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

    public int dealDmg()
    {
        if (target != null)
        {
            int a = target.takeDmg(dmg);
            return a;
        }

        return 0;
    }

}

