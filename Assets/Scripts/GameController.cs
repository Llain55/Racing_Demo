using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text scoreText;
    public Text timeText;
    public GameObject gameOverText;
    public Text gameOverScore;
    public int score = 0;
    public float playTime = 100;
    public bool timerRunning = false;

    public int noOfBoostFuels = 3;
    public int noBoostInScene = 3;
    public GameObject boostFuelPrefab;
    public CarController car;
    // Start is called before the first frame update
    void Start()
    {
        gameOverText.SetActive(false);
        timerRunning = true;
        GenerateBoostFuel();
    }

    private void Update() 
    {
        //AirTime();

        if(timerRunning)
        {
            UpdateTime();
        }

        if (car.boostFuel <= 0 || !timerRunning)
        {
            GameOver();
        }

        if (noBoostInScene == 0)
        {
            GenerateBoostFuel();
            noBoostInScene = 3;
        }
    }

    public void Restart()
    {
        if (car.boostFuel <= 0 || !timerRunning)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
    private void UpdateTime()
    {
        if (playTime > 0)
        {
            playTime -= Time.deltaTime;
            DisplayTime(playTime);
        }
        else
        {
            Debug.Log("Time has run out. Game over");
            playTime = 0;
            timerRunning = false;
        }
    }

    void DisplayTime(float time)
    {
        time +=1;
        string minutes = Mathf.FloorToInt(time / 60).ToString("00");
        string seconds = Mathf.FloorToInt(time % 60).ToString("00");

        string timeDisp = "Time Left: " + minutes + ":" + seconds;
        timeText.text = timeDisp;
        //Debug.Log(timeDisp);
    }
    public void AirTime()
    {
        score += 1;
        scoreText.text ="Score: " + score.ToString("000,000");
        //Debug.Log(score);
    }

    public void DriftScore()
    {
        score+=1;
        scoreText.text = "Score: " + score.ToString("000,000");
    }
        public void DriftScoreBonus()
    {
        score+=100;
        scoreText.text = "Score: " + score.ToString("000,000");
    }

    void GenerateBoostFuel()
    {
        for(int i = 0; i<noOfBoostFuels;i++)
        {
            Vector3 randVector = new Vector3(Random.Range(-140, 140), 0, Random.Range(-140, 140));
            Instantiate(boostFuelPrefab,randVector,Quaternion.identity);
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverText.SetActive(true);
        gameOverScore.text = score.ToString();
    }
}
