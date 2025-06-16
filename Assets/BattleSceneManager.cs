using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSceneManager : MonoBehaviour
{
    [SerializeField] private string menuSceneName;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private AudioListener sounds;



    public void settingsButton()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void restartGame()
    {
        string battleSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(battleSceneName);
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    public void backToGameButton()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1;

    }
    public void backToMenuButton()
    {
        SceneManager.LoadScene(menuSceneName);
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            AudioManager.pauseOst(0);
        }
        else
        {
            AudioManager.pauseOst(1);
        }
    }

}
