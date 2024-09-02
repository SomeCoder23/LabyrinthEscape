using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    #region Singleton
    public static GameUIManager instance;

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
    public Slider volumeSlider;

    int score, level;
    int levelWinPoints = 25;
    int highScore;
    bool doneGenerating = false, doneWaiting = false;

    void Start()
    {
        SoundManager.instance.PlayMusic(State.Game);
        volumeSlider.value = SoundManager.instance.getMusicVolume();
        score = 0;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
    }

    public void ActivateLoadScreen()
    {
        blackImage.SetActive(true);
        Invoke("EndWaiting", fadeDuration);
    }

    public void AddPoint(int points = 1)
    {
        score += points;
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
        SceneSwitcher sceneManager = GetComponent<SceneSwitcher>();
        if (sceneManager != null)
            sceneManager.ReloadScene();
    }

    public void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }

    public int getLevel()
    {
        return level;
    }

    public void ChangeVolume(float value)
    {
        SoundManager.instance.setMusicVolume(value);
    }
}
