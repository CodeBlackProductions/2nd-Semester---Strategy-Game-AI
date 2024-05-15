using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RTS_SelectionHandler : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image dragSelection;

    private List<RTS_UnitController> selectedUnits = new List<RTS_UnitController>();
    private Vector3 centerPos;
    private float distance;

    private RTS_PlayerController playerController;

    #endregion Fields

    #region Internal Methods

    private RaycastHit GetMouseTarget(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Physics.Raycast(ray, out RaycastHit hitInfo);

        return hitInfo;
    }

    private void FindCenter(Vector3 startPos, Vector3 endPos)
    {
        centerPos = Vector3.Lerp(endPos, startPos, 0.5f);
    }

    private void GetDistance(Vector3 startPos, Vector3 endPos)
    {
        distance = Vector3.Distance(endPos, startPos);
    }

    #endregion Internal Methods

    public void Start()
    {
        EventHandler.eventHandler.RTSUnitClicked += SelectSingleUnit;
        playerController = GetComponent<RTS_PlayerController>();
    }

    private void OnDisable()
    {
        EventHandler.eventHandler.RTSUnitClicked -= SelectSingleUnit;
    }

    public dynamic TargetCheck(Vector2 mousePosition)
    {
        RaycastHit mouseTarget = GetMouseTarget(mousePosition);

        if (mouseTarget.collider != null)
        {
            if (mouseTarget.collider.gameObject.TryGetComponent<UnitController>(out UnitController unit))
            {
                return unit;
            }
            else if (mouseTarget.collider.gameObject.TryGetComponent<FormationSlotController>(out FormationSlotController formation))
            {
                return formation;
            }
            else
            {
                return mouseTarget.point;
            }
        }
        else
        {
            return null;
        }
    }

    public void DrawSelection(Vector2 startPos, Vector2 endPos)
    {
        Vector3 pos1 = GetMouseTarget(startPos).point;
        Vector3 pos2 = GetMouseTarget(endPos).point;
        FindCenter(pos1, pos2);
        GetDistance(pos1, pos2);
        dragSelection.transform.position = centerPos;
        dragSelection.transform.position += new Vector3(0, 0.1f, 0);
        dragSelection.rectTransform.sizeDelta = new Vector2(distance, distance);
        dragSelection.enabled = true;
    }

    public List<RTS_UnitController> SelectUnitsInSelection()
    {
        EventHandler.eventHandler.RTSSlotClicked?.Invoke(null);
        DeselectAllUnits();

        Collider[] tempObjects = Physics.OverlapBox(centerPos, new Vector3(distance * 0.5f, distance * 0.5f, distance * 0.5f));

        for (int i = 0; i < tempObjects.Length; i++)
        {
            if (tempObjects[i].TryGetComponent<RTS_UnitController>(out RTS_UnitController unit))
            {
                if (unit.aiUnit)
                {
                    break;
                }
                unit.Selected = true;
                selectedUnits.Add(unit);
            }
        }

        dragSelection.enabled = false;

        if (selectedUnits.Count > 0)
        {
            Time.timeScale = 0;
            return selectedUnits;
        }
        else
        {
            return null;
        }
    }

    public void SelectSingleUnit(UnitController controller)
    {
        if (controller != null)
        {
            DeselectAllUnits();
            EventHandler.eventHandler.RTSSlotClicked?.Invoke(null);
            RTS_UnitController rtsController = controller.ParentFormation.ParentGroup.GetComponent<RTS_UnitController>();
            rtsController.Selected = true;
            selectedUnits.Add(rtsController);
            playerController.SelectedUnits = selectedUnits;
            Time.timeScale = 0;
        }
        else
        {
            DeselectAllUnits();
        }
    }

    public void DeselectAllUnits()
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            selectedUnits[i].Selected = false;
        }

        selectedUnits.Clear();
        Time.timeScale = 1;
    }
}