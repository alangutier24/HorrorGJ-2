using UnityEngine;

public class DriveStartingDialogue : MonoBehaviour, IInteractable
{

    private bool hasStarted = false;
    private bool used = false;
    public Animator eanim;
    public CharacterController player;

    public bool CanInteract() => !used;

    void Start()
    {
        player.enabled = false;
    }

    public bool Interact()
    {
        if (!hasStarted)   { Open();  return true; }

        return true;
    }

    void Open()
    {
        hasStarted = true;
    }

}
