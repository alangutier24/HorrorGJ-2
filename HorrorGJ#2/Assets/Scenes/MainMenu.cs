using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    [SerializeField] Animator animator;

    public void StartGame()
    {
        //animator.enabled = true;  
        GameManager.Instance.gameState = GameManager.GameState.Episodes;
        GameManager.Instance.currentEpisode = GameManager.Episode.Episode1;
        SceneManager.LoadScene("Game");    
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
