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

    public bool blocking;

    private void Awake()
    {
        blocking = false;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        
    }

    public void HandleAttackDefence()//will be called from fsm attack state
    {
        CacheTarget();
        if(CheckAttackDownward() || CheckAttackInward() || CheckAttackOutward())
        {
            if (blocking) return;
            Debug.Log("angle is: " + GetAngleToTarget());
            //StartCoroutine(Shieldblock());
            if(GetAngleToTarget() < 0)
            {
                StartCoroutine(TurnLeftToBlock(-GetAngleToTarget()));
            }else if(GetAngleToTarget() > 0)
            {
                StartCoroutine(TurnRightToBlock(GetAngleToTarget()));
            }
        }
        DecideAndAttack();
    }

    public void CacheTarget()
    {
        target = gameObject.GetComponent<EnemyNPCMovement>().target;
        targetAnimator = target.GetComponent<Animator>();
    }

    public void DecideAndAttack()
    {
        //animator.SetTrigger("InwardSlash");
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
        blocking = true;
        Debug.Log("TurnLeftToBlock, angle is: " + angle);
        RaiseShield();
        float speed = 1f;
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
        blocking = false;
    }

    public IEnumerator TurnRightToBlock(float angle) 
    {
        blocking = true;
        Debug.Log("TurnRightToBlock, angle is: " + angle);
        RaiseShield();
        float speed = 1f;
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
        blocking = false;
    }


    public void RaiseShield()
    {
        Debug.Log("RaiseShield is called");
        animator.SetBool("Shield", true);
    }

    public void LowerShield()
    {
        animator.SetBool("Shield", false);
    }

    public float GetAngleToTarget()
    {
        Vector3 relativeToTarget = target.transform.position - transform.position;
        Vector3 axis = Vector3.Cross(relativeToTarget, transform.forward);//if doesn't work change this: the axis which with respect to the angle is calculated
        return Vector3.SignedAngle(transform.forward, relativeToTarget.normalized, Vector3.up);
    }

    public bool IsFacingAtEnemy()
    {
        Vector3 relativeToTarget = target.transform.position - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, relativeToTarget.normalized);
        //perpendicular cases should also be considered
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
        if (target.GetComponent<EquipmentSystem>().inwardSlash)
        {
            return true;
        }
        return false;
    }

    public bool CheckAttackOutward()
    {
        if (target.GetComponent<EquipmentSystem>().outwardSlash)
        {
            return true;
        }
        return false;
    }

    public bool CheckAttackDownward()
    {
        if (target.GetComponent<EquipmentSystem>().downwardSlash)
        {
            return true;
        }
        return false;
    }
}
