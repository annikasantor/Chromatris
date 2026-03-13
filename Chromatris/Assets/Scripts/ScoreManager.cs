using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    public Piece piece;

    public static int score = 0;
    private int highScore = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("highScore", 0);
        scoreText.text = "SCORE: " + score.ToString();
        highScoreText.text = "HIGHSCORE: " + highScore.ToString();
    }

    //public static bool addScore = false;
    public void AddScore()
    {
        //int doubleScore = Piece.Score;
        
        //addScore = true;
        score++;
        //score += doubleScore;
        scoreText.text = "SCORE: " + score.ToString();
        if (score > highScore)
        {
            PlayerPrefs.SetInt("highScore", score);
        }
        //addScore = false;
    }
    
    
}
