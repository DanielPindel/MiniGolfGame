using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Switch;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    public AudioClip buttonSoundClip;
    public AudioClip startLevelSFXClip;
    public Sprite musicOffIcon;
    public Sprite musicOnIcon;

    private AudioSource bgMusic;
    public bool wasMusicToggledOff;
    private GameObject musicButton;

    GameObject ball;
    GameObject cards;
    public int[] playerStrokesArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] playerBestScore = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public GameObject holeUI;
    public TextMeshProUGUI strokesText;
    public TextMeshProUGUI holeUITitle;
    public ParticleSystem confettiParticles;
    public GameObject scoreboard;
    public AudioSource soundFXObject;

    public static bool gameIsPaused = false;

    // Temporary, for testing
    public bool showCards = false;

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

        bgMusic = this.GetComponent<AudioSource>();
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
        switch (state)
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
        switch (state)
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
        if(showCards)
        {
            ShowCards();
        }
        if (startNextScene)
        {
            startNextScene = false;
            LoadNextLevel();
        }
    }


    private void StartLevel()
    {
        playStartLevelSFX();
        ball = GameObject.FindGameObjectWithTag("Player");
        holeUI = GameObject.Find("HoleUI");
        strokesText = GameObject.Find("StrokesText").GetComponent<TextMeshProUGUI>();
        holeUITitle = GameObject.Find("HoleUITitle").GetComponent<TextMeshProUGUI>();
        scoreboard = GameObject.Find("Scoreboard");
        musicButton = GameObject.Find("SoundButton");
        musicButton.GetComponent<UnityEngine.UI.Image>().sprite = musicOnIcon;
        levelNumber = GetCurrentLevelNumber();
        nextLevel = "Level" + (levelNumber + 1);
        strokesText.SetText("0");
        holeUI.SetActive(false);
        strokesText.enabled = true;
        scoreboard.SetActive(false);
        cards = GameObject.FindGameObjectWithTag("Cards");
        resetStrokes();
        if (!bgMusic.isPlaying)
        {
            if (!wasMusicToggledOff)
            {
                bgMusic.Play();
            }
            else
            {
                musicButton.GetComponent<UnityEngine.UI.Image>().sprite = musicOffIcon;
            }
        }
    }



    public void LoadNextLevel()
    {
        state = SceneNameToEnum(nextLevel);
        SceneManager.LoadScene(SceneNameToString(state));
    }
    public void MainMenu()
    {
        pauseBGMusic();
        state = GameStates.MainMenu;
        SceneManager.LoadScene(SceneNameToString(state));
    }

    public void setHoleUIActive(bool isActive)
    {
        holeUI.SetActive(isActive);
        if (isActive)
        {
            holeUITitle.SetText("HOLE IN " + strokesText.text + "!");
            strokesText.enabled = false;
            compareWithBestScore();
        }
    }

    public bool isHoleUIActive() { return holeUI.activeSelf; }

    public int GetCurrentLevelNumber()
    {
        string levelName = SceneNameToString(state);
        try
        {
            int levelNumber = Int32.Parse(levelName.Substring(levelName.Length - 1));
            if (levelNumber == 0)
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

    private void compareWithBestScore()
    {
        if (playerBestScore[(int)levelNumber - 1] > playerStrokesArray[(int)levelNumber - 1] || playerBestScore[(int)levelNumber - 1] == 0)
        {
            playerBestScore[(int)levelNumber - 1] = playerStrokesArray[(int)levelNumber - 1];
        }
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

    public bool isGamePaused()
    {
        return gameIsPaused;
    }

    public void setGamePause(bool pauseGame)
    {
        gameIsPaused = pauseGame;
    }

    public void playConfettiParticles()
    {
        confettiParticles.Play();
    }

    public void playRandomSFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        int rand = UnityEngine.Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[rand];
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void playAllSFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        for(int i = 0; i < audioClips.Length; i++)
        {
            audioSource.clip = audioClips[i];
            audioSource.volume = volume;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);

        }
    }

    public void playSFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void playButtonClick()
    {
        playSFXClip(buttonSoundClip, transform, 1f);
    }

    public void playStartLevelSFX()
    {
        playSFXClip(startLevelSFXClip, transform, 1f);
    }

    public void ShowCards()
    {
        cards.GetComponent<CardsController>().ShowCards();
    }

    public void HideArrow()
    {
        LineRenderer lineRenderer = ball.GetComponent<LineRenderer>();
        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        startColor.a = 0;
        endColor.a = 0;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    public void BoostBallPower()
    {
        ball.GetComponent<BallController>().RaiseForceMultiplier();
    }

    public void InverseControls()
    {
        ball.GetComponent<BallController>().InverseForce();
    }

    public void EnvironmentFog(bool active)
    {
        GameObject fog = GameObject.Find("Fog");
        Transform fogTransform = fog.transform.Find("FogPlane");
        fogTransform.gameObject.SetActive(active);

        var urp = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
        urp.supportsCameraDepthTexture = active;
        RenderSettings.fog = active;
    }

    public void stopBGMusic()
    {
        bgMusic.Stop();
    }
    public void pauseBGMusic()
    {
        bgMusic.Pause();
    }
    public void playBGMusic()
    {
        bgMusic.Play();
    }
    public void toggleBGMusic()
    {
        if(bgMusic.isPlaying)
        {
            bgMusic.Pause();
            wasMusicToggledOff = true;
            musicButton.GetComponent<UnityEngine.UI.Image>().sprite = musicOffIcon;
        }
        else
        {
            bgMusic.Play();
            wasMusicToggledOff = false;
            musicButton.GetComponent<UnityEngine.UI.Image>().sprite = musicOnIcon;
        }
    }
    public void setBGMusicVolume(float volume)
    {
        bgMusic.volume = volume;
    }
    public float getBGMusicVolume()
    {
        return bgMusic.volume;
    }


}
