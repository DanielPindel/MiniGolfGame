using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        GameManager.Instance.playSFXClip(GameManager.Instance.buttonSoundClip, transform, 1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
       
    }
}
