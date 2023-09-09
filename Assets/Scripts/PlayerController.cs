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
    public GameObject leftUpperArm;
    public GameObject rightUpperArm;
    public GameObject leftForearm;
    public GameObject rightForearm;
    public GameObject legLeftUpper;
    public GameObject legRightUpper;
    public GameObject legLeftLower;
    public GameObject legRightLower;

    [Header("Skinned Mesh Parts")]
    public Transform NakedParts;
    public Transform ArmourSlots;

    public delegate void OnLevelUP();
    public event OnLevelUP onLevelUp;

    private void Awake()
    {
        SetbodyPartReferences();
    }

    void Start()
    {
        LoadPlayerStats();
        InventoryUI.Instance.onGameSaved += SavePlayerStats;
    }

    void Update()
    {
        
    }

    public void SetbodyPartReferences()
    {
        //arms will reference the same guantlet, legs will reference the same pants and boots
        head.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        torso.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        leftForearm.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        rightForearm.GetComponent<BodyPart>().SetPlayerReference (gameObject);
        leftUpperArm.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        rightUpperArm.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legRightUpper.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legLeftUpper.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legRightLower.GetComponent<BodyPart>().SetPlayerReference(gameObject);
        legLeftLower.GetComponent<BodyPart>().SetPlayerReference(gameObject);
    }

    public void LevelUp()
    {

        onLevelUp.Invoke();
    }

    public void LoadPlayerStats()
    {
        PlayerStats playerStats = SaveSystem.LoadData<PlayerStats>("/PlayerStats.json");
        if (playerStats == null) return;
        speed = playerStats.speed;
        angularSpeed = playerStats.angularSpeed;
        jumpHeight = playerStats.jumpHeight;
        strength = playerStats.strength;
        oneHandedSkillLevel = playerStats.oneHandedSkillLevel;
        twoHandedSkillLevel = playerStats.twoHandedSkillLevel;
        rangedSkillLevel = playerStats.rangedSkillLevel;
        combatJumpHeight = playerStats.combatJumpHeight;
    }

    public void SavePlayerStats()
    {
        PlayerStats playerStats = new PlayerStats(this);
        SaveSystem.SaveData("/PlayerStats.json", playerStats);
    }

}

public class PlayerStats
{
    public float speed;
    public float angularSpeed;
    public float jumpHeight;
    public float strength;

    public float oneHandedSkillLevel;
    public float twoHandedSkillLevel;
    public float rangedSkillLevel;
    public float combatJumpHeight;

    public PlayerStats(PlayerController controller)
    {
        speed = controller.speed;
        angularSpeed = controller.angularSpeed;
        jumpHeight = controller.jumpHeight;
        strength = controller.strength;
        oneHandedSkillLevel = controller.oneHandedSkillLevel;
        twoHandedSkillLevel = controller.twoHandedSkillLevel;
        rangedSkillLevel = controller.rangedSkillLevel;
        combatJumpHeight = controller.combatJumpHeight;
    }
}
