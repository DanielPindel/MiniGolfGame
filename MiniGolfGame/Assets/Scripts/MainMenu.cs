using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{

    /**
     * A public member function to read the music volume slider.
     * It sets the music volume.
     */
    public void changeMusicVolume()
    {
        Slider slider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        GameManager.Instance.setBGMusicVolume(slider.value);
    }

    /**
     * A public member function to keep the music volume slider at its last position.
     */
    public void setSliderValue()
    {
        Slider slider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        slider.value = GameManager.Instance.getBGMusicVolume();
    }

    /**
     * A public member function that exits the program.
     */
    public void Exit()
    {
        UnityEngine.Application.Quit();
    }
}
