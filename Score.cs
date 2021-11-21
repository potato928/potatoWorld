using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private Text scoreText;
    private int oldScore;

    void Start()
    {
        scoreText = GetComponent<Text>();
        if (scoreText != null && GManager.instance != null)
        {
            scoreText.text = "Score " + GManager.instance.score.ToString();
        }
    }

    void Update()
    {
        if (scoreText != null && GManager.instance != null)
        {
            if (oldScore != GManager.instance.score)
            {
                scoreText.text = "Score " + GManager.instance.score.ToString();
                oldScore = GManager.instance.score;
            }
        }
    }
}