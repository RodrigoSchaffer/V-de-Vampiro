using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneManager : MonoBehaviour
{

    [SerializeField] private string battleSceneName;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject menuPanel;


    public void playGameButton()
    {
        SceneManager.LoadScene(battleSceneName);
    }

    public void settingsButton()
    {
        settingsPanel.SetActive(true);
        menuPanel.SetActive(false);
        
    }

    public void backToMenuButton()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void exitButton()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
