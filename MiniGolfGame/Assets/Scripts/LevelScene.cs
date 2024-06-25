using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelScene : MonoBehaviour
{
    /**
    * A private GameObject variable to reference the scene.
    */
    GameObject scene;

#nullable enable
    /**
    * CinemachineFreeLook variable to reference the free look camera.
    */
    public CinemachineFreeLook? freeLookCamera;
    /**
    * Float variable to store the mouse sensitivity.
    */
    public float mouseSensitivity;
    /**
    * Float variable to store the smoothing factor of the camera movement.
    */
    public float smoothingFactor = 2f;
    /**
    * Float variable to store the deceleration factor of the camera movement.
    */
    public float decelerationFactor = 0.95f;

    /**
    * Float variable for camera movement calculations.
    */
    private float currentHorizontal;
    /**
    * Float variable for camera movement calculations.
    */
    private float currentVertical;
    /**
    * Float variable for camera movement velocity calculations.
    */
    private float horizontalVelocity;
    /**
    * Float variable for camera movement velocity calculations.
    */
    private float verticalVelocity;

    /**
     * Enum to store the state of the level.
     */
    public enum LevelStates
    {
        Default,
        Finish,
        NextScene
    }
    public LevelStates levelState = LevelStates.Default;

    void Awake()
    {
        scene = GameObject.FindGameObjectWithTag("Scene");
    }

    /**
    * A member function called at the start of the scene.
    */
    void Start()
    {
        mouseSensitivity = 30f;

        if (freeLookCamera is not null)
        {
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
        }
    }

    /**
    * A member function causing an update every frame.
    */
    void Update()
    {
        switch (levelState)
        {
            case LevelStates.Default:
                CameraMovement();
                break;
            case LevelStates.Finish:
                levelState = LevelStates.NextScene;
                break;
            case LevelStates.NextScene:
                break;
        }
    }

    /**
    * Method responsible for camera movement.
    */
    void CameraMovement()
    {
        if (freeLookCamera is not null)
        {
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X") / Screen.width * 100f;
                float mouseY = Input.GetAxis("Mouse Y") / Screen.height;

                float targetHorizontal = mouseX * mouseSensitivity;
                float targetVertical = mouseY * mouseSensitivity;

                currentHorizontal = Mathf.SmoothDamp(currentHorizontal, targetHorizontal, ref horizontalVelocity, smoothingFactor * Time.deltaTime * 10f);
                currentVertical = Mathf.SmoothDamp(currentVertical, targetVertical, ref verticalVelocity, smoothingFactor * Time.deltaTime);
            }
            else
            {
                currentHorizontal *= decelerationFactor;
                currentVertical *= decelerationFactor;

                if (Mathf.Abs(currentHorizontal) < 0.01f)
                {
                    currentHorizontal = 0f;
                }
                if (Mathf.Abs(currentVertical) < 0.01f)
                {
                    currentVertical = 0f;
                }
            }
            freeLookCamera.m_XAxis.Value += currentHorizontal;
            freeLookCamera.m_YAxis.Value -= currentVertical;
        }
    }


    /**
     * Method for loading the next level.
     */
    public void NextLevel()
    {
        GameManager.Instance.LoadNextLevel();
    }

    /**
     * Method for returning to the main menu.
     */
    public void MainMenu()
    {
        GameManager.Instance.MainMenu();
    }

    /**
     * Method for opening a specific scene.
     * 
     * @param level The level to be opened.
     */
    public void OpenScene(int level)
    {
        GameManager.Instance.OpenLevel(level);
        Time.timeScale = 1f;
        GameManager.Instance.setGamePause(false);
    }


    /**
    * A public member function that sets the scoreboard.
    * It sets the visible player scores accordingly to those saved by the Game Manager.
    */
    public void setScoreboard()
    {
        int[] arr = GameManager.Instance.playerStrokesArray;
        GameObject scoresObject = GameObject.Find("Scores");
        Text scoresText = scoresObject.GetComponent<Text>();
        scoresText.text = $"{arr[0]}   {arr[1]}   {arr[2]}   {arr[3]}   {arr[4]}   {arr[5]}   {arr[6]}   {arr[7]}   {arr[8]}   {arr[9]}";
    }

    
    /**
    * A public member function that toggles the visibility of the scoreboard.
    */
    public void toggleScoreboard()
    {
        if (GameManager.Instance.isGamePaused())
        {
            return;
        }

        if (GameManager.Instance.scoreboard.activeInHierarchy)
        {
            GameManager.Instance.scoreboard.SetActive(false);
        }
        else
        {
            GameManager.Instance.scoreboard.SetActive(true);
            //setScoreboard();
        }
    }
}
