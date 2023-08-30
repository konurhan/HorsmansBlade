using UnityEngine;
using Cinemachine;

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
    public float turnSpeed = 1f;
    public Vector3 RBvelocity;

    public float SpeedZUpper;
    public float SpeedZLower;
    

    [Header("Character Stats")]
    [SerializeField] private float speed;


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


    private void Update()
    {
        RBvelocity = rb.velocity;
        HandleMovement();
        GroundedCeheck();
    }

    private void FixedUpdate()
    {
          
    }

    public void GroundedCeheck()
    {
        Vector3 rayOrigin = transform.position + GetComponent<CapsuleCollider>().center;
        RaycastHit hit;
        float searchDistance = GetComponent<CapsuleCollider>().height/2 + searchOffset;
        if(Physics.Raycast(rayOrigin,Vector3.down, out hit, searchDistance)) 
        {
            //Debug.Log("hit to: "+hit.transform.gameObject.name);
            if(hit.transform.gameObject.tag != "Player")
            {
                //Debug.Log("collided objcet name is: " + hit.transform.gameObject.name);
                isGrounded = true;
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                animator.applyRootMotion = true;
                //Debug.Log("on ground");
            }
        }
        else
        {
            isGrounded = false;
            animator.applyRootMotion = false;
            //Debug.Log("in air");
        }
    }

    private void OnAnimatorMove()
    {
        if(isGrounded)
        {
            Vector3 velocity = animator.deltaPosition / Time.deltaTime;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

            InventoryUI.Instance.MakeClosestCollectibleItemVisible();
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
        }
        else
        {
            animator.SetBool("TurnLeft", false);
            animator.SetBool("TurnRight", false);
        }
        transform.Rotate(0, x * turnSpeed, 0);
    }

    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float height = 0;
            //isGrounded = false;//not necessary
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
            rb.AddForce(Vector3.up*rb.mass*gravity* height, ForceMode.Impulse);
            Vector3 vel = rb.velocity;
            rb.velocity = Vector3.zero;
            rb.AddForce(vel, ForceMode.VelocityChange);
            //Debug.Log("rbvel after impulse force: " + rb.velocity);
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
        isMovementRoationalEnabled = false;
    }

    #endregion
}
