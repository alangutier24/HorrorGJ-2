using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HurtPlayerVolume : MonoBehaviour
{
    [SerializeField] private float pushRate;
    [SerializeField] Light2D lightFlicker;

    public void Start()
    {
        lightFlicker = gameObject.GetComponent<Light2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && lightFlicker.intensity != 0)
        {
            CharacterMovement player = collision.GetComponent<CharacterMovement>();
            if(Random.value < pushRate)
                player.RecieveDamage();
         }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            CharacterMovement player = collision.GetComponent<CharacterMovement>();
            player.ResetSpeed();
        }
    }
}
