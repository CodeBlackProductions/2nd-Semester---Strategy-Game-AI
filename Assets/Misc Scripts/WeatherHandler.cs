using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeatherHandler : MonoBehaviour
{
    [SerializeField] int levelIndex = 0;
    [SerializeField] int weatherSwitchChance;
    [SerializeField] GameObject rainPrefab;

    public int LevelIndex { get => levelIndex; }

    public static WeatherHandler weatherHandler;
    private int weatherIndex = 0;
    private GameObject rainOject;
    private SoundManager soundManager;
    private float timer = 30;

    private void Awake()
    {
        if (weatherHandler == null)
        {
            weatherHandler = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        soundManager = SoundManager.soundManager;
    }

    private void Start()
    {
        if (levelIndex == 0)
        {
            rainOject = GameObject.Instantiate<GameObject>(rainPrefab);
            rainOject.transform.position = Camera.main.transform.position;
            rainOject.transform.SetParent(Camera.main.transform);
            rainOject.GetComponent<VisualEffect>().Stop();
            soundManager.ChangeAmbience(Ambience.Forest);
            soundManager.SwitchAmbienceState(true);
        }
        else if (levelIndex == 1)
        {
           soundManager.ChangeAmbience(Ambience.Desert);
           soundManager.SwitchAmbienceState(true);
        }
    }

    private void Update()
    {
        if (timer <= 0)
        {
            int randNr = Random.Range(0, 100);
            if (randNr <= weatherSwitchChance)
            {
                SwitchWeather();
                timer = 30;
            }
            else
            {
                timer = 5;
            } 
        }
        else
        {
            timer -= Time.deltaTime;
        }
    
    }

    private void SwitchWeather()
    {
        if (levelIndex == 0)
        {
            if (weatherIndex == 0)
            {
                rainOject.GetComponent<VisualEffect>().Play();
                soundManager.ChangeAmbience(Ambience.Rain);
                soundManager.SwitchAmbienceState(true);
                weatherIndex = 1;
            }
            else if (weatherIndex == 1)
            {
                rainOject.GetComponent<VisualEffect>().Stop();
                soundManager.ChangeAmbience(Ambience.Forest);
                soundManager.SwitchAmbienceState(true);
                weatherIndex = 0;
            }
          
        }
        else if (levelIndex == 1)
        {
            if (weatherIndex == 0)
            {
                soundManager.ChangeAmbience(Ambience.Strong_Wind);
                soundManager.SwitchAmbienceState(true);
                weatherIndex = 1;
            }
            else if (weatherIndex == 1)
            {
                soundManager.ChangeAmbience(Ambience.Desert);
                soundManager.SwitchAmbienceState(true);
                weatherIndex = 0;
            }
            
        }

        EventHandler.eventHandler.RTSWeatherChange.Invoke(weatherIndex);
    }
}
