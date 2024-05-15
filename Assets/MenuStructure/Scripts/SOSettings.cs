using UnityEngine;

[CreateAssetMenu(fileName = "NewSettingsSO", menuName = "ScriptableObjects/SOSettings")]
public class SOSettings : ScriptableObject
{
    [SerializeField] private float masterVolume = 0;
    [SerializeField] private float musicVolume = 0;
    [SerializeField] private float ambienceVolume = 0;
    [SerializeField] private float sfxVolume = 0;
    [SerializeField] private bool isFullscreen = true;
    [SerializeField] private int resolution = 0;

    public float MasterVolume
    { get => masterVolume; set { masterVolume = value; } }

    public float MusicVolume
    { get => musicVolume; set { musicVolume = value; } }

    public float AmbienceVolume 
    { get => ambienceVolume; set => ambienceVolume = value; }

    public float SFXVolume
    { get => sfxVolume; set { sfxVolume = value; } }

    public bool IsFullscreen
    { get => isFullscreen; set { isFullscreen = value; } }

    public int Resolution
    { get => resolution; set { resolution = value; } }

}