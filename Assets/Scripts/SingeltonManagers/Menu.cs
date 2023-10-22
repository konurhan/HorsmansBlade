using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static Menu instance;

    public Transform MainMenu;

    private float cachedTimescale = 1f;

    public delegate void OnGamePaused();
    public static event OnGamePaused onGamePaused;

    public delegate void OnGameResumed();
    public static event OnGameResumed onGameResumed;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        OpenCloseMainMenu();
    }

    #region Menu Button Callbacks

    public void OpenCloseMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !MainMenu.gameObject.activeInHierarchy)
        {
            MainMenu.gameObject.SetActive(true);
            PauseGame();
            onGamePaused.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && MainMenu.gameObject.activeInHierarchy)
        {
            MainMenu.gameObject.SetActive(false);
            ResumeGame();
            onGameResumed.Invoke();
        }
    }

    public void ActivateTutorials()
    {
        if (gameObject.GetComponent<TutorialManager>().enabled) return;
        gameObject.GetComponent<TutorialManager>().enabled = true;

        Transform parent = TutorialManager.instance.TutorialsParentTransform;
        parent.gameObject.SetActive(true);
    }

    public void DeactivateTutorials()
    {
        if (!gameObject.GetComponent<TutorialManager>().enabled) return;
        gameObject.GetComponent<TutorialManager>().enabled = false;

        Transform parent = TutorialManager.instance.TutorialsParentTransform;
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(0).gameObject);
        }
        parent.gameObject.SetActive(false);
    }

    #endregion


    #region Utils

    private void PauseGame()
    {
        cachedTimescale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = cachedTimescale;
    }

    #endregion
}
