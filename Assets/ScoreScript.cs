using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    private int score = 0;
    public TextMeshProUGUI text;
    public Text highScoreText;
    public int scorePerSecond;
    bool isActivated = false;

    private void Start()
    {
        StartCoroutine(EverySecondScore());
        highScoreText.text = "HighScore: " + PlayerPrefs.GetInt("HighScore").ToString();

    }

    public void Activate()
    {
        isActivated = true;
    }

    public void Deactivate()
    {
        isActivated = false;
        int high = PlayerPrefs.GetInt("HighScore");
        if (score > high)
            PlayerPrefs.SetInt("HighScore", score);
    }

    private IEnumerator EverySecondScore()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            if (isActivated)
            {
                score += scorePerSecond;
                text.text = score.ToString("D6");
            }

            if (Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.E))
            {
                PlayerPrefs.SetInt("HighScore", 0);
            }
        }
    }
    public void AddScore(int much)
    {
        if (!isActivated) return;

        score += much;
        text.text = score.ToString("D6");
    }
}
