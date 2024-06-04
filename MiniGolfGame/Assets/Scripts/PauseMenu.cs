using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    /**
    * A public Game Object for referencing Pause Menu UI.
    */
    public GameObject pauseMenuUI;

    /**
    * A member function causing an update every frame.
    */
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

    /**
    * A public member function for resuming the game.
    * It resumes the game and unpauses the background music if it was not toggled off.
    */
    public void Resume()
    {
        if (!GameManager.Instance.wasMusicToggledOff)
        {
            GameManager.Instance.playBGMusic();
        }
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        StartCoroutine(blockInputAfterResume());
    }

    /**
    * A private coroutine to block ball input for a short period of time after resuming back to the game.
    */
    IEnumerator blockInputAfterResume()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.setGamePause(false);
    }

    /**
    * A public member function for pausing the game.
    * It pauses both the game and the background music.
    */
    void Pause()
    {
        GameManager.Instance.pauseBGMusic();
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.setGamePause(true);
    }

    /**
    * A public member function for toggling the background music.
    */
    public void toggleMusic()
    {
        GameManager.Instance.toggleBGMusic();
    }


    /*public void LoadSettings()
    {
        pauseMenuUI.SetActive(false);

    }*/

    /**
    * A public member function for exiting the program.
    */
    public void Exit()
    {
        UnityEngine.Application.Quit();
    }

}
