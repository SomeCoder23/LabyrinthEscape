using UnityEngine;
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager instance;
    private void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("More than one Sound Manager!");
            return;
        }

        instance = this;
    }

    #endregion

    public AudioSource musicAudio, soundAudio;
    public AudioClip buttonClick;
    public AudioClip loseClip, winClip;

    bool sound = true, music = true, paused = false;

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

    public void ChangeMusic(AudioClip clip)
    {
        musicAudio.clip = clip;
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
}