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
