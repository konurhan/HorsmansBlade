using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public CinemachineVirtualCamera virtualCamera;
    public float rotationSpeed;

    private void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateCameraFollowTarget();
    }

    public void RotateCameraFollowTarget()
    {
        float yRotation = Input.GetAxis("Mouse X");
        /*if (Input.GetKey(KeyCode.Q))
        {
            yRotation -= 1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            yRotation += 1;
        }*/
        Vector3 rotation = Vector3.zero;
        rotation.y = yRotation;
        target.transform.localEulerAngles += rotation * rotationSpeed;
    }

    public void AimingMode()
    {

    }
}
