using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;

    void Update()
    {
        if (GameManager.Instance.isHoleUIActive())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameManager.Instance.isGamePaused())
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        StartCoroutine(blockInputAfterResume());
    }

    // Without this coroutine clicking on the Resume button makes the ball move, so this function waits a bit
    // before setting the game pause to false, which blocks the mouse input for the ball in the BallController
    IEnumerator blockInputAfterResume()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.setGamePause(false);
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.setGamePause(true);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadSettings()
    {
        pauseMenuUI.SetActive(false);

    }

    public void Exit()
    {
        Application.Quit();
    }

}
