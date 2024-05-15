using UnityEngine;

public class CreditsMenuController : MonoBehaviour
{
    #region Fields

    [Space]
    [Header("Credits Menu")]

    [SerializeField] private GameObject topButtonCreditsMenu;

    private GameObject eventSystem;
    private GameObject soundSource;

    #endregion Fields

    #region Unity Functions

    public void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        soundSource = GameObject.Find("SoundSource");
    }

    public void Start()
    {
        SelectFirstButton(topButtonCreditsMenu);
    }

    #endregion Unity Functions

    #region Button Functions

    #region Credits Menu

    /// <summary>
    /// Switches back o the main menu from any sub menu.
    /// </summary>
    public void OnBack()
    {
        PlayClick();
        SceneLoadManager.sceneLoadManager.UnloadSceneAsync((int)Scenes.CreditsMenu);
    }

    #endregion Credits Menu

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