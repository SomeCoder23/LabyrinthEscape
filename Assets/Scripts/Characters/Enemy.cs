using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public AudioClip dieAudio;
    public float chaseSpeedModifier;
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
    private float searchDistance, chaseSpeed, speed;
    Rigidbody rb;
    bool dead;
    AudioSource audio;

    private void Awake()
    {
        Vector3 pos = transform.position;
        pos.y = 10f;
        transform.position = pos;
    }

    void Start()
    {
        audio = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        setSpeed(GameUIManager.instance.getLevel());

        StartCoroutine(SearchRoutine());
    }

    void Update()
    {
        if (dead)
            return;

        if (isPlayerVisible())
        {
            agent.speed = chaseSpeed;
            isChasing = true;
            audio.volume = 1;
            if(agent.enabled)
                agent.SetDestination(player.position);
            animator.SetFloat("Speed", chaseSpeed);
        }
        else if (isChasing)
        {
            agent.speed = speed;
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

        animator.SetBool("isDead", true);
        dead = true;
        agent.enabled = false;
        audio.PlayOneShot(dieAudio);
        Invoke("SelfDestruct", 1.5f);
    }

    void SelfDestruct()
    {
        GameUIManager.instance.AddPoint(5);
        Destroy(this.gameObject);
    }

    void setSpeed(int level)
    {
        speed = agent.speed;
        chaseSpeed = speed + chaseSpeedModifier * level;
        chaseDistance += level / 2.5f;
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