using UnityEngine;
using UnityEngine.Rendering.Universal;

public class I_FlashLight : MonoBehaviour, IInteractable
{
  [SerializeField] private Light2D light;
  [SerializeField] private GameObject highlight;

    private bool hasInteracted = false;
    private CharacterMovement playerInRange;

    void Awake()
    {
        if (light != null)
            light.enabled = false;

        if (highlight != null)
            highlight.SetActive(false);
    }   
    public bool CanInteract() => !hasInteracted;

    public bool Interact()
    {
        if (hasInteracted) return false;

        hasInteracted = true;

        if (light != null)
            light.enabled = true;

        if (playerInRange != null)
        {
            playerInRange.hasLight = true;
            Destroy(gameObject);
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            player.Interactable = this;
            playerInRange = player;
            highlight.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            highlight.SetActive(false);
            if (player.Interactable == this)
                player.Interactable = null;

            if (playerInRange == player)
                playerInRange = null;
        }
    }

}
