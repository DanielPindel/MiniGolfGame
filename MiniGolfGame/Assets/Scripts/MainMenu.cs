using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        GameManager.Instance.playSFXClip(GameManager.Instance.buttonSoundClip, transform, 1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void changeMusicVolume()
    {
        Slider slider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        GameManager.Instance.setBGMusicVolume(slider.value);
    }

    public void setSliderValue()
    {
        Slider slider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        slider.value = GameManager.Instance.getBGMusicVolume();
    }

    public void Exit()
    {
       
    }
}
