using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    #region Fields

    [Header("Options Menu")]
    [SerializeField] private GameObject topButtonOptions;

    [SerializeField] private GameObject masterVolumeSlider;
    [SerializeField] private GameObject musicVolumeSlider;
    [SerializeField] private GameObject ambienceVolumeSlider;
    [SerializeField] private GameObject sfxVolumeSlider;
    [SerializeField] private GameObject fullScreenToggle;
    [SerializeField] private GameObject resolutionDropDown;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private SOSettings settingsSO;


    private GameObject eventSystem;
    private float masterVolumeValue;
    private float musicVolumeValue;
    private float ambienceVolumeValue;
    private float sfxVolumeValue;
    private bool isFullscreen;
    private int resolution;
    private int[] resolutionHeight = new int[6] { 1080, 768, 864, 720, 900, 900 };
    private int[] resolutionWidth = new int[6] { 1920, 1366, 1536, 1280, 1440, 1600 };

    #endregion Fields

    #region Unity Functions

    public void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        audioMixer.SetFloat("MasterVolume", 0);
    }

    public void Start()
    {
        //TODO: load from options save data or standard settings data
        Init();
    }

    #endregion Unity Functions

    #region Button Functions

    #region Options Menu

    public void OnMasterVolumeChange(float input)
    {
        PlayClick();
        masterVolumeValue = input;
        if (input == -10)
        {
            audioMixer.SetFloat("MasterVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", masterVolumeValue);
        }
        masterVolumeSlider.GetComponent<Slider>().value = input;
        string tempString = "";
        tempString = masterVolumeValue.ToString();
        masterVolumeSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = tempString;
    }

    public void OnMusicVolumeChange(float input)
    {
        PlayClick();
        musicVolumeValue = input;
        if (input == -10)
        {
            audioMixer.SetFloat("MusicVolume", -80);
        }
        else
        {
        audioMixer.SetFloat("MusicVolume", musicVolumeValue);
        }
        musicVolumeSlider.GetComponent<Slider>().value = input;
        string tempString = "";
        tempString = musicVolumeValue.ToString();
        musicVolumeSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = tempString;
    }

    public void OnAmbienceVolumeChange(float input)
    {
        PlayClick();
        ambienceVolumeValue = input;
        if (input == -10)
        {
            audioMixer.SetFloat("AmbienceVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("AmbienceVolume", ambienceVolumeValue);
        }
        ambienceVolumeSlider.GetComponent<Slider>().value = input;
        string tempString = "";
        tempString = ambienceVolumeValue.ToString();
        ambienceVolumeSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = tempString;
    }

    public void OnSFXVolumeChange(float input)
    {
        PlayClick();
        sfxVolumeValue = input;
        if (input == -10)
        {
            audioMixer.SetFloat("SFXVolume", -80);
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", sfxVolumeValue);
        }
        sfxVolumeSlider.GetComponent<Slider>().value = input;
        string tempString = "";
        tempString = sfxVolumeValue.ToString();
        sfxVolumeSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = tempString;
    }

    public void OnFullScreenChange(bool input)
    {
        PlayClick();
        isFullscreen = input;
        Screen.fullScreen = input;
        fullScreenToggle.GetComponent<Toggle>().isOn = input;
    }

    public void OnResolutionChange(int input)
    {
        PlayClick();
        resolution = input;
        Screen.SetResolution(resolutionHeight[settingsSO.Resolution], resolutionWidth[settingsSO.Resolution], isFullscreen);
        resolutionDropDown.GetComponent<TMP_Dropdown>().value = settingsSO.Resolution;
    }

    public void OnSaveSettings()
    {
        PlayClick();
        settingsSO.MasterVolume = masterVolumeValue;
        settingsSO.MusicVolume = musicVolumeValue;
        settingsSO.SFXVolume = sfxVolumeValue;
        settingsSO.IsFullscreen = isFullscreen;
        settingsSO.Resolution = resolution;
    }

    public void OnResetSettings()
    {
        PlayClick();
        Init();
    }

    /// <summary>
    /// Switches back o the main menu from any sub menu.
    /// </summary>
    public void OnBack()
    {
        PlayClick();
        Init();
        SceneLoadManager.sceneLoadManager.UnloadSceneAsync((int)Scenes.OptionsMenu);
    }

    #endregion Options Menu

    public void SelectFirstButton(GameObject buttonToSelect)
    {
        eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(buttonToSelect);
    }

    public void PlayClick()
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
    }

    public void Init()
    {
        OnMasterVolumeChange(settingsSO.MasterVolume);
        OnMusicVolumeChange(settingsSO.MusicVolume);
        OnSFXVolumeChange(settingsSO.SFXVolume);
        OnFullScreenChange(settingsSO.IsFullscreen);
        OnResolutionChange(settingsSO.Resolution);
        SelectFirstButton(topButtonOptions);
    }

    #endregion Button Functions
}