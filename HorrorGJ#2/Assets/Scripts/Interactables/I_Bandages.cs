using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class I_Bandages : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject dialogueCanvas;

    [SerializeField] private GameObject highLight;

    [SerializeField] private GameObject spotLight;

    [SerializeField] private CharacterMovement player;

    private bool isEnabled = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out player))
        {
            player.Interactable = this;
            highLight.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out player))
        {
            if(player.Interactable == (IInteractable)this)
            {
                player.Interactable = null;
                highLight.SetActive(false);
                spotLight.SetActive(true);
            }
            if(!isEnabled)   
            {
                Destroy(gameObject);
            }

        }
    }

    public bool CanInteract() => isEnabled;

    public bool Interact()
    {
        dialogueCanvas.SetActive(true);
        GameManager.Instance.gameState = GameManager.GameState.Episodes;
        GameManager.Instance.currentEpisode = GameManager.Episode.Episode2;
        spotLight.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        highLight.SetActive(false);
        player.bleeding = false;

        return true;
    }

}
