using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scenes
{InitScene, MainMenu,StageSelect , OptionsMenu, CreditsMenu, IngameMenu, CampaignMap, ForestScene, DesertScene }

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject done;

    public static SceneLoadManager sceneLoadManager;

    private bool finished = false;

    private WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

    private void Awake()
    {
        if (sceneLoadManager == null)
        {
            sceneLoadManager = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        LoadSceneSingle((int)Scenes.MainMenu);
    }

    public void LoadSceneSingle(int index)
    {
        StartCoroutine(LoadingScreen(1f, SceneManager.LoadSceneAsync(index, LoadSceneMode.Single), false, null));
    }

    public void LoadSceneAdditive(int index)
    {
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
    }

    public void UnloadSceneAsync(int index)
    {
        SceneManager.UnloadSceneAsync(index);
    }

    public IEnumerator LoadingScreen(float forcedTime, AsyncOperation async, bool isSaveState, string levelID)
    {
        finished = false;
        float timer = 0;
        loading.SetActive(true);
        loadingPanel.SetActive(true);
        do
        {
            float asyncProgress = Mathf.InverseLerp(0.0f, 0.9f, async.progress);
            float forcedTimeProgress = Mathf.InverseLerp(0.0f, forcedTime, timer);

            progressBar.fillAmount = asyncProgress < forcedTimeProgress ? asyncProgress : forcedTimeProgress;

            yield return waitFrame;
            timer += Time.deltaTime;
        } while (progressBar.fillAmount < 0.99f);
        loading.SetActive(false);
        done.SetActive(true);

        while (!finished)
        {
            yield return waitFrame;
        }
        done.SetActive(false);
        loadingPanel.SetActive(false);
        EventHandler.eventHandler.FinishLoading.Invoke();
    }

    public void OnContinue()
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        finished = true;
    }
}