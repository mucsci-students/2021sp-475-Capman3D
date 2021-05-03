using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public static bool GameIsPaused = false;
	public GameObject pauseMenu;
	public GameManager gameManager;
	
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && gameManager.resultsTimer == -1)
		{
			if(GameIsPaused)
			{
				Resume();
			}
			else if (Time.timeScale > 0) 
			{
				PauseGame();
			}
		}
    }
	
	public void Resume()
	{
		pauseMenu.SetActive(false);
		Time.timeScale = 1f;
		GameIsPaused = false;
	}
	
	public void PauseGame()
	{
		pauseMenu.SetActive(true);
		Time.timeScale = 0f;
		GameIsPaused = true;
	}
	
	public void Pausing()
	{
		Time.timeScale = 0f;
		GameIsPaused = true;
	}
	
	public void LoadMenu()
	{
		GameIsPaused = false;
		Time.timeScale = 1f;
	}
	
}
