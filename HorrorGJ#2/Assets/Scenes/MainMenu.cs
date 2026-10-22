using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    
    [SerializeField] Animator animator;

    public void StartGame()
    {
        //animator.enabled = true;  
        GameManager.Instance.gameState = GameManager.GameState.NarrativeScreen;
        SceneManager.LoadScene("Game");    
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
