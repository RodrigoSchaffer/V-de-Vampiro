using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


public enum soundType
{
    Attack,
    Hurt,
    Death
}
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static AudioManager instance;

    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioSource Ost;
    private AudioClip ost;

    public void Awake()
    {
        instance = this;
    }

    public static void playSFX(soundType sound, float volume = 1)
    {
        instance.sound.PlayOneShot(instance.soundList[(int)sound], volume);
    }

    public void playOst()
    {
        Ost.clip = ost;
        Ost.Play();
    }

    public static void pauseOst(int i)
    {
        switch (i)
        {
            case 0:
                AudioListener.pause = true;
                break;
            case 1:
                AudioListener.pause = false;
                break;
        }
        
    }

}
