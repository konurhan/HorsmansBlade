using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour //each tutorial should have a trigger point
{
    public string Explanation;//what is this tutorial about

    public bool isCompleted;

    [SerializeField] private GameObject UIPanel;//assign from manager script

    //will be checked in update method;
    //public System.Func<bool> CompletionRequirementCehcker;//this action will be assigned in the manager script individually to each tutorial, example: if W,A,S,D are all pressed at least once for movement tutorial

    public List<TutorialStep> steps;

    private void Awake()
    {
        UIPanel = transform.GetChild(0).gameObject;
        isCompleted = false;
    }

    private void Update()
    {
        if (isCompleted) { return;}
        foreach (TutorialStep step in steps)//for each non-complete step listen for completion
        {
            if (!step.isStepCompleted)
            {
                if (step.singleStepAction())
                {
                    step.isStepCompleted = true;
                }
            }
        }
        foreach (TutorialStep step in steps)// if there is a non-complete step, return
        {
            if (!step.isStepCompleted)
            {
                return;
            }
        }
        
        isCompleted = true;
        TutorialManager.instance.currentTutorialIsCompleted = true;
    }

    public void SetSteps(List<TutorialStep> steps) //call from manager scripts
    {
        this.steps = steps;
    }
}

public class TutorialStep
{
    public bool isStepCompleted;

    public System.Func<bool> singleStepAction; //hitting a single key or pushing a single button

    public TutorialStep(System.Func<bool> singleStepAction)
    {
        isStepCompleted = false;
        this.singleStepAction = singleStepAction;
    }
}