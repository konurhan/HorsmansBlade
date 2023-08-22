using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPCMovement : MonoBehaviour
{
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    private SphereCollider sphereCollider;

    private EnemyNPCAttack attack;

    public GameObject target;
    public float detectionRadius;
    public float closingInRadius;
    public float attackRadius;
    public float surroundingRadius;
    public float triggerentryDistance;
    public bool isMovementRoationalEnabled;
    public bool isClosingIn;

    public Vector3 destinationOffset;

    [SerializeField] private float speed;
    //[SerializeField] private Vector2 velocity;
    //[SerializeField] private Vector2 smoothDeltaPos;
    public Vector3 angularVelocity;
    public Vector3 velocity;
    private Rigidbody rb;

    private float lastYEulerAngle;
    //private float agentRotationY;
    //private bool agentIsManullayRotating;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();

        isMovementRoationalEnabled = true;
        //agentIsManullayRotating = false;
        isClosingIn = false;
    }

    void Start()
    {
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
        
        attack = GetComponent<EnemyNPCAttack>();
        lastYEulerAngle = transform.rotation.eulerAngles.y;
        //agentRotationY = transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        if (agent.hasPath)
        {
            animator.SetFloat("SpeedZ", agent.velocity.magnitude);//refactor this so that animation speed, agent speed and character speeds all together work in a conforming way
        }
        sphereCollider.radius = detectionRadius;//move this into a method
        angularVelocity = rb.angularVelocity;
        velocity = rb.velocity;
        PlayFootAnimOnRotation();
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;//to make agent enable to change vertical position for instance while climbing
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
        if(GetDistanceTotarget() > triggerentryDistance+buffer || target == null)
        { 
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
        return ((target.transform.position) - transform.position).magnitude;
    }

    public float GetDistanceToSurroundingDestination()
    {
        Vector3 destination = target.transform.position + target.transform.forward * destinationOffset.z + target.transform.right * destinationOffset.x;
        return ((destination) - transform.position).magnitude;
    }

    public IEnumerator StrafeTowardsTarget()//used for closing in to target 
    {
        float lerpSpeed = 5f;
        GameObject cachedTarget = target;
        isClosingIn = true;
        float speedZ = 0.5f;
        float speedX = UnityEngine.Random.Range(-1f, 1f);
        speedX = speedX < 0 ? 1f : 1f;//animations are not enough, walking left and approaching doesn't work together
        animator.SetFloat("SpeedZ", speedZ);
        animator.SetFloat("SpeedX", speedX);
        while (true)
        {
            Quaternion lookRotation = Quaternion.LookRotation(cachedTarget.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime*lerpSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator StrafeAroundTheTarget(float duration)
    {
        attack.attacking = true;
        GameObject cachedTarget = target;
        animator.SetFloat("SpeedZ", 0);
        float speedX = UnityEngine.Random.Range(-1f, 1f);
        speedX = speedX < 0 ? -1f : 1f;

        Func<float, float, float> speedModel;
        if (speedX > 0)
        {
            speedModel = NegativeParabolicBy2PeakSpeed;
        }
        else
        {
            speedModel = ParabolicBy2PeakSpeed;
        }

        float strafeDur = duration;
        float time = 0f;
        while (time < strafeDur)
        {
            Quaternion lookRotation = Quaternion.LookRotation(cachedTarget.transform.position - transform.position,Vector3.up);
            transform.rotation = lookRotation;
            float xAxPos = ((time / (strafeDur/2)) - 1);//progress of speedup/slow-downi fitted on [-1,1] interval on the x axis
            float curSpeedX = speedModel(speedX, xAxPos);

            animator.SetFloat("SpeedX", curSpeedX);
            time += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        animator.SetFloat("SpeedZ", 0);
        animator.SetFloat("SpeedX", 0);
        attack.attacking = false;
    }

    public void OnStopStrafing()
    {
        animator.SetFloat("SpeedZ", 0);
        animator.SetFloat("SpeedX", 0);
        isClosingIn = false;
    }

    public float NegativeParabolicBy2PeakSpeed(float peakSpeed, float xAxPos)//peakSpeed - x^2, called if peakSpeed > 0
    {
        float xAxPosClamped = Mathf.Sqrt(peakSpeed) * xAxPos;
        return peakSpeed - Mathf.Pow(xAxPosClamped, 2);
    }

    public float ParabolicBy2PeakSpeed(float peakSpeed, float xAxPos)//peakSpeed + x^2, call if peakSpeed < 0
    {
        float xAxPosClamped = Mathf.Sqrt(-peakSpeed) * xAxPos;
        return peakSpeed + Mathf.Pow(xAxPosClamped, 2);
    }

    public IEnumerator AgentRotateTowardsTarget()
    {
        float angle = attack.GetAngleToTarget();
        float rotated = 0f;
        float speed = 1.5f;
        int sign = 0;
        agent.updateRotation = false;
        if (angle < 0)//turning left
        {
            sign = -1;
        }
        else if (angle > 1)//turning right
        {
            sign = 1;
        }
        while (rotated < angle*sign)
        {
            float step = sign * angle * Time.deltaTime * speed;
            transform.Rotate(new Vector3(0, sign * step, 0));
            rotated += step;
            yield return new WaitForEndOfFrame();
        }
        //agentRotationY += angle;
        agent.updateRotation = true;
    }

    public void PlayFootAnimOnRotation()
    {
        if (agent.updateRotation || animator.GetFloat("SpeedZ") > 0.01f)//animation will only be played while the npc rotation is being manually updated
        {
            animator.SetBool("TurnLeft", false);
            animator.SetBool("TurnRight", false);
            return;
        }

        float newAngle = transform.rotation.eulerAngles.y;

        if (Mathf.Abs(newAngle - lastYEulerAngle) < 0.05f)
        {
            animator.SetBool("TurnLeft", false);
            animator.SetBool("TurnRight", false);
            return;
        }

        if (newAngle > lastYEulerAngle)
        {
            //Debug.Log("turning right");
            animator.SetBool("TurnLeft", false);
            if (!animator.GetBool("TurnRight")) animator.SetBool("TurnRight", true);
        }
        else if (newAngle < lastYEulerAngle)
        {
            //Debug.Log("turning left");
            animator.SetBool("TurnRight", false);
            if (!animator.GetBool("TurnLeft")) animator.SetBool("TurnLeft", true);
        }
        lastYEulerAngle = newAngle;
    }

    public void SetDetectionRadiusOnLoad(float radius)//take this value from the save file
    {
        detectionRadius = radius;
    }

    /*public void StartSpeedUp()
    {
        StartCoroutine(SpeedUp());
    }

    public void StartSlowDown()
    {
        StartCoroutine(SlowDown());
    }

    public IEnumerator SpeedUpStrafe()
    {
        float speed = 0;
        while (speed < 1f)//speed up will take 0.8 seconds
        {
            speed += 0.01f;
            animator.SetFloat("SpeedX", speed);
            yield return new WaitForSeconds(0.008f);
        }
        speed = 1f;
        animator.SetFloat("SpeedX", speed);
        StartCoroutine(SlowDownStrafe());
        yield break;
    }

    public IEnumerator SlowDownStrafe()
    {
        float speed = 1f;
        while (speed > 0)//slow down will take 0.8 seconds
        {
            speed -= 0.01f;
            animator.SetFloat("SpeedX", speed);
            yield return new WaitForSeconds(0.008f);
        }
        speed = 0f;
        animator.SetFloat("SpeedX", speed);
        yield break;
    }

    public IEnumerator SpeedUp()
    {
        Debug.Log("speeding up");
        float speed = animator.GetFloat("SpeedZ");
        Debug.Log("speedZ is: " + speed);

        while (speed < 2f)
        {
            speed += 0.01f;
            animator.SetFloat("SpeedZ", speed);
            yield return new WaitForEndOfFrame();
        }
        speed = 2f;
        animator.SetFloat("SpeedZ", speed);
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
    }*/

    #region Animation Events
    public void GoInToMovementLayer()
    {
        animator.ResetTrigger("Sheat");//improper fix: 
        animator.SetLayerWeight(1, 0f);
        animator.SetLayerWeight(2, 0f);
        animator.SetLayerWeight(0, 1f);
    }

    public void EnableRotationalMovement()
    {
        isMovementRoationalEnabled = true;
    }

    public void DisableRotationalMovement()
    {
        isMovementRoationalEnabled = false;
    }

    #endregion
}
