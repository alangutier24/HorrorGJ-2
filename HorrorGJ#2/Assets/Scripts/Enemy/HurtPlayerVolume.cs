using UnityEngine;

public class HurtPlayerVolume : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            CharacterMovement player = collision.GetComponent<CharacterMovement>();
            if(Random.value < 0.5f)
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
