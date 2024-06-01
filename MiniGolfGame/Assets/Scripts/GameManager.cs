using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Switch;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    //GameObject ball;
    public int[] playerStrokesArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public GameObject holeUI;
    public TextMeshProUGUI strokesText;
    public TextMeshProUGUI holeUITitle;

    bool startNextScene;
    //string nextSceneName;
    //string lastSceneName;

    //string levelName;
    public int levelNumber = 1;
    string nextLevel;

    public enum GameStates
    {
        MainMenu,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8,
        Level9,
        Level10
    }
    public GameStates state = GameStates.MainMenu;

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        switch(state)
        {
            case GameStates.MainMenu:
                StartMainMenu();
                break;
            case GameStates.Level1:
            case GameStates.Level2:
            case GameStates.Level3:
            case GameStates.Level4:
            case GameStates.Level5:
            case GameStates.Level6:
            case GameStates.Level7:
            case GameStates.Level8:
            case GameStates.Level9:
            case GameStates.Level10:
                StartLevel();
                break;
        }
    }

    void Update()
    {
        switch(state)
        {
            case GameStates.MainMenu:
                //MainMenuLoop();
                break;
            case GameStates.Level1:
            case GameStates.Level2:
            case GameStates.Level3:
            case GameStates.Level4:
            case GameStates.Level5:
            case GameStates.Level6:
            case GameStates.Level7:
            case GameStates.Level8:
            case GameStates.Level9:
            case GameStates.Level10:
                LevelLoop();
                break;
        }
    }

    /*public void StartNextScene()
    {
        startNextScene = true;
        nextSceneName = lastSceneName;
    }*/

    //Starts the specified scene
    /*public void StartScene(GameStates scene)
    {
        startNextScene = true;
        nextSceneName = SceneNameToString(scene);
    }*/

    
    public string SceneNameToString(GameStates scene)
    {
        return scene.ToString();
    }
    public GameStates SceneNameToEnum(string scene)
    {
        return (GameStates)Enum.Parse(typeof(GameStates), scene);
    }

    
    private void StartMainMenu()
    {

    }


    
    /*private void MainMenuLoop()
    {
        if(startNextScene)
        {
            startNextScene = false;
            state = GameStates.LevelSelect;
            SceneManager.LoadScene("LevelSelect");
        }
    }*/

    
    private void LevelLoop()
    {
        if (startNextScene)
        {
            startNextScene = false;
            LoadNextLevel();
        }
    }

    
    private void StartLevel()
    {
        //ball = GameObject.FindGameObjectWithTag("Player");
        holeUI = GameObject.Find("HoleUI");
        strokesText = GameObject.Find("StrokesText").GetComponent<TextMeshProUGUI>();
        levelNumber = GetCurrentLevelNumber();
        nextLevel = "Level" + (levelNumber + 1);
        strokesText.SetText("0");
        holeUI.SetActive(false);
        strokesText.enabled = true;
    }


    
    public void LoadNextLevel()
    {
        state = SceneNameToEnum(nextLevel);
        SceneManager.LoadScene(SceneNameToString(state));
    }
    public void MainMenu()
    {
        state = GameStates.MainMenu;
        SceneManager.LoadScene(SceneNameToString(state));
    }



    public void setHoleUIActive(bool isActive)
    {
        holeUI.SetActive(isActive);
        if (isActive)
        {
            //holeUITitle.SetText("HOLE IN " + strokesText.text + "!");
            strokesText.enabled = false;
        }
    }

    public int GetCurrentLevelNumber()
    {
        string levelName = SceneNameToString(state);
        try
        {
            int levelNumber = Int32.Parse(levelName.Substring(levelName.Length - 1));
            if(levelNumber == 0)
            {
                levelNumber = 10;
            }
            return levelNumber;
        }
        catch (FormatException)
        {
            Console.Error.WriteLine($"Unable to parse '{levelName.Substring(levelName.Length - 1)}' from scene name '{levelName}'");
            Console.Error.WriteLine("Returning Level 1");

            return 1;
        }
    }
    public void addStrokeToCurrentLevel()
    {
        playerStrokesArray[(int)levelNumber - 1]++;
        strokesText.SetText(playerStrokesArray[(int)levelNumber - 1].ToString());

    }
    public void resetStrokes()
    {
        playerStrokesArray[(int)levelNumber - 1] = 0;
        strokesText.SetText("0");
        strokesText.enabled = true;
    }

    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        state = SceneNameToEnum(levelName);
        SceneManager.LoadScene(levelName);
    }
}
