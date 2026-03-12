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

    private int score = 0;
    private int highScore = 0;
    
    private bool _inCoroutine = false;

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

    public void AddScore()
    {
        score++;
        scoreText.text = "SCORE: " + score.ToString();
        if (score > highScore)
        {
            PlayerPrefs.SetInt("highScore", score);
        }
    }

    //private void Update()
    //{
    //    int random = Random.Range(0, 100);
    //    
    //    if(random == 33)
    //    {
    //        CallCoroutine();
    //    }
    //}

    //public void CallCoroutine()
    //{
    //    if (!_inCoroutine)
    //    {
    //        StartCoroutine(DoubleScore());
    //    }
    //}
    
    IEnumerator DoubleScore()
    {
        while(true)
        {
            _inCoroutine = true;
            
            score++;
            
            _inCoroutine = false;
        }
    }
    
    
}
