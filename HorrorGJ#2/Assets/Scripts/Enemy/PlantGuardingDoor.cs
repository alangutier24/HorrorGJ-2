using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlantGuardingDoor : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] SoundDefinition hurtSfx;
    [SerializeField] SpriteRenderer sr;

    [SerializeField] GameObject door;

    [SerializeField] float healthDecreaseRate = 4f;

    [SerializeField] CharacterMovement player;

    private float hittingLight = 0f;
    private float hurtDuration = 1f;

    public float health = 100f;

    private bool narrative = false;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<CharacterMovement>();
    }


    public float GetLightAmount()   {
            
        if(!player.playerLight.enabled) return 0f;

        Light2D light2D = player.GetComponentInChildren<Light2D>();
        Vector2 point = transform.position;
        
        Vector2 origin = light2D.transform.position;
        Vector2 toPoint = point - origin;
        float dist = toPoint.magnitude;

        float hittingLight;

        if (dist > light2D.pointLightOuterRadius)
        {
            return 0;
        } 

        float angle = Vector2.Angle(light2D.transform.up, toPoint);
        if (angle > light2D.pointLightOuterAngle * 0.5f) 
        {
            return 0;
        }
        hittingLight = 1f - Mathf.InverseLerp(light2D.pointLightInnerRadius, light2D.pointLightOuterRadius, dist);
        return hittingLight;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float lightAmount = GetLightAmount();
            if (lightAmount > 0.5f)
            {
                Debug.Log("Player is in trigger with light on");
                if(!narrative)
                {
                    GameManager.Instance.gameState = GameManager.GameState.Episodes;
                    GameManager.Instance.currentEpisode = GameManager.Episode.Episode11;
                    narrative = true;
                    return;
                }
                hittingLight += Time.deltaTime;
                if (hittingLight >= hurtDuration)
                {
                    AudioManager.Instance.PlaySFX(hurtSfx);
                    hittingLight = 0f;
                    health -= Time.deltaTime * healthDecreaseRate;
                }
                animator.SetBool("IsHurt", true);

            }
            else
            {
             animator.SetBool("IsHurt", false);
            }

            if(health <= 0f)
            {
                Destroy(door);
                Destroy(gameObject);
            }
        }
    }


}
