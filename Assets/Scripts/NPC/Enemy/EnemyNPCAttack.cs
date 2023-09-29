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

    private Coroutine strafeAroundCoroutine;

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

    public void HandleAttack()
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
        int rand = Random.Range(3, 4);
        switch (rand)
        {
            case 0:
                attacking = true;
                animator.SetTrigger("InwardSlash");
                StartCoroutine(AttackCoolDown(2f));
                break;
            case 1:
                attacking = true;
                animator.SetTrigger("OutwardSlash");
                StartCoroutine(AttackCoolDown(2f));
                break;
            case 2:
                attacking = true;
                animator.SetTrigger("DownwardSlash");
                StartCoroutine(AttackCoolDown(2f));
                break;
            case 3:
                //doing nothing: can strafe in this phase, or reposition
                strafeAroundCoroutine = movement.StartCoroutine(movement.StrafeAroundTheTarget(3));
                break;
        }
    }

    public IEnumerator AttackCoolDown(float duration)
    {
        float passed = 0f;
        while (passed < duration)
        {
            passed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
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
        while(true)
        {
            yield return new WaitForSeconds(1);
            break;
        }
        LowerShield();
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
            yield return new WaitForEndOfFrame();
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
            yield return new WaitForEndOfFrame();
        }
        LowerShield();
        agent.updateRotation = true;
    }

    

    public void RaiseShield()
    {
        blocking = true;
        Debug.Log("RaiseShield is called");
        animator.SetBool("Shield", true);
    }

    public void LowerShield()
    {
        blocking = false;
        animator.SetBool("Shield", false);
    }

    public float GetAngleToTarget()
    {
        if (!target) return 0;
        Vector3 relativeToTarget = target.transform.position - transform.position;
        //Vector3 axis = Vector3.Cross(relativeToTarget, transform.forward);//changes its sign with the sign of the angle, so it causes sign to be always negative or always positive
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
        if (targetAnimator.GetBool("Shield"))
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
