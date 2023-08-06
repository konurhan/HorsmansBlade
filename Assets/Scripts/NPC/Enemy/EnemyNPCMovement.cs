using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPCMovement : MonoBehaviour
{
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    private SphereCollider sphereCollider;

    public GameObject target;
    public float detectionRadius;
    public float attackRadius;
    public float triggerentryDistance;

    [SerializeField] private float speed;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 smoothDeltaPos;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        sphereCollider = GetComponent<SphereCollider>();

        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    void Update()
    {
        //SynchronizePositionWithRootMotion();
        //Debug.Log("update");
        animator.SetFloat("SpeedZ", agent.velocity.magnitude);
        sphereCollider.radius = detectionRadius;//move this into a method
    }

    private void OnAnimatorMove()
    {
        //Debug.Log("OnAnimatorMove");
        Vector3 rootPosition = animator.rootPosition;
        //rootPosition.y = agent.nextPosition.y;//to make agent enable to change vertical position for instance while climbing
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;//manually updating agent movement
    }

    /*public void SynchronizePositionWithRootMotion()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;
        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPos = Vector2.Lerp(smoothDeltaPos, deltaPosition, smooth);

        velocity = smoothDeltaPos / Time.deltaTime;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance/agent.stoppingDistance);
        }

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.stoppingDistance;
        if (shouldMove)
        {
            animator.SetFloat("SpeedZ",velocity.magnitude);
        }
        else
        {
            animator.SetFloat("SpeedZ", 0);
        }
    }*/

    /*public void CheckForThePlayer()
    {
        
    }*/

    public void Chase()
    {

    }

    public void Attack()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.isTrigger && other.gameObject.tag == "Player")
        {
            if(target == null)
            {
                target = other.gameObject;
                triggerentryDistance = GetDistanceTotarget();
                Debug.Log("found target name is: "+target.name+" with distance: "+triggerentryDistance);
            }
        }
    }

    public bool TargetOutOfRange()//call in chase
    {
        float buffer = 0.2f;
        if((target.transform.position - transform.position).magnitude > triggerentryDistance+buffer || target == null)
        {
            Debug.Log((target.transform.position - transform.position).magnitude);
            target = null;
            return true;
        }
        return false;
    }

    public bool DoesHaveTarget()//call in idle
    {
        return target != null;
    }

    public float GetDistanceTotarget()
    {
        //if (target == null) return -1;//already checking before this method gets called
        
        return (transform.position - target.transform.position).magnitude;
    }

    public void SetDetectionRadiusOnLoad(float radius)//take this value from the save file
    {
        detectionRadius = radius;
    }

    public void StartSpeedUp()
    {
        StartCoroutine(SpeedUp());
    }

    public void StartSlowDown()
    {
        StartCoroutine(SlowDown());
    }

    public IEnumerator SpeedUp()
    {
        Debug.Log("speeding up");
        float speed = animator.GetFloat("SpeedZ");
        Debug.Log("speedZ is: " + speed);
        
        while(speed < 2f)
        {
            speed += 0.01f;
            animator.SetFloat("SpeedZ", speed);
            yield return new WaitForEndOfFrame();
        }
        speed = 2f;
        animator.SetFloat("SpeedZ",speed);
        yield break;
    }

    public IEnumerator SlowDown()
    {
        float speed = animator.GetFloat("SpeedZ");
        while (speed > 0f)
        {
            speed -= 0.01f;
            animator.SetFloat("SpeedZ", speed);
            yield return new WaitForEndOfFrame();
        }
        speed = 0f;
        animator.SetFloat("SpeedZ", speed);
        yield break;
    }

    public void GoInToMovementLayer()//animation event
    {
        animator.ResetTrigger("Sheat");//improper fix: 
        animator.SetLayerWeight(1, 0f);
        animator.SetLayerWeight(2, 0f);
        animator.SetLayerWeight(0, 1f);
    }

}
