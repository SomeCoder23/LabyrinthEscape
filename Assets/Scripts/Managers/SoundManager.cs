using UnityEngine;
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager instance;
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Audio");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        if (instance != null)
        {
            Debug.LogWarning("More than one Sound Manager!");
            return;
        }

        instance = this;
    }

    #endregion

    public AudioSource musicAudio, soundAudio;
    public AudioClip mainMenuMusic, gameMusic;
    public AudioClip buttonClick;
    public AudioClip loseClip, winClip;

    bool sound = true, music = true, paused = false;
    State currentState = State.MainMenu;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public void ClickBtn()
    {
        if (sound)
            soundAudio.PlayOneShot(buttonClick);
    }

    public void PlaySound(AudioClip clip)
    {
        if (sound)
            soundAudio.PlayOneShot(clip);
    }
    public void StopSound()
    {
        if(soundAudio.isPlaying)
            soundAudio.Stop();
    }
    public void StopMusic()
    {
        musicAudio.Stop();
    }

    public void PlayMusic(State gameState)
    {
        if (gameState == currentState)
            return;

        if (gameState == State.Game)
        {
            musicAudio.volume -= 0.5f;
            musicAudio.clip = gameMusic;
        }
        else { 
            if(musicAudio.volume > 0.1f)
                musicAudio.volume += 0.5f;
            musicAudio.clip = mainMenuMusic; 
        }

        currentState = gameState;
        musicAudio.Play();
    }

    public bool isSoundOn()
    {
        if (!paused)
            return sound;
        else return false;
    }

    public bool isMusicOn()
    {
        if (!paused)
            return music;
        else return false;
    }

    public void Lose()
    {
        PlaySound(loseClip);
    }

    public void NewLevel()
    {
        PlaySound(winClip);
    }

    public void PlayRandom(AudioClip[] clips)
    {
        int index = Random.Range(0, clips.Length);
        PlaySound(clips[index]);
    }

    public void setMusicVolume(float volume)
    {
        musicAudio.volume = volume;
    }

    public float getMusicVolume()
    {
        return musicAudio.volume;
    }
}

public enum State
{
    MainMenu,
    Game
}