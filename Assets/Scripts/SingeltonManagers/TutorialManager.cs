using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public Transform TutorialsParentTransform;
    [SerializeField] int lastCompletedTutorialNum;

    [SerializeField] GameObject currentTutorial;
    public bool currentTutorialIsCompleted;//set from inside a tutorial

    [SerializeField] List<List<TutorialStep>> tutorialCompletionCheckers;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        InventoryUI.Instance.onGameSaved += SaveTutorialProgress;
        
        SetupCheckersList();//add completion checker methods for all tutorials

        LoadTutorialProgress();
        
    }

    private void OnDisable()
    {
        InventoryUI.Instance.onGameSaved -= SaveTutorialProgress;
    }

    private void Update()
    {
        if (currentTutorialIsCompleted)
        {
            LoadNextTutorial();
        }
    }

    private void SaveTutorialProgress()
    {
        TutorialProgressData data = new TutorialProgressData(lastCompletedTutorialNum);
        SaveSystem.SaveData("/TutorialProgressData.json", data);
    }

    private void LoadTutorialProgress()
    {
        TutorialProgressData data = SaveSystem.LoadData<TutorialProgressData>("/TutorialProgressData.json");
        if (data == null) 
        { 
            lastCompletedTutorialNum = -1;
        }
        else
        {
            lastCompletedTutorialNum = data.lastCompletedTutorialIndex;
        }

        if (Resources.Load("Prefabs/Tutorials/Tutorial" + (lastCompletedTutorialNum + 1).ToString()) == null)
        {
            Debug.Log("Couldn't Find Path: Prefabs/Tutorials/Tutorial" + (lastCompletedTutorialNum + 1).ToString());
            currentTutorial = null;
            Menu.instance.DeactivateTutorials();
            return;//no more tutorials left
        }
        currentTutorial = Instantiate(Resources.Load("Prefabs/Tutorials/Tutorial"+ (lastCompletedTutorialNum + 1).ToString()), TutorialsParentTransform) as GameObject;
        currentTutorialIsCompleted = false;
        SetupCurrentTutorial();
    }

    private void LoadNextTutorial()
    {
        lastCompletedTutorialNum++;

        Destroy(currentTutorial);
        currentTutorialIsCompleted = false;

        if (Resources.Load("Prefabs/Tutorials/Tutorial" + (lastCompletedTutorialNum + 1).ToString()) == null)
        {
            Debug.Log("no more tutorials left");//close tutorial system
            currentTutorial = null;
            Menu.instance.DeactivateTutorials();
            return;//no more tutorials left
        }
        currentTutorial = Instantiate(Resources.Load("Prefabs/Tutorials/Tutorial" + (lastCompletedTutorialNum + 1).ToString()), TutorialsParentTransform) as GameObject;
        SetupCurrentTutorial();
    }

    private void SetupCheckersList()
    {
        AddToList(MovementCheck, GetButtonPressedW);
        AddToList(MovementCheck, GetButtonPressedA);
        AddToList(MovementCheck, GetButtonPressedS);
        AddToList(MovementCheck, GetButtonPressedD);
        AddToList(MovementCheck, GetButtonPressedSpace);

        AddToList(MeleeEquipmentCheck, IfMeleeWeaponEquipped);
        AddToList(MeleeEquipmentCheck, IfEquippedMeleeWeaponHasDrawn);

        AddToList(MeleeWeaponAttackCheck, IfMeleeWeaponOutwardSlash);
        AddToList(MeleeWeaponAttackCheck, IfMeleeWeaponInwardSlash);

        AddToList(RangedWeaponEquipmentCheck, IfRangedWeaponEquipped);
        AddToList(RangedWeaponEquipmentCheck, IfEquippedRangedWeaponHasDrawn);

        AddToList(RangedWeaponAttackCheck, IfRangedWeaponIsUsed);

        tutorialCompletionCheckers = new List<List<TutorialStep>>()
        {
            CreateStepsForTutorial(MovementCheck),
            CreateStepsForTutorial(MeleeEquipmentCheck),
            CreateStepsForTutorial(MeleeWeaponAttackCheck),
            CreateStepsForTutorial(RangedWeaponEquipmentCheck),
            CreateStepsForTutorial(RangedWeaponAttackCheck),
        };
    }

    private void SetupCurrentTutorial()//this method might not be needed
    {
        currentTutorial.GetComponent<Tutorial>().steps = tutorialCompletionCheckers[lastCompletedTutorialNum+1];
    }

    private List<TutorialStep> CreateStepsForTutorial(List<System.Func<bool>> methods)
    {
        List<TutorialStep> steps = new List<TutorialStep>();
        foreach (System.Func<bool> method in methods)
        {
            TutorialStep step = new TutorialStep(method);
            steps.Add(step);
        }
        return steps;
    }

    private void AddToList(List<System.Func<bool>> stepList, System.Func<bool> method)
    {
        stepList.Add(method);
    }

    #region Complete List of Steps of Individual Tutorials

    [SerializeField] private List<System.Func<bool>> MovementCheck = new List<System.Func<bool>>();
    [SerializeField] private List<System.Func<bool>> MeleeEquipmentCheck = new List<System.Func<bool>>();//open invenetory; equip a weapon; press 1,2 or 3 to draw it; use the weapon
    [SerializeField] private List<System.Func<bool>> MeleeWeaponAttackCheck = new List<System.Func<bool>>();
    [SerializeField] private List<System.Func<bool>> RangedWeaponEquipmentCheck = new List<System.Func<bool>>();
    [SerializeField] private List<System.Func<bool>> RangedWeaponAttackCheck = new List<System.Func<bool>>();
    
    #endregion

    #region OneStep Actions

    public bool GetButtonPressedW()
    {
        if (Input.GetKeyDown(KeyCode.W)) return true;
        return false;
    }

    public bool GetButtonPressedA()
    {
        if (Input.GetKeyDown(KeyCode.A)) return true;
        return false;
    }

    public bool GetButtonPressedS()
    {
        if (Input.GetKeyDown(KeyCode.S)) return true;
        return false;
    }

    public bool GetButtonPressedD()
    {
        if (Input.GetKeyDown(KeyCode.D)) return true;
        return false;
    }

    public bool GetButtonPressedSpace()
    {
        if (Input.GetKeyDown(KeyCode.Space)) return true;
        return false;
    }

    public bool IfMeleeWeaponEquipped()
    {
        PlayerAttack attack = InventoryUI.Instance.Player.GetComponent<PlayerAttack>();
        if (attack.meleeWeaponOneHanded == null && attack.meleeWeaponTwoHanded == null) return false;
        return true;
    }

    public bool IfEquippedMeleeWeaponHasDrawn()
    {
        PlayerAttack attack = InventoryUI.Instance.Player.GetComponent<PlayerAttack>();
        if (attack.usingOneHanded || attack.usingTwoHanded) return true;
        return false;
    }

    public bool IfMeleeWeaponInwardSlash()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetAxis("Mouse X") < -0.15f && Input.GetAxis("Mouse Y") < 0.15f && Input.GetAxis("Mouse Y") > -0.15f)
            {
                return true;
            }
        }
        return false;
    }

    public bool IfMeleeWeaponOutwardSlash()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetAxis("Mouse X") > 0.15f && Input.GetAxis("Mouse Y") < 0.15f && Input.GetAxis("Mouse Y") > -0.15f)
            {
                return true;
            }
        }
        return false;
    }

    public bool IfRangedWeaponEquipped()
    {
        PlayerAttack attack = InventoryUI.Instance.Player.GetComponent<PlayerAttack>();
        if (attack.rangedWeapon == null) return false;
        return true;
    }

    public bool IfEquippedRangedWeaponHasDrawn()
    {
        PlayerAttack attack = InventoryUI.Instance.Player.GetComponent<PlayerAttack>();
        if (attack.usingRanged) return true;
        return false;
    }

    public bool IfRangedWeaponIsUsed()
    {
        if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(0))
            {
                return true;
            }
        }
        return false;
    }

    #endregion
}

public class TutorialProgressData
{
    public int lastCompletedTutorialIndex;

    public TutorialProgressData()
    {
        lastCompletedTutorialIndex = 0;
    }

    public TutorialProgressData(int lastInd)
    {
        lastCompletedTutorialIndex = lastInd;
    }
}
