
using System.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Transform player;
    CharacterMovement pcm;
    [SerializeField] public float lightThreshhold = 0.7f;
    [SerializeField] public float chaseDistance = 15f;
    [SerializeField] public float attackDistance = 3f;
    [SerializeField] public float stalkDistance = 7f;

    [SerializeField] public float minDist = 30f;
    [SerializeField] public float maxDist = 90f;
    [SerializeField] private Animator animator;
    [Tooltip("Vertical scale for 2:1 iso tiles. 0.5 keeps ground speed equal in every direction and makes diagonals follow tile edges.")]
    [SerializeField] float isoYScale = 0.5f;
    readonly int directionHashTag = Animator.StringToHash("EnemyDirection");
    private Rigidbody2D rb;
    private UnityEngine.AI.NavMeshAgent agent;

    private Light2D[] lights;

    void Start()
    {
        pcm = GameObject.FindWithTag("Player").GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();

        lights = pcm.GetComponentsInChildren<Light2D>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.velocity = new Vector2(10f, 5f);
    }

    public void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    public void AgentStop()
    {
        agent.isStopped = true;
        agent.velocity = Vector2.zero;
    }
  
    public float GetLightAmount(Vector2 point)
    {
        if(lights.Length == 0) return 0;
        if(pcm.playerLight == null) return 0;
        float hittingLight = 0;
        foreach(Light2D light2D in lights)
        {       
            Vector2 origin = light2D.transform.position;
            Vector2 toPoint = point - origin;
            float dist = toPoint.magnitude;

            if (dist > light2D.pointLightOuterRadius)
            {
                hittingLight = 0f;
                continue;
            } 

            float angle = Vector2.Angle(light2D.transform.up, toPoint);
            if (angle > light2D.pointLightOuterAngle * 0.5f) 
            {
                hittingLight = 0f;
                continue;
            }
            hittingLight = 1f - Mathf.InverseLerp(light2D.pointLightInnerRadius, light2D.pointLightOuterRadius, dist);
        }
        return hittingLight;
    }

    public bool InDark()
    {
        if(!pcm.lightOn)
            return true;

        return GetLightAmount(transform.position) <= lightThreshhold;
    }

    public bool RandomPointAway(
    Vector2 origin, Vector2 threat,
    float minDist, float maxDist, float coneAngle,
    out Vector2 result, int attempts = 10, float sampleRadius = 90f)
    {
        Vector2 away = origin - threat;
        if (away.sqrMagnitude < 0.0001f) away = Random.insideUnitCircle;
        away.Normalize();
 
        var path = new UnityEngine.AI.NavMeshPath();
 
        for (int i = 0; i < attempts; i++)
        {
            float angle = Random.Range(-coneAngle, coneAngle) * 0.5f;
            Vector2 dir = Quaternion.Euler(0f, 0f, angle) * away;
            Vector2 candidate = origin + dir * Random.Range(minDist, maxDist);
 
            if (!UnityEngine.AI.NavMesh.SamplePosition(candidate, out UnityEngine.AI.NavMeshHit hit, sampleRadius, UnityEngine.AI.NavMesh.AllAreas))
                continue;
 
            Vector2 p = hit.position;
 
            // SamplePosition can snap the point back towards the threat; reject that
            if (Vector2.Dot(p - origin, away) <= 0f) continue;
 
            // Discard points on disconnected NavMesh islands
            if (!UnityEngine.AI.NavMesh.CalculatePath(origin, p, UnityEngine.AI.NavMesh.AllAreas, path) ||
                path.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
                continue;
 
            result = p;
            return true;
        }
 
        result = origin;
        return false;
    }

    public bool runAway = false;
    Vector2 point;
    
    public bool RanFarEnough()
    {
        if(!agent.pathPending && agent.remainingDistance <= 3f)
        {
           return true;
        }
        return false;
    }

    public void RunAway()
    {
        if(runAway) return;
        Debug.Log("Running away from player.");
        agent.isStopped = true;
        agent.ResetPath();
        bool returnPoint = RandomPointAway(transform.position, player.position, minDist, maxDist, 45, out point);
        agent.SetDestination(point);
        runAway = true;
    }

    public void SetFacing()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        int FacingIndex = ((Mathf.RoundToInt(angle / 45f) % 8) + 8) % 8;
 
        animator.SetFloat(directionHashTag, FacingIndex);
    }

    public void Stalk()
    {
        agent.isStopped = false;
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 target = (Vector2)player.position - direction * (stalkDistance - 3f);
        agent.SetDestination(target);
    }

    public void EnemyReset()
    {
        if(agent.isStopped)
        {
            agent.isStopped = false;
        }
        agent.ResetPath();
    }

    public bool ReachedEdgeOfPlayer ()
    {
        if(!agent.pathPending && agent.remainingDistance <= stalkDistance)
        {
            return true;
        }
        return false;
    }
    
    public bool PlayerInAttackDistance()
    {
        if(!agent.pathPending && agent.remainingDistance <= attackDistance)
        {
            return true;
        }
        return false;
    }

    public bool LightOn()
    {
        return pcm.playerLight.enabled;
    }

}
