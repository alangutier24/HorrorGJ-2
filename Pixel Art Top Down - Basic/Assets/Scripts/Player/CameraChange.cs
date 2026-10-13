using UnityEngine;

public class CameraChange : MonoBehaviour
{

    [SerializeField] private AnimEventReceiver receiver;  
    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject medCam;

    [SerializeField] private GameObject canvas;

    [SerializeField] private Animator anim;

    void OnEnable()
    {
        receiver.OnSwitchCamera += SwitchCamera; 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            canvas.SetActive(true);
            anim.SetBool("SetActive", true);
            other.GetComponent<CharacterMovement>().medCamIsActive = true; 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
    }

    public void SwitchCamera()
    {
        anim.SetBool("FadeIn", false);
        anim.SetBool("FadeOut", false);
        mainCam.SetActive(false);
        medCam.SetActive(true);
    }
}
