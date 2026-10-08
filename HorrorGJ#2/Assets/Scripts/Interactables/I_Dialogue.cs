using UnityEngine;

public class I_Dialogue : MonoBehaviour, IInteractable
{
    [SerializeField]
    private DialogueObject dialogueObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            player.Interactable = this;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && other.TryGetComponent(out CharacterMovement player))
        {
            if(player.Interactable is I_Dialogue i_dialogue && i_dialogue == this)
            {
                player.Interactable = null;
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Interact from Interacable");
        DialogueManager.Instance.dialoguePanelUI.ShowDialogue(dialogueObject);
    }

}
