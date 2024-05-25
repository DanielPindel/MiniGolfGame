using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;
using TMPro;

public class LogicScript : MonoBehaviour
{
    public int[] playerStrokesArray = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    public TextMeshProUGUI strokesText;
    public GameObject holeUI;
    public TextMeshProUGUI holeUITitle;

    void Start()
    {
        strokesText.SetText("0");
        holeUI.SetActive(false);
        strokesText.enabled = true;
    }

    public void setHoleUIActive(bool active)
    {
        holeUI.SetActive(active);
        if (active)
        {
            holeUITitle.SetText("HOLE IN " + strokesText.text + "!");
            strokesText.enabled = false;
        }
    }

    public void nextLevel()
    {
        int nextLevelNumber = tryGetCurrentLevelNumber("Unable to load next level.") + 1;

        SceneManager.LoadScene("Level" + nextLevelNumber);
    }

    public void addStrokeToCurrentLevel()
    {
        int levelNumber = tryGetCurrentLevelNumber("Unable to add level strokes.");

        playerStrokesArray[(int)levelNumber - 1]++;
        strokesText.SetText(playerStrokesArray[(int)levelNumber - 1].ToString());

    }

    public void resetStrokes()
    {
        int levelNumber = tryGetCurrentLevelNumber("Unable to reset level strokes.");

        playerStrokesArray[(int)levelNumber - 1] = 0;
        strokesText.SetText("0");

    }


    private int tryGetCurrentLevelNumber(string errorMessage)
    {
        string levelName = SceneManager.GetActiveScene().name;
        int levelNumber;

        try
        {
            levelNumber = Int32.Parse(levelName.Substring(levelName.Length - 1));
            return levelNumber;
        }
        catch (FormatException)
        {
            Console.Error.WriteLine($"Unable to parse '{levelName.Substring(levelName.Length - 1)}' from scene name '{levelName}'");
            Console.Error.WriteLine(errorMessage);
            Console.Error.WriteLine("Returning Level 1");

            return 1;
        }
    }

    private T GetChildComponentByName<T>(string name) where T : Component
    {
        foreach (Component component in GetComponentsInChildren<Component>(true))
        {
            if (component.gameObject.name == name)
            {
                
            }
        }
        return null;
    }

}
