using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    #region Singleton
    public static ScoreManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one ScoreManager!!");
            return;
        }
        instance = this;
    }

    #endregion

    public GameObject blackImage, loseWindow;
    public float fadeDuration = 2;
    public Text levelCounter, scoreTxt, loseBody;

    int score, level;
    int levelWinPoints = 25;
    int highScore;
    bool doneGenerating = false, doneWaiting = false;

    void Start()
    {
        score = 0;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
    }

    public void ActivateLoadScreen()
    {
        Debug.Log("FADING IN SCREEN");
        blackImage.SetActive(true);
        Invoke("EndWaiting", fadeDuration);
    }

    public void AddPoint()
    {
        score++;
        scoreTxt.text = "Score: " + score.ToString();
    }

    public void SaveScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", score);
            loseBody.text += "\n You beat the high score :)";
        }
    }

    public void StartLevel(int level)
    {
        this.level = level;
        score += level * levelWinPoints;
        scoreTxt.text = "Score: " + score.ToString();
        doneGenerating = true;
        if (doneGenerating && doneWaiting)
            FadeOut();
    }

    void EndWaiting()
    {
        doneWaiting = true;
        if (doneGenerating && doneWaiting)
            FadeOut();
    }

    void FadeOut()
    {
        doneWaiting = false;
        doneGenerating = false;
        Debug.Log("FADING OUT SCREEN");
        blackImage.SetActive(false);
        levelCounter.text = "Level " + level.ToString();
    }

    public void Lose()
    {
        loseBody.text = "Level Reached: " + level + "\nScore: " + score;
        SaveScore();
        loseWindow.SetActive(true);
    }

    public void Restart()
    {
        loseWindow.SetActive(false);
        level = 1;
        score = 0;
        //blackImage.SetActive(true);
        scoreTxt.text = "Score: 0";
        levelCounter.text = "Level 1";
        MazeGenerator.instance.ResetMaze();
    }

    //private IEnumerator FadeImage(float targetAlpha)
    //{
    //    float startAlpha = blackImage.color.a;
    //    float elapsedTime = 0f;

    //    while (elapsedTime < fadeDuration)
    //    {
    //        Debug.Log("FADING BLACK SCREEN");
    //        elapsedTime += 0.1f;
    //        float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
    //        blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, newAlpha);
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //    blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, targetAlpha);
    //}
}
