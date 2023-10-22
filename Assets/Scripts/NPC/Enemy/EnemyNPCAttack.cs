using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPCAttack : MonoBehaviour
{
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }

    public GameObject target;
    public Animator targetAnimator;

    public EnemyNPCMovement movement;

    public bool blocking;
    public bool attacking;

    public Coroutine strafeAroundCoroutine;
    public Coroutine turnTowardsTheTargetCoroutine;

    WaitForEndOfFrame waitNextFrame = new WaitForEndOfFrame();
    WaitForSeconds waitAScaledSecond = new WaitForSeconds(1);

    private void Awake()
    {
        blocking = false;
        attacking = false;

        movement = GetComponent<EnemyNPCMovement>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        strafeAroundCoroutine = null;
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void HandleDefence()//will be called from fsm attack state every frame when the npc is in attack state
    {
        CacheTarget();
        if (blocking) return;
        if (attacking) return;
        if (!IsTargetFacingToMe()) return;//if target is not swinging towards you, no need to block
        if (IsInSafeDistance(movement.attackRadius + 1f)) return;//if have a safe distance with the target, no need to block
        
        if (GetComponent<EnemyNPCEquipmentSystem>().shield == null) return;
        if(CheckAttackDownward() || CheckAttackInward() || CheckAttackOutward())
        {
            //Debug.Log("angle is: " + GetAngleToTarget());
            //StartCoroutine(Shieldblock());
            if(GetAngleToTarget() < 0)
            {
                StartCoroutine(TurnLeftToBlock(-GetAngleToTarget()));
            }else if(GetAngleToTarget() > 0)
            {
                StartCoroutine(TurnRightToBlock(GetAngleToTarget()));
            }
            else
            {
                Debug.Log("looking towards the target");
                StartCoroutine(Shieldblock());
            }
        }
    }

    public void HandleAttack()//turn to enemy here
    {
        if (blocking) return;
        if (attacking) return;
        DecideAndAttack();
    }

    public void CacheTarget()
    {
        target = gameObject.GetComponent<EnemyNPCMovement>().target;
        targetAnimator = target.GetComponent<Animator>();
    }

    public void DecideAndAttack()//enemy will decide which attack action to perform
    {
        HandleAngleToTarget();//turn to enemy here

        int rand = Random.Range(0, 100);
        switch (rand)
        {
            case <= 39 when rand >= 0:
                attacking = true;
                animator.SetTrigger(AnimatorController.Instance.InwardSlash);
                StartCoroutine(AttackCoolDown(1.2f));
                break;
            case <= 79 when rand >= 40:
                attacking = true;
                animator.SetTrigger(AnimatorController.Instance.OutwardSlash);
                StartCoroutine(AttackCoolDown(1.2f));
                break;
            case <= 99 when rand >= 80:
                float speedX = UnityEngine.Random.Range(-1f, 1f);
                speedX = speedX <= 0 ? -1f : 1f;
                strafeAroundCoroutine = movement.StartCoroutine(movement.StrafeAroundTheTarget(2, speedX));//sets attacking first true, then false at the end
                break;
        }
    }

    public void HandleAngleToTarget()
    {
        Debug.Log("angle is: " + GetAngleToTarget());
        if (turnTowardsTheTargetCoroutine != null)
        {
            Debug.Log("turnTowardsTheTargetCoroutine is not null");
            return;
        }

        if (GetAngleToTarget() >= 30 || GetAngleToTarget() <= -30)
        {
            turnTowardsTheTargetCoroutine = StartCoroutine(TurnTowardsTheTarget(0.2f));
        }
    }

    public IEnumerator AttackCoolDown(float duration)
    {
        float passed = 0f;
        while (passed < duration)
        {
            passed += Time.deltaTime;
            yield return waitNextFrame;
        }
        attacking = false;
    }

    public bool IsTargetFacingToMe()
    {
        float angle = GetAngleToTarget();
        if(angle > -70 && angle < 70) return true;
        return false;
    }

    public bool IsInSafeDistance(float defenceDistance)
    {
        if (movement.GetDistanceTotarget() <= defenceDistance) return false;
        return true;
    }

    public IEnumerator Shieldblock()
    {
        RaiseShield();

        float passed = 0f;
        while (passed < 1)
        {
            passed += Time.deltaTime;
            yield return waitNextFrame;
        }

        LowerShield();
    }

    public IEnumerator TurnTowardsTheTarget(float duration)
    {
        Debug.Log("TurnTowardsTheTarget is called");
        attacking = true;

        float lerpSpeed = 2f;
        GameObject cachedTarget = target;

        float passed = 0f;

        while (passed < duration)
        {
            Quaternion lookRotation = Quaternion.LookRotation(cachedTarget.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lerpSpeed);
            passed += Time.deltaTime;
            //Debug.Log("passed time: " + passed);
            yield return waitNextFrame;
        }
        attacking = false;
        turnTowardsTheTargetCoroutine = null;
        Debug.Log("TurnTowardsTheTarget is finished");
    }

    public IEnumerator TurnLeftToBlock(float angle)
    {
        Debug.Log("TurnLeftToBlock, angle is: " + angle);
        RaiseShield();
        float speed = 1.5f;
        float rotated = 0f;
        agent.updateRotation = false;
        while (rotated < angle)
        {
            float step = angle * Time.deltaTime * speed;
            transform.Rotate(new Vector3(0, -step, 0));
            rotated += step;
            yield return waitNextFrame;
        }
        LowerShield();
        agent.updateRotation = true;
    }

    public IEnumerator TurnRightToBlock(float angle) 
    {
        Debug.Log("TurnRightToBlock, angle is: " + angle);
        RaiseShield();
        float speed = 1.5f;
        float rotated = 0f;
        agent.updateRotation = false;
        while (rotated < angle) 
        {
            float step = angle * Time.deltaTime * speed;
            transform.Rotate(new Vector3(0,step,0));
            rotated += step;
            yield return waitNextFrame;
        }
        LowerShield();
        agent.updateRotation = true;
    }

    

    public void RaiseShield()
    {
        blocking = true;
        Debug.Log("RaiseShield is called");
        animator.SetBool(AnimatorController.Instance.Shield, true);
    }

    public void LowerShield()
    {
        blocking = false;
        animator.SetBool(AnimatorController.Instance.Shield, false);
    }

    public float GetAngleToTarget()
    {
        if (!target) return 0;
        Vector3 relativeToTarget = target.transform.position - transform.position;
        return Vector3.SignedAngle(transform.forward, relativeToTarget.normalized, Vector3.up);
    }

    public bool IsFacingAtEnemy()
    {
        Vector3 relativeToTarget = target.transform.position - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, relativeToTarget.normalized);
        if (dotProduct > 0)
        {
            return true;
        }
        return false;
    }

    public bool CheckShieldUp()
    {
        if (targetAnimator.GetBool(AnimatorController.Instance.Shield))
        {
            return true;
        }
        return false;
    }

    public bool CheckAttackInward()
    {
        if (target.GetComponent<PlayerAttack>().inwardSlash)
        {
            return true;
        }
        return false;
    }

    public bool CheckAttackOutward()
    {
        if (target.GetComponent<PlayerAttack>().outwardSlash)
        {
            return true;
        }
        return false;
    }

    public bool CheckAttackDownward()
    {
        if (target.GetComponent<PlayerAttack>().downwardSlash)
        {
            return true;
        }
        return false;
    }
}
