using System;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static EventHandler eventHandler;

    public Action FinishLoading = () => { };
    public Action<bool> CPSwitchControls = (_) => { };
    public Action RTSStartBattle = () => { };
    public Action RTSFinishDeployment = () => { };
    public Action<FormationSlotController> RTSSlotClicked = (_)=> { };
    public Action<UnitController> RTSUnitClicked = (_) => { };
    public Action<bool> RTSSwitchControls = (_) => { };
    public Action<UnitController, bool> RTSUnitInCombat = (_,_) => { };
    public Action<int> RTSWeatherChange;
    public Action OnSelection = () => { };
    public Action<float,float,int> OnFloatValueChange = (_,_,_) => { };

    public enum Value { currentCommandPoints}

    private void Awake()
    {
        if (eventHandler == null)
        {
            eventHandler = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}