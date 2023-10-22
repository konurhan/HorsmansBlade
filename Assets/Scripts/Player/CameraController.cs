using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public CinemachineVirtualCamera mainVCamera;
    public CinemachineVirtualCamera aimCamera;
    public float rotationSpeed;

    private PlayerAttack attack;

    private void Awake()
    {
        attack = GetComponent<PlayerAttack>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //make this changes triggered from player attack script for efficiency
        if (attack.aiming)
        {
            RotateCameraFollowTarget();
            //target.transform.localEulerAngles -= new Vector3(0,50,0);

            if (aimCamera.gameObject.activeSelf)
            {
                attack.HandleSpineLookAt();
                return;
            } 
            aimCamera.gameObject.SetActive(true);
            mainVCamera.gameObject.SetActive(false);
            attack.HandleSpineLookAt();
        }
        else
        {
            if (mainVCamera.gameObject.activeSelf)
            {
                RotateCameraFollowTarget();
                return;
            }
            aimCamera.gameObject.SetActive(false);
            mainVCamera.gameObject.SetActive(true);
            RotateCameraFollowTarget();
        }
    }

    public void RotateCameraFollowTarget()
    {
        float yRotation = Input.GetAxis("Mouse X");
        float xRotation = Input.GetAxis("Mouse Y");
        Vector3 rotation = Vector3.zero;
        rotation.y = yRotation;
        rotation.x = xRotation;
        target.transform.localEulerAngles += rotation * rotationSpeed;
    }

    public void ChooseCameraModeMode()
    {

    }
}
