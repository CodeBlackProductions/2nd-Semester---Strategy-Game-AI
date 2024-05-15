using UnityEngine;

public class IngameMenuController : MonoBehaviour
{
    #region Fields

    private GameObject eventSystem;
    private GameObject soundSource;
    private RTS_PlayerController playerController;

    #endregion Fields

    #region Unity Functions

    public void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        soundSource = GameObject.Find("SoundSource");
    }

    public void Start()
    {
        playerController = FindObjectOfType<RTS_PlayerController>();
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
    }

    #endregion Unity Functions

    #region Button Functions

    #region Ingame Menu

    /// <summary>
    /// Switches to the game options menu.
    /// </summary>
    public void OnOptions()
    {
        PlayClick();

        SceneLoadManager.sceneLoadManager.LoadSceneAdditive((int)Scenes.OptionsMenu);
    }

    public void OnBackToMain()
    {
        PlayClick();
        Time.timeScale = 1;
        SoundManager.soundManager.SwitchAmbienceState(false);
        SceneLoadManager.sceneLoadManager.LoadSceneSingle((int)Scenes.MainMenu);
    }

    public void OnResume()
    {
        PlayClick();
        SceneLoadManager.sceneLoadManager.UnloadSceneAsync((int)Scenes.IngameMenu);
        EventHandler.eventHandler.RTSSwitchControls(true);
        Time.timeScale = 1;
    }

    #endregion Ingame Menu

    public void SelectFirstButton(GameObject buttonToSelect)
    {
        eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(buttonToSelect);
    }

    public void PlayClick()
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
    }

    #endregion Button Functions
}