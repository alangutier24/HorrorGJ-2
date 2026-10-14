using UnityEngine;
using UnityEngine.Rendering.Universal;

public class I_FlashLight : MonoBehaviour, IInteractable
{
  [SerializeField] private Light2D light;

    private bool hasInteracted = false;
    private CharacterMovement playerInRange;

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
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            if (player.Interactable == this)
                player.Interactable = null;

            if (playerInRange == player)
                playerInRange = null;
        }
    }

}
