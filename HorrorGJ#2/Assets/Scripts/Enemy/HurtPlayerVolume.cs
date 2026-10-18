using UnityEngine;

public class HurtPlayerVolume : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player entered hurt volume");
            CharacterMovement player = collision.GetComponent<CharacterMovement>();
            player.RecieveDamage();
         }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player exited hurt volume");
            CharacterMovement player = collision.GetComponent<CharacterMovement>();
            player.ResetSpeed();
        }
    }
}
