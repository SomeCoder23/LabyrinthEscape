using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text highScore;
    public GameObject[] helpPages;
    public GameObject nextBtn, backBtn;

    int currentPage = 0;

    private void Start()
    {
        SoundManager.instance.PlayMusic(State.MainMenu);
        if (PlayerPrefs.HasKey("HighScore"))
            highScore.text = "High Score: "  + PlayerPrefs.GetInt("HighScore");
        else highScore.text = "High Score: 0000";
    }

    public void NextPage()
    {

        helpPages[currentPage].SetActive(false);
        currentPage++;
        helpPages[currentPage].SetActive(true);
        if (!backBtn.activeSelf)
            backBtn.SetActive(true);

        if (currentPage >= helpPages.Length - 1)
            nextBtn.SetActive(false);
    }

    public void PreviousPage()
    {
        helpPages[currentPage].SetActive(false);
        currentPage--;
        helpPages[currentPage].SetActive(true);
        if (!nextBtn.activeSelf)
            nextBtn.SetActive(true);

        if (currentPage == 0)
            backBtn.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
