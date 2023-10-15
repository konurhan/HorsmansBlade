using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPCMovement : MonoBehaviour
{
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    private SphereCollider sphereCollider;
    private Rigidbody rb;
    private EnemyController controller;
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
    public SurroundingDest surroundingDestination;

    [SerializeField] private float speed;
    public Vector3 angularVelocity;
    public Vector3 velocity;

    public Transform WaypointGraphParent;
    public List<Transform> waypoints;
    
    private float lastYEulerAngle;

    private Coroutine changeSpeedX;
    private Coroutine changeSpeedZ;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<EnemyController>();

        isMovementRoationalEnabled = true;
        isClosingIn = false;

        for (int i = 0; i < WaypointGraphParent.childCount; i++)
        {
            waypoints.Add(WaypointGraphParent.GetChild(i).gameObject.transform);
        }
    }

    void Start()
    {
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
        
        attack = GetComponent<EnemyNPCAttack>();
        lastYEulerAngle = transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        if (agent.hasPath && controller.stateMachine.currentstate != controller.patrollingState)
        {
            animator.SetFloat("SpeedZ", agent.velocity.magnitude);//refactor this so that animation speed, agent speed and character speeds all together work in a conforming way
        }else if (agent.hasPath && controller.stateMachine.currentstate == controller.patrollingState)
        {
            float clamped = agent.velocity.magnitude;
            if (clamped > 1) clamped = 1;
            animator.SetFloat("SpeedZ", clamped);
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

    public void AssignClosestEmptyDestination()
    {
        //find closest empty point to close-in
        //surroundingDestination = null;
        int index = -1;
        for (int i = 0; i < target.GetComponent<PlayerController>().relativePos.Count; i++)
        {
            SurroundingDest surDest = target.GetComponent<PlayerController>().relativePos[i];
            if (surDest.occupantNPC != null) continue;
            if (surroundingDestination == null)
            {
                surroundingDestination = surDest;
                surroundingDestination.occupantNPC = gameObject;
                destinationOffset = surroundingDestination.relativePosToPlayer.transform.localPosition;
                index = i;
                continue;
            }
            if ((surroundingDestination.relativePosToPlayer.transform.position - gameObject.transform.position).magnitude > (surDest.relativePosToPlayer.transform.position - gameObject.transform.position).magnitude)
            {
                surroundingDestination.occupantNPC = null;
                surroundingDestination = surDest;
                surroundingDestination.occupantNPC = gameObject;
                destinationOffset = surroundingDestination.relativePosToPlayer.transform.localPosition;
                index = i;
                continue;
            }
        }

        Debug.Log("closest dest is: " + index);
    }
        

    //Used to detect if NPC is occupying another NPC's region
    public void CheckForStrafeDirectionChange()//call only after AssignClosestEmptyDestination method is called
    {
        SurroundingDest closestOccupiedDest = null;
        foreach (SurroundingDest surDest in target.GetComponent<PlayerController>().relativePos)
        {
            if (surDest.occupantNPC == null) continue;
            if (closestOccupiedDest == null)
            {
                closestOccupiedDest = surDest;
                continue;
            }
            if ((closestOccupiedDest.relativePosToPlayer.transform.position - gameObject.transform.position).magnitude > (surDest.relativePosToPlayer.transform.position - gameObject.transform.position).magnitude)
            {
                closestOccupiedDest = surDest;
                continue;
            }
        }

        if (surroundingDestination == closestOccupiedDest) return;

        EnemyNPCAttack attack = gameObject.GetComponent<EnemyNPCAttack>();
        if (attack.strafeAroundCoroutine != null)
        {
            StopCoroutine(attack.strafeAroundCoroutine);
            OnStopStrafeAroundTheTarget();
        }

        //Special Case: first and last  
        if (surroundingDestination == target.GetComponent<PlayerController>().relativePos.Last() && closestOccupiedDest == target.GetComponent<PlayerController>().relativePos[0])
        {
            Debug.Log("Occupying another NPC's region, strafe left");
            attack.strafeAroundCoroutine = StartCoroutine(StrafeAroundTheTarget(2, -1));//strafe to left
            return;
        }
        else if (closestOccupiedDest == target.GetComponent<PlayerController>().relativePos.Last() && surroundingDestination == target.GetComponent<PlayerController>().relativePos[0])
        {
            Debug.Log("Occupying another NPC's region, strafe right");
            attack.strafeAroundCoroutine = StartCoroutine(StrafeAroundTheTarget(2, 1));//strafe to right
            return;
        }

        int indClosest = 0, indOriginal = 0;
        for (int i = 0; i < target.GetComponent<PlayerController>().relativePos.Count; i++)
        {
            if (target.GetComponent<PlayerController>().relativePos[i] == closestOccupiedDest)
            {
                indClosest = i;
            }
            if (target.GetComponent<PlayerController>().relativePos[i] == surroundingDestination)
            {
                indOriginal = i;
            }
        }

        if (indClosest == indOriginal - 1)
        {
            Debug.Log("Occupying another NPC's region, strafe right");
            attack.strafeAroundCoroutine = StartCoroutine(StrafeAroundTheTarget(2, 1));//strafe to right
            return;
        }
        else if (indOriginal == indClosest - 1)
        {
            Debug.Log("Occupying another NPC's region, strafe left");
            attack.strafeAroundCoroutine = StartCoroutine(StrafeAroundTheTarget(2, -1));//strafe to left
            return;
        }
    }

    public bool TargetOutOfRange()//call in chase
    {
        float buffer = 0.2f;
        if (triggerentryDistance < detectionRadius)
        {
            if (GetDistanceTotarget() > detectionRadius+buffer || target == null)
            {
                target = null;
                return true;
            }
        }
        else if(GetDistanceTotarget() > triggerentryDistance+buffer || target == null)
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

        float speedZ = 0.5f;//bring this up/down gradually
        float speedX = 1.5f;//bring this up/down gradually

        //speedX = speedX <= 0 ? 1.5f : 1.5f;//animations are not enough, walking left and approaching doesn't work together
        animator.SetFloat("SpeedZ", speedZ);
        animator.SetFloat("SpeedX", speedX);
        while (true)
        {
            Quaternion lookRotation = Quaternion.LookRotation(cachedTarget.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime*lerpSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator StrafeAroundTheTarget(float duration, float direction)
    {
        attack.attacking = true;
        GameObject cachedTarget = target;
        
        if (changeSpeedZ != null)
        {
            StopCoroutine(changeSpeedZ);
        }
        if (animator.GetFloat("SpeedZ") < 0)
        {
            StartSpeedUpZ(0);
        }
        else if (animator.GetFloat("SpeedZ") > 0)
        {
            StartSlowDownZ(0);
        }
        float speedX = direction;
        if (changeSpeedX != null)
        {
            StopCoroutine(changeSpeedX);
        }
        if (animator.GetFloat("SpeedX") < speedX)
        {
            StartSpeedUpX(speedX);//bunun ne kadar surecegini hesapla, onu total sureden dus ve o kadar sure 
        }
        else if (animator.GetFloat("SpeedX") > speedX)
        {
            StartSlowDownX(speedX);
        }

        float strafeDur = duration;
        float time = 0f;

        while (time < strafeDur)
        {
            Quaternion lookRotation = Quaternion.LookRotation(cachedTarget.transform.position - transform.position, Vector3.up);
            transform.rotation = lookRotation;

            time += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        OnStopStrafeAroundTheTarget();
    }

    public void OnStopStrafeAroundTheTarget()
    {
        attack.attacking = false;

        if (changeSpeedX != null) 
        { 
            StopCoroutine (changeSpeedX);
        }

        if (changeSpeedZ != null)
        {
            StopCoroutine (changeSpeedZ);
        }

        if (animator.GetFloat("SpeedX") < 0)
        {
            StartSpeedUpX(0);//bunun ne kadar surecegini hesapla, onu total sureden dus ve o kadar sure 
        }
        else if (animator.GetFloat("SpeedX") > 0)
        {
            StartSlowDownX(0);
        }
    }

    public void OnStopStrafing()//when closing in stops
    {
        if (animator.GetFloat("SpeedZ") < 0)
        {
            StartSpeedUpZ(0);
        }
        else if (animator.GetFloat("SpeedZ") > 0)
        {
            StartSlowDownZ(0);
        }

        if (animator.GetFloat("SpeedX") < 0)
        {
            StartSpeedUpX(0);
        }
        else if (animator.GetFloat("SpeedX") > 0)
        {
            StartSlowDownX(0);
        }

        //animator.SetFloat("SpeedZ", 0);//bring down slowly
        //animator.SetFloat("SpeedX", 0);//bring down slowly
        isClosingIn = false;
    }

    public float NegativeParabolicBy2PeakSpeed(float peakSpeed, float xAxPos)//peakSpeed - x^2, called if peakSpeed > 0
    {
        float xAxPosClamped = Mathf.Sqrt(peakSpeed) * xAxPos;
        return peakSpeed - Mathf.Pow(xAxPosClamped, 4);
    }

    public float ParabolicBy2PeakSpeed(float peakSpeed, float xAxPos)//peakSpeed + x^2, call if peakSpeed < 0
    {
        float xAxPosClamped = Mathf.Sqrt(-peakSpeed) * xAxPos;
        return peakSpeed + Mathf.Pow(xAxPosClamped, 4);
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

    public void StartSpeedUpX(float targetSpeed)
    {
        if (changeSpeedX != null)
        {
            StopCoroutine(changeSpeedX);
        }
        changeSpeedX = StartCoroutine(SpeedUpX(targetSpeed));
    }

    public void StartSlowDownX(float targetSpeed)
    {
        if (changeSpeedX != null)
        {
            StopCoroutine(changeSpeedX);
        }
        changeSpeedX = StartCoroutine(SlowDownX(targetSpeed));
    }

    public void StartSpeedUpZ(float targetSpeed)
    {
        if (changeSpeedZ != null)
        {
            StopCoroutine(changeSpeedZ);
        }
        changeSpeedZ = StartCoroutine(SpeedUpZ(targetSpeed));
    }

    public void StartSlowDownZ(float targetSpeed)
    {
        if (changeSpeedZ != null)
        {
            StopCoroutine(changeSpeedZ);
        }
        changeSpeedZ = StartCoroutine(SlowDownZ(targetSpeed));
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

    public IEnumerator SpeedUpZ(float targetSpeed)
    {
        //Debug.Log("speeding up");
        float speed = animator.GetFloat("SpeedZ");
        //Debug.Log("speedZ is: " + speed);

        while (speed < targetSpeed)
        {
            speed += 0.01f;
            animator.SetFloat("SpeedZ", speed);
            yield return new WaitForEndOfFrame();
        }
        speed = targetSpeed;
        animator.SetFloat("SpeedZ", speed);
        yield break;
    }

    public IEnumerator SpeedUpX(float targetSpeed)//do this in real time, not by frames
    {
        //Debug.Log("speeding up");
        float speed = animator.GetFloat("SpeedX");
        //Debug.Log("SpeedX is: " + speed);

        while (speed < targetSpeed)
        {
            speed += 0.01f;
            animator.SetFloat("SpeedX", speed);
            yield return new WaitForEndOfFrame();
        }
        speed = targetSpeed;
        animator.SetFloat("SpeedX", speed);
        yield break;
    }

    public IEnumerator SlowDownZ(float targetSpeed)
    {
        float speed = animator.GetFloat("SpeedZ");
        while (speed > targetSpeed)
        {
            speed -= 0.01f;
            animator.SetFloat("SpeedZ", speed);
            yield return new WaitForEndOfFrame();
        }
        speed = targetSpeed;
        animator.SetFloat("SpeedZ", speed);
        yield break;
    }

    public IEnumerator SlowDownX(float targetSpeed)
    {
        float speed = animator.GetFloat("SpeedX");
        while (speed > targetSpeed)
        {
            speed -= 0.01f;
            animator.SetFloat("SpeedX", speed);
            yield return new WaitForEndOfFrame();
        }
        speed = targetSpeed;
        animator.SetFloat("SpeedX", speed);
        yield break;
    }

    #region Animation Events
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
