using System;
using UnityEngine;

public enum SFX
{ Click, Error, Attack1, Attack2, Attack3, WarHorn1, WarHorn2, TimeToPrepare, StandStrong }

public enum Music
{ Base }

public enum Ambience
{ Forest, Desert, Rain, Rain_Lake, Strong_Wind, BattleSounds, ArmyMarch }

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;

    [SerializeField] private GameObject soundSourceObject;
    [SerializeField] private GameObject musicSourceObject;
    [SerializeField] private GameObject ambienceSourceObject;
    [SerializeField] private ClipStruct[] sfxClips;
    [SerializeField] private ClipStruct[] musicClips;
    [SerializeField] private ClipStruct[] ambienceClips;

    private AudioSource soundSource;
    private AudioSource musicSource;
    private AudioSource ambienceSource;

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        soundSource = soundSourceObject.GetComponent<AudioSource>();
        musicSource = musicSourceObject.GetComponent<AudioSource>();
        ambienceSource = ambienceSourceObject.GetComponent<AudioSource>();
    }

    public void PlayOneShot(SFX sfx)
    {
        soundSource.PlayOneShot(sfxClips[(int)sfx].Clip);
    }

    public void PlayOneShotLocal(SFX sfx, AudioSource soundSource)
    {
        soundSource.PlayOneShot(sfxClips[(int)sfx].Clip);
    }

    public void ChangeMusic(Music music)
    {
        musicSource.clip = musicClips[(int)music].Clip;
    }

    public void SwitchMusicState(bool shouldPlay)
    {
        if (shouldPlay && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
        else if (!shouldPlay && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void ChangeAmbience(Ambience ambience)
    {
        ambienceSource.clip = ambienceClips[(int)ambience].Clip;
    }

    public void SwitchAmbienceState(bool shouldPlay)
    {
        if (shouldPlay && !ambienceSource.isPlaying)
        {
            ambienceSource.Play();
        }
        else if (!shouldPlay && ambienceSource.isPlaying)
        {
            ambienceSource.Stop();
        }
    }

    public void ChangeLocalAmbience(Ambience ambience, AudioSource soundSource)
    {
        if (soundSource.clip != ambienceClips[(int)ambience].Clip)
        {
            soundSource.clip = ambienceClips[(int)ambience].Clip;
        }
    }

    public void SwitchLocalAmbienceState(bool shouldPlay, AudioSource soundSource)
    {
        if (shouldPlay && !soundSource.isPlaying)
        {
            soundSource.Play();
        }
        else if (!shouldPlay && soundSource.isPlaying)
        {
            soundSource.Stop();
        }
    }
}

[Serializable]
internal struct ClipStruct
{
    public string Key;
    public AudioClip Clip;
}