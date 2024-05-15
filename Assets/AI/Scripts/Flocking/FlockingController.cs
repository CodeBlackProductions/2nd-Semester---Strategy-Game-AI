using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class FlockingController : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float alignmentWeight = 1;
    [SerializeField] private float cohesionWeight = 1;
    [SerializeField] private float separationWeight = 1;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    [Space(10)]
    [Header("Flock Bounds")]
    [SerializeField] private Transform boundsCenter;
    [SerializeField] private float boundsSize;
    [SerializeField] private float boundsWeight = 1;

    [Space(10)]
    [Header("Ground Avoidance")]
    [SerializeField] private Transform ground;
    [SerializeField] private float minGroundDistance;
    [SerializeField] private float avoidanceWeight = 1;

    [Space(10)]
    [Header("Random Wander")]
    [SerializeField] private float wanderWeight = 1;
    [SerializeField] private float timerMin = 0;
    [SerializeField] private float timerMax = 10;

    private List<FlockingController> neighbours = new List<FlockingController>();

    private Vector3 direction;
    private Vector3 targetDirection;
    private Vector3 wanderDir;

    private float timer = 0;
    public Vector3 Direction { get => direction; }

    public void Start()
    {
        direction = transform.forward;
    }

    public void Alignment()
    {
        Vector3 alignment = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            alignment += neighbours[i].Direction;
        }
        alignment /= neighbours.Count;
        alignment = alignment.normalized;
        targetDirection += alignment * (speed * alignmentWeight);
    }

    public void Cohesion()
    {
        Vector3 cohesion = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            cohesion += neighbours[i].transform.position;
        }
        cohesion /= neighbours.Count;
        cohesion = cohesion - transform.position;
        cohesion = cohesion.normalized;
        targetDirection += cohesion * (speed * cohesionWeight);
    }

    public void Separation()
    {
        Vector3 separation = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            Vector3 dir = neighbours[i].transform.position - transform.position;
            dir /= dir.sqrMagnitude;
            separation += dir;
        }
        separation /= neighbours.Count;
        separation = separation.normalized * -1;
        targetDirection += separation * (speed * separationWeight);
    }

    public void Bounds()
    {
        Vector3 bounds = Vector3.zero;
        Vector3 dir = boundsCenter.position - transform.position;
        bounds += (dir / boundsSize);
        bounds = bounds.normalized;
        targetDirection += bounds * (speed * boundsWeight);
    }

    public void GroundAvoidance()
    {
        Vector3 avoidance = Vector3.zero;
       
        if (transform.position.y < minGroundDistance)
        {
            Vector3 dir = new Vector3(transform.forward.x,ground.position.y - transform.position.y, transform.forward.z);
            avoidance += dir;
            avoidance = avoidance.normalized * -1;
            targetDirection += avoidance * (speed * avoidanceWeight);
        }
        
    }

    public void Wander() 
    {
        if (timer <= 0)
        {
            float randomX = Random.Range(boundsCenter.position.x - boundsSize, boundsCenter.position.x + boundsSize);
            float randomZ = Random.Range(boundsCenter.position.z - boundsSize, boundsCenter.position.z + boundsSize);

            wanderDir = new Vector3(randomX, transform.position.y, randomZ) - transform.position;
            wanderDir = wanderDir.normalized;

            targetDirection += wanderDir * (speed * wanderWeight);

            timer = Random.Range(timerMin, timerMax);
        }
        else
        {
            targetDirection += wanderDir * (speed * wanderWeight);
            timer -= Time.deltaTime;
        }
    }

    public void Update()
    {
        targetDirection = transform.forward;

        Alignment();
        Cohesion();
        Separation();
        Bounds();
        GroundAvoidance();
        Wander();

        Vector3 dif = (targetDirection - direction) * Time.deltaTime;
        direction = direction + dif;
        direction = direction.normalized;

        transform.forward += direction * rotationSpeed * Time.deltaTime;
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void OnTriggerEnter(Collider other)
    {
        FlockingController temp = other.GetComponent<FlockingController>();
        if (temp != null)
        {
            neighbours.Add(temp);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        FlockingController temp = other.GetComponent<FlockingController>();
        if (temp != null)
        {
            neighbours.Remove(temp);
        }
    }
}