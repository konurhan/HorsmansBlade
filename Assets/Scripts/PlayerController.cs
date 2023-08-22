using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Character Stats")]
    public float speed;
    public float angularSpeed;
    public float jumpHeight;
    public float strength;

    [Header("Combat Stats")]
    public float oneHandedSkillLevel;
    public float twoHandedSkillLevel;
    public float rangedSkillLevel;
    public float combatJumpHeight;

    [Header("Body Parts")]
    public GameObject head;
    public GameObject torso;
    public GameObject armLeft;
    public GameObject armRight;
    public GameObject legLeft;
    public GameObject legRight;

    public delegate void OnLevelUP();
    public event OnLevelUP onLevelUp;

    private void Awake()
    {
        SetbodyPartReferences();
    }

    void Start()
    {
        LoadPlayerStats();
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

    public void LevelUp()
    {

        onLevelUp.Invoke();
    }

    public void LoadPlayerStats()
    {

    }

}
