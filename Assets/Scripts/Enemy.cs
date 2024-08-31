using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float searchRadius = 20f;
    public float chaseDistance = 10f;
    public Vector2 searchTimeInterval;
    public float rotationSpeed = 2f;
    public float fieldOfViewAngle = 45;
    public LayerMask obstacleMask;
    public float agentRecoveryTime = 0.7f;
    public float searchVolume = 0.5f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isChasing = false;
    private Vector3 searchDestination;
    private float searchDistance;
    Rigidbody rb;
    bool dead;
    AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        StartCoroutine(SearchRoutine());
    }

    void Update()
    {

        if (isPlayerVisible())
        {
            isChasing = true;
            audio.volume = 1;
            if(agent.enabled)
                agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.speed * 1.5f);
        }
        else if (isChasing)
        {
            isChasing = false;
            StartCoroutine(SearchRoutine());
        }
    }

    IEnumerator SearchRoutine()
    {
        float waitTime;

        while (!isChasing)
        {
            searchDestination = GetRandomPos();
            agent.SetDestination(searchDestination);
            audio.volume = 1;
            animator.SetFloat("Speed", agent.speed);
            if (!agent.enabled)
                StopCoroutine(SearchRoutine());

            while (agent.remainingDistance > agent.stoppingDistance)
                yield return null;

            audio.volume = searchVolume;
            animator.SetFloat("Speed", rotationSpeed);
            yield return RotateAndSearch();

            waitTime = Random.Range(searchTimeInterval.x, searchTimeInterval.y);
            yield return new WaitForSeconds(waitTime);
        }
    }

    bool isPlayerVisible()
    {
        if (player == null) return false;

        Vector3 playerDirection = (player.position - transform.position);
        searchDistance = isChasing ? chaseDistance * 2 : chaseDistance;
        if (playerDirection.magnitude <= searchDistance)
        {
            float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, playerDirection.normalized);

            //divides by two because the FOV is centered by the forward direction of enemy
            if (angleBetweenEnemyAndPlayer < fieldOfViewAngle / 2f)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, player.position, out hit, obstacleMask))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }

    Vector3 GetRandomPos()
    {
        Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, searchRadius, 1))
        {
            return hit.position;
        }

        return transform.position; 
    }

    IEnumerator RotateAndSearch()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 360, 0);
        float rotationProgress = 0;

        while (rotationProgress < 1)
        {    
            rotationProgress += Time.deltaTime * rotationSpeed;
            transform.RotateAround(transform.position, Vector3.up, Quaternion.Slerp(startRotation, endRotation, rotationProgress).y);
            yield return null;
        }

    }

    public void Push(Vector3 force)
    {
        agent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
        StartCoroutine(RecoverAgent());
    }

    private IEnumerator RecoverAgent()
    {
        yield return new WaitForSeconds(agentRecoveryTime);
        rb.isKinematic = true;
        agent.enabled = true;
    }

    public void Die()
    {
        if (dead) 
            return;

        dead = true;
        Debug.Log(name + "WAS KILLED");
        agent.enabled = false;
        StartCoroutine(ShutDown());
    }

    IEnumerator ShutDown()
    {
        while(audio.volume > 0)
        {
            audio.volume -= 0.1f;
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, searchRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.yellow;
        float stepAngle = fieldOfViewAngle / 12;
        for (int i = 0; i <= 12; i++)
        {
            float currentAngle = -fieldOfViewAngle / 2f + stepAngle * i;
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Gizmos.DrawRay(transform.position, rayDirection * chaseDistance);
        }

    }
}