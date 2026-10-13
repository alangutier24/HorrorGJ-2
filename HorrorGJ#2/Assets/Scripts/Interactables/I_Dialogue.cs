using UnityEngine;

public class I_Dialogue : MonoBehaviour, IInteractable
{
    [SerializeField]
    private DialogueObject dialogueObject; 

    [SerializeField]
    private GameObject key;

    private bool isEnabled = true;
    private bool hasInteracted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            player.Interactable = this;
            if(!hasInteracted)
                key.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            if(player.Interactable is I_Dialogue i_dialogue && i_dialogue == this)
            {
                player.Interactable = null;
                key.SetActive(false);
            }
        }
    }

    public bool CanInteract() => isEnabled;

    public bool Interact()
    {
        if(!hasInteracted)
        {
            DialogueManager.Instance.dialoguePanelUI.ShowDialogue(dialogueObject);
            hasInteracted = true;
            key.SetActive(false);
        }
        return true;
    }

    public void OnFocusGained()
    {
        throw new System.NotImplementedException();
    }

    public void OnFocusLost()
    {
        throw new System.NotImplementedException();
    }

}
