using UnityEngine;

public class CameraChange : MonoBehaviour
{

    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject medCam;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            mainCam.SetActive(false);
            medCam.SetActive(true);
            other.GetComponent<CharacterMovement>().medCamIsActive = true;    
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
    }
}
