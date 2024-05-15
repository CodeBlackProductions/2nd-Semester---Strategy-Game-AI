using UnityEngine;

public class StageSelectMenuController : MonoBehaviour
{
    #region Fields

    [Header("Stage Select Menu")]
    [SerializeField] private GameObject topButtonStageSelectMenu;

    private GameObject eventSystem;
    private GameObject musicSource;

    #endregion Fields

    #region Unity Functions

    public void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        musicSource = GameObject.Find("MusicSource");
    }

    public void Start()
    {
        SelectFirstButton(topButtonStageSelectMenu);
    }

    #endregion Unity Functions

    #region Button Functions

    #region Stage Select Menu

    /// <summary>
    /// starts a new game and switches to game scene.
    /// </summary>
    public void OnForest()
    {
        PlayClick();
        SceneLoadManager.sceneLoadManager.LoadSceneSingle((int)Scenes.ForestScene);
    }

    /// <summary>
    /// starts a new game and switches to game scene.
    /// </summary>
    public void OnDesert()
    {
        PlayClick();
        SceneLoadManager.sceneLoadManager.LoadSceneSingle((int)Scenes.DesertScene);
    }

    /// <summary>
    /// Switches back o the main menu from any sub menu.
    /// </summary>
    public void OnBack()
    {
        PlayClick();
        SceneLoadManager.sceneLoadManager.UnloadSceneAsync((int)Scenes.StageSelect);
    }

    #endregion Stage Select Menu

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