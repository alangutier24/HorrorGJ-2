using UnityEngine;

public class I_Notes : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject dialogueCanvas;

    [SerializeField] private GameObject highLight;

    private bool isEnabled = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            player.Interactable = this;
            highLight.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            if(player.Interactable == (IInteractable)this)
            {
                player.Interactable = null;
                highLight.SetActive(false);
            }
        }
    }

    public bool CanInteract() => isEnabled;

    public bool Interact()
    {
        dialogueCanvas.SetActive(true);
        GameManager.Instance.gameState = GameManager.GameState.Episodes;
        GameManager.Instance.currentEpisode = GameManager.Episode.Episode4;

        Destroy(gameObject);

        return true;
    }
}
