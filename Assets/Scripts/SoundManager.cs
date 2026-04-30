using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip buttonClip;
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip hintClip;
    public AudioClip endgameClip;
    public AudioClip victoryClip;
    public bool isMuted = false;

    private Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            clips["button"] = buttonClip;
            clips["correct"] = correctClip;
            clips["wrong"] = wrongClip;
            clips["hint"] = hintClip;
            clips["endgame"] = endgameClip;
            clips["victory"] = victoryClip;

            // SaveData'yý yükle
            SaveManager.Load();

            // Ses ayarýný uygula
            SetMute(SaveManager.data.soundMuted);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(string name)
    {
        if (isMuted) return;

        if (clips.ContainsKey(name))
            sfxSource.PlayOneShot(clips[name]);
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (isMuted) return;

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void SetMute(bool mute)
    {
        isMuted = mute;

        sfxSource.mute = mute;
        if (musicSource != null)
            musicSource.mute = mute;

        // SaveData'ya yaz
        SaveManager.data.soundMuted = mute;
        SaveManager.Save();
    }
}
