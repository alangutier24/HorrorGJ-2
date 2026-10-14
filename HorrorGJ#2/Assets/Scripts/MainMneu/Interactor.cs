using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{

    [SerializeField] LayerMask interactMask;   
    
    [SerializeField]
    private Vector3 _rayCastOffset = new Vector3(0, 0f, 0);


    private void Update()
    {
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            if(DoInteractionTest(out IInteractable interactable))
            {
                if(interactable.CanInteract())
                {
                    interactable.Interact();
                }
            }
        }
    }
    
    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;

        Vector3 center = transform.position + _rayCastOffset;
        Collider2D hit = Physics2D.OverlapCircle(center, 1.25f, interactMask);

        if (hit != null)
        {
            interactable = hit.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
                return true;
        }

        return false;
    }

}
