using UnityEngine;
using Cinemachine;


[DefaultExecutionOrder(1)]
public class PlayerMovement : MonoBehaviour
{
    //public CinemachineVirtualCamera vcam;
    public Camera mainCam;
    private Rigidbody rb;
    private Animator animator;
    private float searchOffset = 0.01f;
    public float jumpHeight = 0.5f;
    public float combatJumpHeight = 0.3f;
    public float gravity = 9.81f;
    
    public Vector3 RBvelocity;

    public float SpeedZUpper;
    public float SpeedZLower;
    

    [Header("Character Stats")]
    [SerializeField] private float speed;
    public float angularSpeed = 1f;

    [Header("Flags")]
    [SerializeField]private bool isGrounded;
    public bool isMovementRoationalEnabled;
    private float lastHorizontalSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        isGrounded = true;
        isMovementRoationalEnabled = true;

        SpeedZUpper = 2f;
        SpeedZLower = -2f;
    }

    private void Start()
    {
        GetComponent<PlayerController>().onLevelUp += CachePlayerMovementStats;

        speed = GetComponent<PlayerController>().speed;
        angularSpeed = GetComponent<PlayerController>().angularSpeed;

        animator.SetFloat("MovementSpeedMultiplier", speed);
        animator.SetFloat("AngularSpeedMultiplier", angularSpeed * 5);
    }

    private void Update()
    {
        RBvelocity = rb.velocity;
        HandleMovement();
        GroundedCeheck();
    }

    private void FixedUpdate()
    {
          
    }

    public void CachePlayerMovementStats()
    {
        speed = GetComponent<PlayerController>().speed;
        angularSpeed = GetComponent<PlayerController>().angularSpeed;
    }

    public void GroundedCeheck()
    {
        Vector3 rayOrigin = transform.position + GetComponent<CapsuleCollider>().center;
        float searchDistance = GetComponent<CapsuleCollider>().height / 2 + searchOffset;
        if (Physics.Raycast(rayOrigin,Vector3.down, out RaycastHit hit, searchDistance)) 
        {
            if(!hit.transform.gameObject.CompareTag("Player"))
            {
                isGrounded = true;
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                animator.applyRootMotion = true;
            }
        }
        else
        {
            isGrounded = false;
            animator.applyRootMotion = false;
        }
    }

    private void OnAnimatorMove()
    {
        if(isGrounded)
        {
            if (Time.deltaTime == 0) return;

            Vector3 velocity = animator.deltaPosition / Time.deltaTime;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

            InventoryUI.Instance.MakeClosestCollectibleItemVisible();
            InventoryUI.Instance.MakeClosestLootableContainerVisible();
        }    
    }

    public void HandleMovement()
    {
        HandleJump();
        float forwardSpeed = animator.GetFloat("SpeedZ");
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
            {
                forwardSpeed += 0.01f;
                if (forwardSpeed > SpeedZUpper) forwardSpeed = SpeedZUpper;
                animator.SetFloat("SpeedZ", forwardSpeed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                forwardSpeed -= 0.01f;
                if (forwardSpeed < SpeedZLower) forwardSpeed = SpeedZLower;
                animator.SetFloat("SpeedZ", forwardSpeed);
            }
            GetComponent<PlayerController>().GainSpeedXP();
        }
        else
        {
            SlowDownForward();
        }

        float x = 0;
        float speedBuff = 0.1f;
        if (Input.GetKey(KeyCode.D) && isMovementRoationalEnabled)
        {
            x += 1;
            
        }
        else if (Input.GetKey(KeyCode.A) && isMovementRoationalEnabled)
        {
            x -= 1;
            
        }
        if(x > 0)
        {
            if (animator.GetFloat("SpeedZ") < speedBuff && animator.GetFloat("SpeedZ") > -speedBuff) 
            {
                animator.SetBool("TurnRight", true);
            }
            else
            {
                animator.SetBool("TurnRight", false);
            }
            animator.SetBool("TurnLeft", false);
            GetComponent<PlayerController>().GainAngularSpeedXP();
        }
        else if (x < 0)
        {
            if (animator.GetFloat("SpeedZ") < speedBuff && animator.GetFloat("SpeedZ") > -speedBuff)
            {
                animator.SetBool("TurnLeft", true);
            }
            else
            {
                animator.SetBool("TurnLeft", false);
            } 
            animator.SetBool("TurnRight", false);
            GetComponent<PlayerController>().GainAngularSpeedXP();
        }
        else
        {
            animator.SetBool("TurnLeft", false);
            animator.SetBool("TurnRight", false);
        }
        transform.Rotate(0, x * angularSpeed, 0);
    }

    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float height;
            if(animator.GetLayerWeight(1)  == 1)
            {
                animator.SetTrigger("CombatJump");
                height = combatJumpHeight;
            }
            else
            {
                animator.SetTrigger("Jump");
                height = jumpHeight;
            }
            
            searchOffset = -0.02f;//now raycast can't hit to the ground

            rb.AddForce(height * Physics.gravity.magnitude * rb.mass * Vector3.up, ForceMode.Impulse);
            Vector3 vel = rb.velocity;
            rb.velocity = Vector3.zero;
            rb.AddForce(vel, ForceMode.VelocityChange);
        }
        else
        {
            searchOffset = 0.01f;//now raycast can hit to the ground
        }
    }

    public void SlowDownForward()
    {
        float speed = animator.GetFloat("SpeedZ");
        if (speed > 0) 
        {
            speed -= 0.01f;
        }else if (speed < 0) 
        {
            speed += 0.01f;
        }

        if (Mathf.Abs(speed) < 0.01f)
        {
            speed = 0;
        }
        animator.SetFloat("SpeedZ", speed);
    }

    public void SetRotationAnimationSpeed()//foot movements should become faster as the character levels-up and become faster.
    {

    }


    #region Animation Events

    public void EnableRotationalMovement()
    {
        isMovementRoationalEnabled = true;
    }

    public void DisableRotationalMovement()
    {
        Debug.Log("DisableRotationalMovement is called");
        isMovementRoationalEnabled = false;
    }

    #endregion
}
