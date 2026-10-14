using UnityEngine;

public class I_FlashLight : MonoBehaviour, IInteractable
{
    private bool isEnabled = true;
    private bool hasInteracted = false;
    public void Interact()
    {
        if(!hasInteracted)
        {
            hasInteracted = true;
            isEnabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            player.Interactable = this;
            if(!hasInteracted)
                player.hasLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            Destroy(gameObject);
        }
    }

}
