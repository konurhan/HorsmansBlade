using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Character Stats")]
    public float speed;
    public float angularSpeed;
    public float jumpHeight;
    public float strength;

    [Header("Movement XP")]
    public float speedXP;
    public float angularSpeedXP;

    [Header("Combat Stats")]
    public float oneHandedSkillLevel;
    public float twoHandedSkillLevel;
    public float rangedSkillLevel;
    public float combatJumpHeight;

    [Header("Combat XP")]
    public float oneHandedSkillXP;
    public float twoHandedSkillXP;
    public float rangedSkillXP;

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

        Menu.onGamePaused += ConfigureForPause;
        Menu.onGameResumed += ConfigureForResume;
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

    public void GainSpeedXP()
    {
        speedXP++;
        if (speedXP > 10000)
        {
            speedXP -= 10000;
            speed += 0.1f ;
            LevelUp("speed");
        }
    }

    public void GainAngularSpeedXP()
    {
        angularSpeedXP++;
        if (angularSpeedXP > 10000)
        {
            angularSpeedXP -= 10000;
            angularSpeed += 0.1f;
            LevelUp("angularSpeed");
        }
    }

    public void GainOneHandedXP()
    {
        oneHandedSkillXP++;
        if (oneHandedSkillXP > 10)
        {
            oneHandedSkillXP -= 10;
            oneHandedSkillLevel++;
            LevelUp("oneHanded");
        }
    }

    public void GainTwoHandedXP()
    {
        twoHandedSkillXP++;
        if (twoHandedSkillXP > 10)
        {
            twoHandedSkillXP -= 10;
            twoHandedSkillLevel++;
            LevelUp("twoHanded");
        }
    }

    public void GainRangedXP()
    {
        rangedSkillXP++;
        if (rangedSkillXP > 10)
        {
            rangedSkillXP -= 10;
            rangedSkillLevel++;
            LevelUp("ranged");
        }
    }

    public void LevelUp(string skillName)
    {
        if (onLevelUp == null) return;
        onLevelUp.Invoke();

        string message = null;
        switch (skillName)
        {
            case "ranged":
                message = "Ranged weapon skill leveled up to " + rangedSkillLevel.ToString();
                break;
            case "oneHanded":
                message = "One Handed weapon skill leveled up to " + oneHandedSkillLevel.ToString();
                break;
            case "twoHanded":
                message = "Two Handed weapon skill leveled up to " + twoHandedSkillLevel.ToString();
                break;
            case "angularSpeed":
                message = "Angular Speed leveled up to " + angularSpeed.ToString();
                break;
            case "speed":
                message = "Speed leveled up to " + speed.ToString();
                break;
        }

        GameObject floatingMessaege = Instantiate(Resources.Load("Prefabs/UI/FloatingText"), InventoryUI.Instance.FloatingTextParent) as GameObject;
        floatingMessaege.GetComponent<TextMeshProUGUI>().text = message;
        Destroy(floatingMessaege, 3f);
    }

    public void ConfigureForPause()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<PlayerHealth>().enabled = false;
        GetComponent<CameraController>().enabled = false;
        GetComponent<InventoryController>().enabled = false;
    }

    public void ConfigureForResume()
    {
        GetComponent<Animator>().enabled = true;
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<PlayerAttack>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<PlayerHealth>().enabled = true;
        GetComponent<CameraController>().enabled = true;
        GetComponent<InventoryController>().enabled = true;
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
        oneHandedSkillXP = playerStats.oneHandedSkillXP;
        twoHandedSkillXP = playerStats.twoHandedSkillXP;
        rangedSkillXP = playerStats.rangedSkillXP;
    }

    public void SavePlayerStats()
    {
        PlayerStats playerStats = new PlayerStats(this);
        SaveSystem.SaveData("/PlayerStats.json", playerStats);
    }

}

[System.Serializable]
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

    public float oneHandedSkillXP;
    public float twoHandedSkillXP;
    public float rangedSkillXP;

    public PlayerStats()
    {
        
    }

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
        oneHandedSkillXP = controller.oneHandedSkillXP;
        twoHandedSkillXP = controller.twoHandedSkillXP;
        rangedSkillXP = controller.rangedSkillXP;
    }
}
