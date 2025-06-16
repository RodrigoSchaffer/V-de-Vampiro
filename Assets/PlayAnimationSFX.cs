using UnityEngine;

public class PlayAnimationSFX : StateMachineBehaviour
{

    [SerializeField] private soundType sound;
    [SerializeField, Range(0, 1)] private float volume = 1;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.playSFX(sound, volume);
    }

}
