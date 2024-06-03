using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScene : MonoBehaviour
{
    GameObject scene;

#nullable enable
    public CinemachineFreeLook? freeLookCamera;
    public float mouseSensitivity = 5f;
    public float smoothingFactor = 2f;
    public float decelerationFactor = 0.95f;

    private float currentHorizontal;
    private float currentVertical;
    private float horizontalVelocity;
    private float verticalVelocity;

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

    void Start()
    {
        if(freeLookCamera is not null)
        {
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
        }
    }

    void Update()
    {
        switch(levelState)
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

    void CameraMovement()
    {
        if(freeLookCamera is not null)
        {
            if (Input.GetMouseButton(1))
            {
                float targetHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 100f;
                float targetVertical = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

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



    public void NextLevel()
    {
        GameManager.Instance.LoadNextLevel();
    }

    public void MainMenu()
    {
        GameManager.Instance.MainMenu();
    }

    public void OpenScene(int level)
    {
        GameManager.Instance.OpenLevel(level);
        Time.timeScale = 1f;
        GameManager.Instance.setGamePause(false);
    }
}
