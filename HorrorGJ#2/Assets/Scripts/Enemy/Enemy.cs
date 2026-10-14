
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Transform player;
    CharacterMovement pcm;
    [SerializeField] public float lightThreshhold = 0.7f;
    [SerializeField] public float chaseDistance = 15f;
    [SerializeField] public float attackDistance = 3f;
    private Rigidbody2D rb;
    private UnityEngine.AI.NavMeshAgent agent;

    private Light2D[] lights;

    private bool active = true; // true now for testing

    void Start()
    {
        pcm = GameObject.FindWithTag("Player").GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        lights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        System.Array.Resize(ref lights, lights.Length + 1);
        lights[lights.Length - 1] = pcm.light;
        foreach (var l in lights) Debug.Log($"Luz encontrada: {l.name}", l);
        Debug.Log(lights.Length);
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.velocity = new Vector2(1, 0.5f);

    }
    //Knows the players location at all times
    //moves closer when in dark


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
            // 1 inside inner radius, fading to 0 at outer radius
            hittingLight = 1f - Mathf.InverseLerp(light2D.pointLightInnerRadius, light2D.pointLightOuterRadius, dist);
        }
        return hittingLight;
    }

    public bool InDark()
    {
        if(!pcm.SwitchLight)
            return true;

        return GetLightAmount(transform.position) <= lightThreshhold;
    }

    public bool RandomPointAway(
    Vector2 origin, Vector2 threat,
    float minDist, float maxDist, float coneAngle,
    out Vector2 result, int attempts = 10, float sampleRadius = 30f)
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
    
    public void RanFarEnough()
    {
        Debug.Log("Running");
        Debug.Log("Enemy: " + transform.position + " Point: " + point);
        if(!agent.pathPending && agent.remainingDistance <= 3f)
        {
            runAway = false;
            Debug.Log("Ran far enough");
        }
    }

    public void RunAway()
    {
        if(runAway) return;

        bool returnPoint = RandomPointAway(transform.position, player.position, 8f, 15f, 45, out point);
        agent.SetDestination(point);
        runAway = true;
    }

    void Update()
    {
        //if activated chase player
        //if near light, stop
        //if touching light, health decreases, spawn point resets if health decreases too much
        //if(!active) return;

        //if(InDark())
        //{
        //    MoveToPlayer();
        //}
        //else
        //{
        //    AgentStop();
        //}

    }
}
