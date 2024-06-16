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

    /**
    * A public Audio Clip variable for storing button click sound effect.
    */
    public AudioClip buttonSoundClip;

    /**
    * A public Audio Clip variable for storing the sound effect for starting the level.
    */
    public AudioClip startLevelSFXClip;

    /**
    * A public Sprite variable for storing the music-off icon.
    */
    public Sprite musicOffIcon;

    /**
    * A public Sprite variable for storing the music-on icon.
    */
    public Sprite musicOnIcon;

    /**
    * A private Audio Source variable for storing the background music.
    */
    private AudioSource bgMusic;

    /**
    * A public bool variable for checking whether the music was toggled off by the user.
    */
    public bool wasMusicToggledOff;

    /**
    * A private Game Object variable and referencing the music button.
    */
    private GameObject musicButton;

    /**
    * A public int array for storing player stroke scores.
    */
    public int[] playerStrokesArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    /**
    * A public int array for storing player best stroke scores.
    */
    public int[] playerBestScore = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    /**
    * A public Game Object variable and referencing the Hole UI.
    */
    public GameObject holeUI;

    /**
    * A public Text variable for referencing the strokes text.
    */
    public TextMeshProUGUI strokesText;

    /**
    * A public Text variable for referencing the Hole UI title.
    */
    public TextMeshProUGUI holeUITitle;

    /**
    * A public Text variable for referencing the Hole UI title.
    */
    public TextMeshProUGUI levelNumberText;

    /**
    * A public Game Object variable for referencing the scoreboard.
    */
    public GameObject scoreboard;

    /**
    * A public Audio Source variable for referencing the sound effect object.
    */
    public AudioSource soundFXObject;

    /**
    * A public bool variable for checking whether the game is paused.
    */
    public static bool gameIsPaused = false;

    public bool showCards = false;
    public bool blockCards = false;

    /**
    * A private Game Object variable for referencing the ball.
    */
    GameObject ball;

    /**
    * A private Game Object variable for referencing the cards.
    */
    GameObject cards;

    GameObject scoresObject;


    bool startNextScene;
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


    private void LevelLoop()
    {
        if(showCards && !blockCards)
        {
            showCards = false;
            ShowCards();
        }
        if (startNextScene)
        {
            startNextScene = false;
            LoadNextLevel();
        }
    }

    /**
    * A private member function for starting each level.
    * It assigns appropriate values to many different variables at the start of the level scene, as well as toggles some objects on or off.
    */
    private void StartLevel()
    {
        playStartLevelSFX();
        ball = GameObject.FindGameObjectWithTag("Player");
        holeUI = GameObject.Find("HoleUI");
        strokesText = GameObject.Find("StrokesText").GetComponent<TextMeshProUGUI>();
        holeUITitle = GameObject.Find("HoleUITitle").GetComponent<TextMeshProUGUI>();
        levelNumberText = GameObject.Find("LevelNumberText").GetComponent<TextMeshProUGUI>();
        scoreboard = GameObject.Find("Scoreboard");
        scoresObject = GameObject.Find("Scores");
        musicButton = GameObject.Find("SoundButton");
        musicButton.GetComponent<UnityEngine.UI.Image>().sprite = musicOnIcon;
        levelNumber = GetCurrentLevelNumber();
        levelNumberText.SetText($"Level {levelNumber}");
        nextLevel = "Level" + (levelNumber + 1);
        strokesText.SetText("0");
        holeUI.SetActive(false);
        strokesText.enabled = true;
        scoreboard.SetActive(false);
        cards = GameObject.FindGameObjectWithTag("Cards");
        resetStrokes();
        ResetCardEffects();
        cards.GetComponent<CardsController>().ResetUsedCards();
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

    /**
     * A public member function that toggles the HoleUI (final menu).
     * It sets the HoleUI and strokes text visibility and modifies the HoleUI title text 
     * with the amount of strokes player has won with.
     * @param isActive a bool value for toggling the HoleUI on and off.
     */
    public void setHoleUIActive(bool isActive)
    {
        holeUI.SetActive(isActive);
        if (isActive)
        {
            holeUITitle.SetText("HOLE IN " + strokesText.text + "!");
            strokesText.enabled = false;
            compareWithBestScore();
            showCards = false;
            HideCards();
            setScoreboard();
        }
    }

    /**
     * A public member function that gets the HoleUI (final menu) activity value.
     * @return the HoleUI activity value.
     */
    public bool isHoleUIActive() { return holeUI.activeSelf; }

    /**
     * A public member function that gets the number of the currently active level.
     * @return the currently active level number.
     */
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

    /**
     * A public member function that adds 1 to array with player strokes.
     * The array index where the number is added is the currently active level number.
     */
    public void addStrokeToCurrentLevel()
    {
        playerStrokesArray[(int)levelNumber - 1]++;
        strokesText.SetText(playerStrokesArray[(int)levelNumber - 1].ToString());
        if (playerStrokesArray[(int)levelNumber - 1] % 5 == 0 && !blockCards) 
        {
            showCards = true;
        }
        //setScoreboard();
    }

    /**
    * A private member function that compares current score with best (lowest) score saved.
    */
    private void compareWithBestScore()
    {
        if (playerBestScore[(int)levelNumber - 1] > playerStrokesArray[(int)levelNumber - 1] || playerBestScore[(int)levelNumber - 1] == 0)
        {
            playerBestScore[(int)levelNumber - 1] = playerStrokesArray[(int)levelNumber - 1];
        }
    }

    /**
    * A public member function that resets player strokes for the currently active level.
    */
    public void resetStrokes()
    {
        playerStrokesArray[(int)levelNumber - 1] = 0;
        strokesText.SetText("0");
        strokesText.enabled = true;
    }

    /**
    * A public member function that loads a level scene given by the id.
    * @param id of the level to be opened
    */
    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        state = SceneNameToEnum(levelName);
        SceneManager.LoadScene(levelName);
    }

    /**
    * A public member function that returns whether the game is currently paused.
    * @return the bool value of whether the game is paused.
    */
    public bool isGamePaused()
    {
        return gameIsPaused;
    }

    /**
    * A public member function that toggles the game pause.
    * @param the bool value of desired toggle (on or off)
    */
    public void setGamePause(bool pauseGame)
    {
        gameIsPaused = pauseGame;
    }


    /**
    * A public member function that plays a random sound effect clip from a given array.
    * @param audioClips the array of sound clips to choose from.
    * @param spawnTransform a variable for position for instantiating the AudioSource object.
    * @param volume the volume of the sound effect played.
    */
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

    /**
    * A public member function that plays all sound effect clips from a given array.
    * @param audioClips the array of sound clips to play.
    * @param spawnTransform a variable for position for instantiating the AudioSource object.
    * @param volume the volume of the sound effects played.
    */
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

    /**
    * A public member function that plays a given sound effect clip.
    * @param audioClip sound clip to play.
    * @param spawnTransform a variable for position for instantiating the AudioSource object.
    * @param volume the volume of the sound effect played.
    */
    public void playSFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    /**
    * A public member function that plays the button click sound effect clip.
    */
    public void playButtonClick()
    {
        playSFXClip(buttonSoundClip, transform, 1f);
    }

    /**
    * A public member function that plays the sound effect clip for starting a level.
    */
    public void playStartLevelSFX()
    {
        playSFXClip(startLevelSFXClip, transform, 1f);
    }

    public void ShowCards()
    {
        cards.GetComponent<CardsController>().ShowCards();
    }

    public void HideCards()
    {
        cards.GetComponent<CardsController>().HideCards();
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

    public void MagnetForce()
    {
        ball.GetComponent<BallController>().ActivateMagnet();
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

    public void SpinArrow()
    {
        ball.GetComponent<BallController>().ActivateSpinningArrow(true);
    }

    public void stopBGMusic()
    {
        bgMusic.Stop();
    }

    /**
    * A public member function that pauses background music.
    */
    public void pauseBGMusic()
    {
        bgMusic.Pause();
    }

    /**
    * A public member function that plays background music.
    */
    public void playBGMusic()
    {
        bgMusic.Play();
    }

    /**
    * A public member function that toggles background music.
    * It checks whether the background music is playing and sets the opposite state, 
    * while also changing music toggle icon image.
    */
    public void toggleBGMusic()
    {
        if (gameIsPaused)
        {
            return;
        }
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

    /**
    * A public member function that sets background music volume.
    * @param volume the volume for background music.
    */
    public void setBGMusicVolume(float volume)
    {
        bgMusic.volume = volume;
    }

    /**
    * A public member function that reads background music volume.
    * @return background music volume.
    */
    public float getBGMusicVolume()
    {
        return bgMusic.volume;
    }

    public void FreezeInputs(bool active)
    {
        ball.GetComponent<BallController>().isInputActive = active;
    }

    public void ResetCardEffects()
    {
        // HideArrow
        LineRenderer lineRenderer = ball.GetComponent<LineRenderer>();
        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        startColor.a = 1;
        endColor.a = 1;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;

        // BallPower
        ball.GetComponent<BallController>().forceMultiplier = 20f;

        // InverseControls
        if(ball.GetComponent<BallController>().forceMultiplier < 0)
        {
            ball.GetComponent<BallController>().InverseForce();
        }

        // Fog
        EnvironmentFog(false);

        //SpinningArrow
        //ball.GetComponent<BallController>().ActivateSpinningArrow(false);

        cards.GetComponent<CardsController>().ResetUsedCards();
    }

    public void toggleCards()
    {
        blockCards = !blockCards;
        if(blockCards)
        {
            Debug.Log("Blocking cards");
        }
        else
        {
            Debug.Log("Unblocking cards");
        }
    }

    public bool cardsAreBlocked()
    {
        return blockCards;
    }

    public void setScoreboard()
    {
        int[] arr = playerStrokesArray;
        Text scoresText = scoresObject.GetComponent<Text>();
        scoresText.text = $"{arr[0]}   {arr[1]}   {arr[2]}   {arr[3]}   {arr[4]}   {arr[5]}   {arr[6]}   {arr[7]}   {arr[8]}   {arr[9]}";
    }

    

}
