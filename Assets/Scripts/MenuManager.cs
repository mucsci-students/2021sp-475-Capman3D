using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private PlayfabManager playfabManager;
    public GameObject playfabScreen;
    public GameObject mainMenu;
    public Text connectingText;
    public GameObject namePrompt;
    public InputField inputField;
    public GameObject options;
    public Text displayName;
    public Image blackCover;

    public GameObject leaderboard1;
    public GameObject leaderboard2;
    public GameObject leaderboard3;

    public Text[] level1Names;
    public Text[] level1Scores;
    public Text[] level2Names;
    public Text[] level2Scores;
    public Text[] level3Names;
    public Text[] level3Scores;

    private bool hasNameToSend = false;
    private float fadeOutTimer = -1;
    private int levelToLoad;

    private AudioSource[] audio;

    public Slider volumeSlider;
    public VolumeValueChange musicPlayerPrefab;
    private VolumeValueChange musicPlayer;

    void Start()
    {
        audio = GetComponents<AudioSource>();
        playfabManager = (PlayfabManager)GameObject.FindObjectOfType(typeof(PlayfabManager));
        DontDestroyOnLoad(playfabManager);
        playfabManager.Login();
        if (!PlayerPrefs.HasKey("Name") || PlayerPrefs.GetString("Name") == "")
        {
            connectingText.gameObject.SetActive(false);
            namePrompt.SetActive(true);
            inputField.gameObject.SetActive(true);
        }
        else
        {
            hasNameToSend = true;
        }

        if (GameObject.FindObjectOfType(typeof(VolumeValueChange)) == null)
        {
            musicPlayer = Instantiate(musicPlayerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            musicPlayer = (VolumeValueChange)GameObject.FindObjectOfType(typeof(VolumeValueChange));
        }
        volumeSlider.value = musicPlayer.musicVolume;
    }

    void Update()
    {
        if (playfabManager.isLoggedIn && hasNameToSend)
        {
            displayName.text = "Nickname: " + PlayerPrefs.GetString("Name");
            playfabManager.UpdateDisplayName(PlayerPrefs.GetString("Name"));
            hasNameToSend = false;
        }
        if (playfabManager.hasDisplayName && !leaderboard1.activeSelf)
        {
            leaderboard1.SetActive(true);
            playfabManager.displayedScore = false;
            playfabManager.GetLeaderboard(1, level1Names, level1Scores);
        }
        if (leaderboard1.activeSelf && playfabManager.displayedScore && !leaderboard2.activeSelf)
        {
            leaderboard2.SetActive(true);
            playfabManager.displayedScore = false;
            playfabManager.GetLeaderboard(2, level2Names, level2Scores);
        }
        if (leaderboard2.activeSelf && playfabManager.displayedScore && !leaderboard3.activeSelf)
        {
            leaderboard3.SetActive(true);
            playfabManager.displayedScore = false;
            playfabManager.GetLeaderboard(3, level3Names, level3Scores);
        }
        if (leaderboard3.activeSelf && playfabManager.displayedScore && !mainMenu.activeSelf && connectingText.gameObject.activeInHierarchy)
        {
            playfabScreen.SetActive(false);
            mainMenu.SetActive(true);
        }
        if (fadeOutTimer >= 0)
        {
            fadeOutTimer += Time.deltaTime;
            blackCover.color = new Color(0, 0, 0, fadeOutTimer * 2);
            if (fadeOutTimer > 1)
            {    
                PlayerPrefs.SetInt("RoundNumber", 1);
                PlayerPrefs.SetInt("Score", 0);
                PlayerPrefs.SetInt("Lives", 5);
                SceneManager.LoadScene(levelToLoad);
            }
        }

        if (!musicPlayer.playing && fadeOutTimer == -1)
        {
            musicPlayer.Play(0);
        }
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void StartLevel(int level)
    {
        musicPlayer.Stop();
        audio[0].Play();
        levelToLoad = level;
        blackCover.gameObject.SetActive(true);
        fadeOutTimer = 0;
    }

    public void SetNicknameStart()
    {
        if (inputField.text.Length > 0)
        {
            PlayerPrefs.SetString("Name", inputField.text);
            namePrompt.SetActive(false);
            inputField.gameObject.SetActive(false);
            connectingText.gameObject.SetActive(true);
            connectingText.text = "Updating nickname...";
            hasNameToSend = true;
        }
    }

    public void ChangeNickname()
    {
        options.SetActive(false);
        playfabScreen.SetActive(true);
        connectingText.gameObject.SetActive(false);
        namePrompt.SetActive(true);
        inputField.gameObject.SetActive(true);
        playfabManager.hasDisplayName = false;
        hasNameToSend = false;
    }

    public void QuitGame()
    {   
		Application.Quit();
    }
	
	public void SetVolume(float vol)
	{
		musicPlayer.SetVolume(vol);
	}
}
