using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Body Parts")]
    public GameObject head;
    public GameObject torso;
    public GameObject armLeft;
    public GameObject armRight;
    public GameObject legLeft;
    public GameObject legRight;

    private void Awake()
    {
        SetbodyPartReferences();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetbodyPartReferences()
    {
        head.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        torso.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        armLeft.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        armRight.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legLeft.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legRight.GetComponent<BodyPart>().SetPlayerReference(gameObject);
    }
}
