using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("PlayerUI")]
    [SerializeField] private Canvas playerUICanvas;
    [SerializeField] private Image commandPointUI;
    [SerializeField] private GameObject finishDeploymentButton;
    [SerializeField] private GameObject startBattleButton;

    [Header("UnitSelectionUI")]
    [SerializeField] private GameObject unitSelectionCanvas;
    [SerializeField] private Canvas squareFormationCanvas;
    [SerializeField] private Image[] squareFormationButtons = new Image[9];
    [SerializeField] private Canvas triangleFormationCanvas;
    [SerializeField] private Image[] triangleFormationButtons = new Image[9];
    [SerializeField] private Canvas lineFormationCanvas;
    [SerializeField] private Image[] lineFormationButtons = new Image[9];

    private FormationSlotController slotController;
    private Image selectedButton;

    public enum UnitType
    { Unit, HeavyUnit, TwoHandedUnit, RangeUnit }

    public bool UnitSelectionActive { get; set; } = false;

    public static UIHandler uiHandler;

    public void Start()
    {
        if (uiHandler == null)
        {
            uiHandler = this;
        }
        else
        {
            Destroy(this);
        }

        EventHandler.eventHandler.RTSSlotClicked += SwitchSelectionMenu;
        EventHandler.eventHandler.OnFloatValueChange += OnValueChange;
        SoundManager.soundManager.PlayOneShot(SFX.TimeToPrepare);
    }

    public void OnDestroy()
    {
        EventHandler.eventHandler.RTSSlotClicked -= SwitchSelectionMenu;
        EventHandler.eventHandler.OnFloatValueChange -= OnValueChange;
    }

    private void OnValueChange(float value, float maxValue, int index)
    {
        switch (index)
        {
            case (int)EventHandler.Value.currentCommandPoints:
                commandPointUI.fillAmount = value / maxValue;
                break;
        }
    }

    public void SwitchSelectionMenu(FormationSlotController controller)
    {
        unitSelectionCanvas.SetActive(!unitSelectionCanvas.activeSelf && controller != null ? true : false);
        UnitSelectionActive = unitSelectionCanvas.activeSelf;
        playerUICanvas.enabled = !unitSelectionCanvas.activeSelf;
        slotController = unitSelectionCanvas.activeSelf ? controller : null;

        if (!unitSelectionCanvas.activeSelf)
        {
            EventHandler.eventHandler.RTSSwitchControls?.Invoke(true);
        }
        else
        {
            EventHandler.eventHandler.RTSSwitchControls?.Invoke(false);
            switch (controller.ParentController.Formation)
            {
                case (int)UnitGroupController.GroupFormation.Line:
                    squareFormationCanvas.enabled = false;
                    triangleFormationCanvas.enabled = false;
                    lineFormationCanvas.enabled = true;
                    break;

                case (int)UnitGroupController.GroupFormation.Square:
                    squareFormationCanvas.enabled = true;
                    triangleFormationCanvas.enabled = false;
                    lineFormationCanvas.enabled = false;
                    break;

                case (int)UnitGroupController.GroupFormation.Triangle:
                    squareFormationCanvas.enabled = false;
                    triangleFormationCanvas.enabled = true;
                    lineFormationCanvas.enabled = false;
                    break;
            }
            OnSlotSelection(controller.ParentController.Formations.IndexOf(controller.gameObject));
        }
    }

    public void OnUnitSelection(int type)
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        switch ((UnitType)type)
        {
            case UnitType.Unit:
                slotController.ParentController.SetSlotUnitType(slotController, UnitType.Unit);
                break;

            case UnitType.HeavyUnit:
                slotController.ParentController.SetSlotUnitType(slotController, UnitType.HeavyUnit);
                break;

            case UnitType.TwoHandedUnit:
                slotController.ParentController.SetSlotUnitType(slotController, UnitType.TwoHandedUnit);
                break;

            case UnitType.RangeUnit:
                slotController.ParentController.SetSlotUnitType(slotController, UnitType.RangeUnit);
                break;
        }
    }

    public void OnFormationSelection(int type)
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        switch ((FormationDataStruct.FormationTypes)type)
        {
            case FormationDataStruct.FormationTypes.Square:
                slotController.ParentController.SetSlotFormation(slotController, FormationDataStruct.FormationTypes.Square);
                break;

            case FormationDataStruct.FormationTypes.Triangle:
                slotController.ParentController.SetSlotFormation(slotController, FormationDataStruct.FormationTypes.Triangle);
                break;
        }
    }

    public void OnGroupFormationSelection(int type)
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        switch ((UnitGroupController.GroupFormation)type)
        {
            case UnitGroupController.GroupFormation.Square:
                slotController.ParentController.Formation = (int)UnitGroupController.GroupFormation.Square;
                squareFormationCanvas.enabled = true;
                triangleFormationCanvas.enabled = false;
                lineFormationCanvas.enabled = false;
                break;

            case UnitGroupController.GroupFormation.Triangle:
                slotController.ParentController.Formation = (int)UnitGroupController.GroupFormation.Triangle;
                squareFormationCanvas.enabled = false;
                triangleFormationCanvas.enabled = true;
                lineFormationCanvas.enabled = false;
                break;

            case UnitGroupController.GroupFormation.Line:
                slotController.ParentController.Formation = (int)UnitGroupController.GroupFormation.Line;
                squareFormationCanvas.enabled = false;
                triangleFormationCanvas.enabled = false;
                lineFormationCanvas.enabled = true;
                break;
        }
    }

    public void OnGroupStanceSelection(int type)
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        switch ((UnitGroupController.GroupStance)type)
        {
            case UnitGroupController.GroupStance.Aggressive:
                slotController.ParentController.Stance = (int)UnitGroupController.GroupStance.Aggressive;
                break;

            case UnitGroupController.GroupStance.Defensive:
                slotController.ParentController.Stance = (int)UnitGroupController.GroupStance.Defensive;
                break;

            case UnitGroupController.GroupStance.Stationary:
                slotController.ParentController.Stance = (int)UnitGroupController.GroupStance.Stationary;
                break;
        }
    }

    public void OnHideSelectionMenu()
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        unitSelectionCanvas.SetActive(false);
        playerUICanvas.enabled = true;
        EventHandler.eventHandler.RTSSwitchControls?.Invoke(true);
    }

    public void OnSlotSelection(int slot)
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        if (selectedButton != null)
        {
            selectedButton.color = Color.white;
        }

        switch (slotController.ParentController.Formation)
        {
            case (int)UnitGroupController.GroupFormation.Square:
                selectedButton = squareFormationButtons[slot];
                break;

            case (int)UnitGroupController.GroupFormation.Triangle:
                selectedButton = triangleFormationButtons[slot];
                break;

            case (int)UnitGroupController.GroupFormation.Line:
                selectedButton = lineFormationButtons[slot];
                break;
        }
        selectedButton.color = Color.cyan;

        FormationSlotController temp = slotController.ParentController.Formations[slot].GetComponent<FormationSlotController>();
        slotController = temp;
    }

    public void OnStartBattle()
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        EventHandler.eventHandler.RTSStartBattle.Invoke();
        startBattleButton.SetActive(false);
        SoundManager.soundManager.PlayOneShot(SFX.WarHorn1);
    }

    public void OnFinishDeployment()
    {
        SoundManager.soundManager.PlayOneShot(SFX.Click);
        EventHandler.eventHandler.RTSFinishDeployment.Invoke();
        finishDeploymentButton.SetActive(false);
        startBattleButton.SetActive(true);
        SoundManager.soundManager.PlayOneShot(SFX.StandStrong);
    }
}