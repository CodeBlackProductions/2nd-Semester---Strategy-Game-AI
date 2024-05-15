using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    #region Fields

    [Header("Main Menu")]
    [SerializeField] private GameObject topButtonMainMenu;

    [SerializeField] private AudioMixer AudioMixer;
    [SerializeField] private SOSettings SettingsSO;

    private GameObject eventSystem;
    private GameObject musicSource;
    private int[] resolutionHeight = new int[6] { 1080, 768, 864, 720, 900, 900 };
    private int[] resolutionWidth = new int[6] { 1920, 1366, 1536, 1280, 1440, 1600 };

    #endregion Fields

    #region Unity Functions

    public void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        musicSource = GameObject.Find("MusicSource");
    }

    public void Start()
    {
        Init();
    }

    #endregion Unity Functions

    #region Button Functions

    #region Main Menu

    /// <summary>
    /// Opens Stage Select.
    /// </summary>
    public void OnNewGame()
    {
        PlayClick();
        SceneLoadManager.sceneLoadManager.LoadSceneAdditive((int)Scenes.StageSelect);
    }

    /// <summary>
    /// Switches to the game options menu.
    /// </summary>
    public void OnOptions()
    {
        PlayClick();

        SceneLoadManager.sceneLoadManager.LoadSceneAdditive((int)Scenes.OptionsMenu);
    }

    /// <summary>
    /// Shows the Credits.
    /// </summary>
    public void OnCredits()
    {
        PlayClick();
        SceneLoadManager.sceneLoadManager.LoadSceneAdditive((int)Scenes.CreditsMenu);
    }

    public void OnQuitGame()
    {
        PlayClick();
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    #endregion Main Menu

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
        AudioMixer.SetFloat("MasterVolume", SettingsSO.MasterVolume);
        AudioMixer.SetFloat("MusicVolume", SettingsSO.MusicVolume);
        AudioMixer.SetFloat("SFXVolume", SettingsSO.SFXVolume);
        Screen.fullScreen = SettingsSO.IsFullscreen;
        Screen.SetResolution(resolutionWidth[SettingsSO.Resolution], resolutionHeight[SettingsSO.Resolution], SettingsSO.IsFullscreen);
        musicSource.GetComponent<AudioSource>().Play();
        SelectFirstButton(topButtonMainMenu);
    }

    #endregion Button Functions
}