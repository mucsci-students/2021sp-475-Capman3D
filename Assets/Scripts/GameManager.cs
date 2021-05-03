using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public int playerScore;
	public PlayerScript player;
	public Image dangerOverlay;
	public Text freezeText;
	public EnemyWaypointNav[] spiders;
	public GameObject[] redGates;
	public GameObject[] yellowGates;
	public GameObject[] greenGates;
	public GameObject[] blueGates;
	public Text redDotsText;
	public Text yellowDotsText;
	public Text greenDotsText;
	public Text blueDotsText;
	public Image redLocked;
	public Image redUnlocked;
	public Image yellowLocked;
	public Image yellowUnlocked;
	public Image greenLocked;
	public Image greenUnlocked;
	public Image blueLocked;
	public Image blueUnlocked;
	public GameObject explosionPrefab;
	public GameObject deathScreen;
	public GameObject winScreen;
	public CameraScript mainCamera;
	public GameObject pickups;
	
	private Text scoreText;
	private Text lifeText;
	private int playerLives = 5;
	private float flashTimer = 0;
	private float freezeTimer = -1;
	private int redDotsRemaining;
	private int yellowDotsRemaining;
	private int greenDotsRemaining;
	private int blueDotsRemaining;
	private Pause pauseScript;

	private GameObject[] redDots;
	private GameObject[] yellowDots;
	private GameObject[] greenDots;
	private GameObject[] blueDots;

	AudioSource[] audio;

	public Canvas gameplayUi;
	public Canvas resultsScreen;

	public Text scoreLabel;
	public Text scoreNumbers;
	public Text roundLabel;
	public Text roundNumbers;
	public Text timeLabel;
	public Text timeNumbers;
	public Text livesLabel;
	public Text livesNumbers;
	public Text finalScoreNumbers;

	public GameObject scoreBonuses;
	public GameObject leaderboard;

	public float resultsTimer = -1;
	private float scoreTimer = -1;
	private float roundScoreTimer = -1;
	private float timeScoreTimer = -1;
	private float livesScoreTimer = -1;
	private float leaderboardTimer = -1;

	private int finalScore;
	private int targetScore;
	private int roundNumber;
	private float totalTime = 0;
	private float finalTime;
	private float leaderboardDist;

	private string finalMinutes;
	private string finalSeconds;
	private string finalMilliseconds;

	public Text[] scores;
	public Text[] names;

	public GameObject continueButton;
	public GameObject quitWarning;
	public Text resultsScreenLives;

	private PlayfabManager playfabManager;

	private bool startedScoreSend = false;
	private bool startedLeaderboardDisplay = false;

	private float continueTimer = -1;

	public GameObject startScreen;
	public Image blackCover;
	public Text roundText;
	public GameObject[] countdown;
	public GameObject goText;
	public Image blackCover2;

	private float startScreenTimer = 0;

	private VolumeValueChange musicPlayer;
	
    // Start is called before the first frame update
    void Start()
    {
		Time.timeScale = 0;
        scoreText = GameObject.Find("Score").GetComponent<Text>();
		lifeText = GameObject.Find("Lives").GetComponent<Text>();
		audio = GetComponents<AudioSource>();

		redDots = GameObject.FindGameObjectsWithTag("RedDot");
		redDotsRemaining = redDots.Length;
		redDotsText.text = redDotsRemaining.ToString();
		redUnlocked.enabled = false;
		
		yellowDots = GameObject.FindGameObjectsWithTag("YellowDot");
		yellowDotsRemaining = yellowDots.Length;
		yellowDotsText.text = yellowDotsRemaining.ToString();
		yellowUnlocked.enabled = false;
		
		greenDots = GameObject.FindGameObjectsWithTag("GreenDot");
		greenDotsRemaining = greenDots.Length;
		greenDotsText.text = greenDotsRemaining.ToString();
		greenUnlocked.enabled = false;
		
		blueDots = GameObject.FindGameObjectsWithTag("BlueDot");
		blueDotsRemaining = blueDots.Length;
		blueDotsText.text = blueDotsRemaining.ToString();
		blueUnlocked.enabled = false;
		
		pauseScript = GameObject.FindObjectOfType(typeof(Pause)) as Pause;
		
		resultsScreen.enabled = false;

		leaderboardDist = leaderboard.transform.localPosition.x;

		if (PlayerPrefs.HasKey("RoundNumber"))
		{
			roundNumber = PlayerPrefs.GetInt("RoundNumber");
		}
		else
		{
			roundNumber = 1;
		}
		roundText.text = "Round " + roundNumber;

		if (PlayerPrefs.HasKey("Score"))
		{
			playerScore = PlayerPrefs.GetInt("Score");
		}
		else
		{
			playerScore = 0;
		}
		scoreText.text = playerScore.ToString();

		if (PlayerPrefs.HasKey("Lives"))
		{
			playerLives = PlayerPrefs.GetInt("Lives");
		}
		else
		{
			playerLives = 5;
		}
		lifeText.text = "";
		for (int i = 0; i < playerLives; ++i)
		{
			lifeText.text += "1";
		}

		playfabManager = (PlayfabManager)GameObject.FindObjectOfType(typeof(PlayfabManager));

        musicPlayer = (VolumeValueChange)GameObject.FindObjectOfType(typeof(VolumeValueChange));
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = playerScore.ToString();
		totalTime += Time.deltaTime;

		if (playerLives == 1)
		{
			flashTimer += Time.deltaTime;
			if (flashTimer > 1)
			{
				flashTimer -= 1;
			}
			dangerOverlay.color = new Color(1, 0, 0, -0.05f * Mathf.Cos(2 * Mathf.PI * flashTimer) + 0.05f);
		}
		else
		{
			flashTimer = 0;
			dangerOverlay.color = new Color(1, 0, 0, 0);
		}
		
		if (playerLives == 0 && resultsTimer == -1)
		{
			musicPlayer.Stop();
			audio[4].Play();
			deathScreen.SetActive(true);
			player.Kill();
			freezeText.enabled = false;
			continueButton.SetActive(false);
			quitWarning.SetActive(false);
			resultsTimer = 0;
		}

		if (freezeTimer > 0)
		{
			foreach (EnemyWaypointNav spider in spiders)
			{
				spider.realSpeed = 0;
			}
			freezeText.text = ((int)freezeTimer + 1).ToString();

			freezeTimer -= Time.deltaTime;
			if (freezeTimer <= 0)
			{
				freezeTimer = -1;
				freezeText.text = "";
				foreach (EnemyWaypointNav spider in spiders)
				{
					spider.realSpeed = spider.speed;
				}
			}
		}

		if ((spiders[0].speed == 0 && 
			spiders[1].speed == 0 &&
			spiders[2].speed == 0 &&
			spiders[3].speed == 0) && resultsTimer == -1)
		{
			musicPlayer.Stop();
			audio[3].Play();
			winScreen.SetActive(true);
			freezeText.enabled = false;
			finalTime = totalTime;
			resultsTimer = 0;
		}
		
		if (resultsTimer >= 0)
		{
			resultsTimer += Time.deltaTime;
			if (resultsTimer > 4 && gameplayUi.enabled)
			{
				player.Kill();
				resultsScreenLives.text = "";
				for (int i = 0; i < playerLives; ++i)
				{
					resultsScreenLives.text += "1";
				}
				gameplayUi.enabled = false;
				resultsScreen.enabled = true;
				mainCamera.isTracking = false;
				if (SceneManager.GetActiveScene().buildIndex == 1)
				{
					mainCamera.transform.position = new Vector3(-48, 4, -38);
					mainCamera.transform.eulerAngles = new Vector3(10, 390, 0);
				}
				else if (SceneManager.GetActiveScene().buildIndex == 2)
				{
					mainCamera.transform.position = new Vector3(-50, 6, 50);
					mainCamera.transform.eulerAngles = new Vector3(31.5f, 479, 1);
				}
				else
				{
					mainCamera.transform.position = new Vector3(28, 56, 23.71f);
					mainCamera.transform.eulerAngles = new Vector3(60, 270, 0);
				}
				player.enabled = false;
				pickups.SetActive(false);
				foreach(EnemyWaypointNav spider in spiders)
				{
					spider.gameObject.SetActive(false);
				}
				musicPlayer.Play(4);
				scoreTimer = 0;
				targetScore = playerScore;
			}
		}

		if (scoreTimer >= 0)
		{
			scoreTimer += Time.deltaTime;
			if (scoreTimer > 1)
			{
				if (scoreLabel.text == "")
				{
					audio[1].Play();
				}
				scoreLabel.text = "Score";
				scoreNumbers.text = playerScore.ToString();
				if (CountScore(false))
				{
					scoreTimer = -1;
					if (playerLives > 0)
					{
						roundScoreTimer = 0;
						targetScore = finalScore + (roundNumber * 10000);
					}
					else
					{
						leaderboardTimer = 0;
					}
				}
				finalScoreNumbers.text = finalScore.ToString();
			}
		}

		if (roundScoreTimer >= 0)
		{
			roundScoreTimer += Time.deltaTime;
			if (roundScoreTimer > 1)
			{
				if (roundLabel.text == "")
				{
					audio[1].Play();
				}
				roundLabel.text = "Round " + roundNumber + " Completed";
				roundNumbers.text = (roundNumber * 10000).ToString();
				if (CountScore(true))
				{
					roundScoreTimer = -1;
					timeScoreTimer = 0;
					targetScore = finalScore + (int)Mathf.Round((Mathf.Max(0, 600 - finalTime) * 100));

					int totalMilliseconds = (int)Mathf.Round(finalTime * 1000);
					finalMinutes = (totalMilliseconds / 60000).ToString();

					int numSeconds = (totalMilliseconds % 60000) / 1000;
					if (numSeconds >= 10)
					{
						finalSeconds = numSeconds.ToString();
					}
					else
					{
						finalSeconds = "0" + numSeconds.ToString();
					}
					
					int numMilliseconds = totalMilliseconds % 1000;
					if (numMilliseconds >= 100)
					{
						finalMilliseconds = numMilliseconds.ToString();
					}
					else if (numMilliseconds >= 10)
					{
						finalMilliseconds = "0" + numMilliseconds.ToString();
					}
					else
					{
						finalMilliseconds = "00" + numMilliseconds.ToString();
					}
				}
				finalScoreNumbers.text = finalScore.ToString();
			}
		}

		if (timeScoreTimer >= 0)
		{
			timeScoreTimer += Time.deltaTime;
			if (timeScoreTimer > 1)
			{
				if (timeLabel.text == "")
				{
					audio[1].Play();
				}
				timeLabel.text = "Time - " + finalMinutes + ":" + finalSeconds + "." + finalMilliseconds;
				timeNumbers.text = ((int)Mathf.Round((Mathf.Max(0, 600 - finalTime) * 100))).ToString();
				if (CountScore(true))
				{
					timeScoreTimer = -1;
					livesScoreTimer = 0;
					targetScore = finalScore + (playerLives * 10000);
				}
				finalScoreNumbers.text = finalScore.ToString();
			}
		}

		if (livesScoreTimer >= 0)
		{
			livesScoreTimer += Time.deltaTime;
			if (livesScoreTimer > 1)
			{
				if (livesLabel.text == "")
				{
					audio[1].Play();
				}
				string lifeWord = playerLives == 1 ? "Life" : "Lives";
				livesLabel.text = playerLives.ToString() + " " + lifeWord + " Remaining";
				livesNumbers.text = (playerLives * 10000).ToString();
				if (CountScore(true))
				{
					livesScoreTimer = -1;
					leaderboardTimer = 0;
				}
				finalScoreNumbers.text = finalScore.ToString();
			}
		}

		if (leaderboardTimer >= 0)
		{
			if (!startedScoreSend)
			{
				playfabManager.sentScore = false;
				playfabManager.SendLeaderboard(finalScore, SceneManager.GetActiveScene().buildIndex);
				startedScoreSend = true;
			}
			else if (playfabManager.sentScore && !startedLeaderboardDisplay)
			{
				leaderboardTimer += Time.deltaTime;
				if (leaderboardTimer >= 2)
				{
					playfabManager.displayedScore = false;
					playfabManager.GetLeaderboard(SceneManager.GetActiveScene().buildIndex, names, scores);
					startedLeaderboardDisplay = true;
				}
			}
			else if (playfabManager.displayedScore)
			{
				leaderboardTimer += Time.deltaTime;
				if (leaderboardTimer >= 2 && leaderboardTimer < 3)
				{
					scoreBonuses.transform.localPosition += Vector3.left * (leaderboardDist / 2 * Mathf.PI * Mathf.Sin(Mathf.PI * leaderboardTimer) * Time.deltaTime);
					leaderboard.transform.localPosition += Vector3.left * (leaderboardDist / 2 * Mathf.PI * Mathf.Sin(Mathf.PI * leaderboardTimer) * Time.deltaTime);
				}
				else if (leaderboardTimer >= 3)
				{
					scoreBonuses.transform.localPosition = Vector3.left * leaderboardDist;
					leaderboard.transform.localPosition = Vector3.zero;
					leaderboardTimer = -1;
				}
			}
		}

		if (continueTimer >= 0)
		{
			blackCover2.enabled = true;
			continueTimer += Time.deltaTime;
			if (continueTimer < 1 && playerLives < 5)
			{	
				resultsScreenLives.text = "";
				for (int i = 0; i < ((continueTimer % 0.1f < 0.05f) ? playerLives : playerLives + 1); ++i)
				{
					resultsScreenLives.text += "1";
				}
			}
			if (continueTimer > 1)
			{
				blackCover2.color = new Color(0, 0, 0, Mathf.Min(1, (continueTimer - 1) * 2));
			}
			if (continueTimer > 2)
			{
				AdvanceRound();
			}
		}

		if (startScreenTimer >= 0)
		{
			startScreenTimer += Time.unscaledDeltaTime;
			if (startScreenTimer > 1 && startScreenTimer <= 2)
			{
				blackCover.color = new Color(0, 0, 0, Mathf.Max(0, 1 - ((startScreenTimer - 1) * 4)));
			}
			else if (startScreenTimer > 2 && blackCover.enabled)
			{
				blackCover.enabled = false;
				countdown[0].SetActive(true);
				audio[5].Play();
			}
			else if (startScreenTimer > 3 && countdown[0].activeInHierarchy)
			{
				countdown[0].SetActive(false);
				countdown[1].SetActive(true);
				audio[5].Play();
			}
			else if (startScreenTimer > 4 && countdown[1].activeInHierarchy)
			{
				countdown[1].SetActive(false);
				countdown[2].SetActive(true);
				audio[5].Play();
			}
			else if (startScreenTimer > 5 && countdown[2].activeInHierarchy)
			{
				startScreen.SetActive(false);
				goText.SetActive(true);
				Time.timeScale = 1;
				audio[6].Play();
				musicPlayer.Play(SceneManager.GetActiveScene().buildIndex);
			}
			else if (startScreenTimer > 6)
			{
				goText.SetActive(false);
				startScreenTimer = -1;
			}
		}
    }
	
	public void CloseDeathScreen()
	{
		pauseScript.Resume();
		deathScreen.SetActive(false);
		resultsTimer = 0;
	}
	
	public void AddScore(int amt)
	{
		playerScore = playerScore + amt;
	}
	
	public void LoadMenu ()
	{
		musicPlayer.Stop();
		SceneManager.LoadScene("Menu");
	}
	
	public void LoadGame0 ()
	{
		SceneManager.LoadScene("Level1");
	}
	
	public void LoadGame1 ()
	{
		SceneManager.LoadScene("Level2");
	}
	
	public void LoadGame2 ()
	{
		SceneManager.LoadScene("Level3");
	}
	
	public void exitgame()
	{
		Debug.Log("exitgame");
		Application.Quit();
	}
	
	public void Death()
	{
		playerLives = playerLives - 1;
		updateLives();
		audio[0].Play();
	}
	
	public void addLife()
	{
		if(playerLives < 5)
		{
			playerLives = playerLives + 1;
			updateLives();
		}
	}

	public void Freeze()
	{
		freezeTimer = 9.999f;
	}
	
	private void updateLives()
	{
		if(playerLives == 0)
		{
			lifeText.text = "";
		} else if(playerLives == 1)
		{
			lifeText.text = "1";
		} else if(playerLives == 2)
		{
			lifeText.text = "11";
		} else if(playerLives == 3)
		{
			lifeText.text = "111";
		} else if(playerLives == 4)
		{
			lifeText.text = "1111";
		} else
		{
			lifeText.text = "11111";
		}
	}

	public void Collect(string type)
	{
		if (type == "RedDot")
		{
			--redDotsRemaining;
			if (redDotsRemaining == 0)
			{
				foreach (GameObject gate in redGates)
				{
					audio[8].Play();
					Instantiate(explosionPrefab, gate.transform.position, Quaternion.identity);
					Destroy(gate);
					redDotsText.text = "";
					redLocked.enabled = false;
					redUnlocked.enabled = true;
				}
			}
			else
			{
				redDotsText.text = redDotsRemaining.ToString();
			}
		} else if (type == "YellowDot")
		{
			--yellowDotsRemaining;
			if(yellowDotsRemaining == 0)
			{
				foreach (GameObject gate in yellowGates)
				{
					audio[8].Play();
					Instantiate(explosionPrefab, gate.transform.position, Quaternion.identity);
					Destroy(gate);
					yellowDotsText.text = "";
					yellowLocked.enabled = false;
					yellowUnlocked.enabled = true;
				}
			}
			else
			{
				yellowDotsText.text = yellowDotsRemaining.ToString();
			}
		} else if (type == "GreenDot")
		{
			--greenDotsRemaining;
			if(greenDotsRemaining == 0)
			{
				foreach (GameObject gate in greenGates)
				{
					audio[8].Play();
					Instantiate(explosionPrefab, gate.transform.position, Quaternion.identity);
					Destroy(gate);
					greenDotsText.text = "";
					greenLocked.enabled = false;
					greenUnlocked.enabled = true;
				}
			}
			else
			{
				greenDotsText.text = greenDotsRemaining.ToString();
			}
		} else if (type == "BlueDot")
		{
			--blueDotsRemaining;
			if(blueDotsRemaining == 0)
			{
				foreach (GameObject gate in blueGates)
				{
					audio[8].Play();
					Instantiate(explosionPrefab, gate.transform.position, Quaternion.identity);
					Destroy(gate);
					blueDotsText.text = "";
					blueLocked.enabled = false;
					blueUnlocked.enabled = true;
				}
			}
			else
			{
				blueDotsText.text = blueDotsRemaining.ToString();
			}
		}
	}

	private bool CountScore(bool shouldIterate)
	{
		audio[2].Play();
		if (finalScore < targetScore - 512 && shouldIterate)
		{
			finalScore += 512;
			return false;
		}
		if (finalScore < targetScore)
		{
			finalScore = targetScore;
			return false;
		}
		return true;
	}

	public void Continue()
	{
		continueTimer = 0;
		musicPlayer.Stop();
		audio[7].Play();
	}

	private void AdvanceRound()
	{
		PlayerPrefs.SetInt("RoundNumber", roundNumber + 1);
		PlayerPrefs.SetInt("Score", finalScore);
		PlayerPrefs.SetInt("Lives", Mathf.Min(playerLives + 1, 5));
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	// private void UpdateLeaderboard(int score, string name)
	// {
	// 	for (int i = 1; i <= 10; ++i)
	// 	{
	// 		if (!PlayerPrefs.HasKey("Score" + i.ToString()))
	// 		{
	// 			PlayerPrefs.SetInt("Score" + i.ToString(), score);
	// 			PlayerPrefs.SetString("Name" + i.ToString(), name);
	// 			break;
	// 		}
	// 		else if (PlayerPrefs.GetInt("Score" + i.ToString()) < score)
	// 		{
	// 			int prevScore = PlayerPrefs.GetInt("Score" + i.ToString());
	// 			string prevName = PlayerPrefs.GetString("Name" + i.ToString());
	// 			PlayerPrefs.SetInt("Score" + i.ToString(), score);
	// 			PlayerPrefs.SetString("Name" + i.ToString(), name);
	// 			UpdateLeaderboard(prevScore, prevName);
	// 			break;
	// 		}
	// 	}
	// 	for (int i = 1; i <= 10; ++i)
	// 	{
	// 		scores[i - 1].text = PlayerPrefs.GetInt("Score" + i.ToString()).ToString();
	// 		names[i - 1].text = PlayerPrefs.GetString("Name" + i.ToString());
	// 	}
	// }
}
