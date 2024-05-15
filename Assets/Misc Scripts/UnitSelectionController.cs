using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionController : MonoBehaviour
{
    #region Fields

    [SerializeField] private UnitGroupController.GroupFormation groupFormation;
    [SerializeField] private UnitGroupController.GroupStance groupStance;

    [SerializeField] private GameObject formationSlotPrefab;
    [SerializeField] private float slotXSize = 3.5f;
    [SerializeField] private float slotZSize = 4;

    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject heavyUnitPrefab;
    [SerializeField] private GameObject TwoHandedUnitPrefab;
    [SerializeField] private GameObject RangeUnitPrefab;

    [SerializeField] private GameObject UnitGroupPrefab;

    private List<FormationDataStruct> formationsInGroup = new List<FormationDataStruct>();

    private Dictionary<int, List<Vector3>> groupFormations = new Dictionary<int, List<Vector3>>();
    private List<Vector3> formationPositions = new List<Vector3>();
    private List<GameObject> formations = new List<GameObject>();

    private GameObject unitContainer;

    private float cohesion;
    private int stance;

    private float xOffset;
    private float zOffset;

    private int team = 1;
    public int Team { get => team; }
    public float Cohesion { get => cohesion; }
    public int Stance { get => stance; set => stance = value; }

    public int Formation
    {
        get => (int)groupFormation;
        set
        {
            groupFormation = (UnitGroupController.GroupFormation)value;
            UpdateSlots();
        }
    }

    public List<GameObject> Formations { get => formations;}

    #endregion Fields

    private void Start()
    {
        EventHandler.eventHandler.RTSFinishDeployment += SpawnFormation;

        unitContainer = GameObject.Find("UnitContainer");
        if (unitContainer == null)
        {
            unitContainer = new GameObject("UnitContainer");
        }

        InitializeSlots();

        SlotSetup();

        SpawnSlots();
    }

    private void OnDestroy()
    {
        EventHandler.eventHandler.RTSFinishDeployment -= SpawnFormation;
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < 9; i++)
        {
            FormationDataStruct temp = new FormationDataStruct();
            temp.formation = FormationDataStruct.FormationTypes.Square;
            temp.formationRotationSpeed = 3;
            temp.unitPrefab = null;
            temp.unitSpacingFactor = 1;
            formationsInGroup.Add(temp);
        }
    }

    private void SlotSetup()
    {
        cohesion = ((int)groupStance + 1) * 5 + ((int)groupStance * 5);
        stance = (int)groupStance;

        groupFormations.Clear();

        #region Line Formation

        groupFormations.Add(0, new List<Vector3>());

        for (int i = 0; i < 9; i++)
        {
            groupFormations[0].Add(new Vector3(0, 0, 0));
        }

        if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = slotXSize + 2;
        }
        else if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[0].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = slotXSize;
        }
        for (int i = 1; i <= 7; i += 2)
        {
            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle)
            {
                xOffset += 2;
            }

            groupFormations[0][i] = new Vector3(xOffset, 0, 0);

            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle)
            {
                xOffset += slotXSize + 2;
            }
            else if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[i].formation == FormationDataStruct.FormationTypes.None)
            {
                xOffset += slotXSize;
            }
        }

        if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = slotXSize + 2;
        }
        else if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[0].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = slotXSize;
        }
        for (int i = 2; i <= 8; i += 2)
        {
            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle)
            {
                xOffset += 2;
            }

            groupFormations[0][i] = new Vector3(-xOffset, 0, 0);

            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle)
            {
                xOffset += slotXSize + 2;
            }
            else if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[i].formation == FormationDataStruct.FormationTypes.None)
            {
                xOffset += slotXSize;
            }
        }

        #endregion Line Formation

        #region Square Formation

        groupFormations.Add(1, new List<Vector3>());

        for (int i = 0; i < 9; i++)
        {
            groupFormations[1].Add(new Vector3(0, 0, 0));
        }

        if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = slotXSize + 2;
            zOffset = slotZSize;
        }
        else if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[0].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = slotXSize;
            zOffset = slotZSize;
        }

        if (formationsInGroup[1].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[1][1] = new Vector3(xOffset, 0, 0);
            xOffset -= 2;
        }
        else
        {
            groupFormations[1][1] = new Vector3(xOffset, 0, 0);
        }

        if (formationsInGroup[2].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[1][2] = new Vector3(-xOffset, 0, 0);
            xOffset -= 2;
        }
        else
        {
            groupFormations[1][2] = new Vector3(-xOffset, 0, 0);
        }

        groupFormations[1][3] = new Vector3(0, 0, zOffset);
        groupFormations[1][4] = new Vector3(0, 0, -zOffset);

        if (formationsInGroup[3].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = slotXSize + 2;
        }
        else if (formationsInGroup[3].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[3].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = slotXSize;
        }

        if (formationsInGroup[5].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[1][5] = new Vector3(xOffset, 0, zOffset);
            xOffset -= 2;
        }
        else
        {
            groupFormations[1][5] = new Vector3(xOffset, 0, zOffset);
        }

        if (formationsInGroup[6].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[1][6] = new Vector3(-xOffset, 0, zOffset);
            xOffset -= 2;
        }
        else
        {
            groupFormations[1][6] = new Vector3(-xOffset, 0, zOffset);
        }

        if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = slotXSize + 2;
        }
        else if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[4].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = slotXSize;
        }

        if (formationsInGroup[7].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[1][7] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            groupFormations[1][7] = new Vector3(xOffset, 0, -zOffset);
        }

        if (formationsInGroup[8].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[1][8] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            groupFormations[1][8] = new Vector3(-xOffset, 0, -zOffset);
        }

        #endregion Square Formation

        #region Triangle Formation

        groupFormations.Add(2, new List<Vector3>());

        for (int i = 0; i < 9; i++)
        {
            groupFormations[2].Add(new Vector3(0, 0, 0));
        }

        if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = slotXSize + 2;
            zOffset = slotZSize;
        }
        else if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[0].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = slotXSize;
            zOffset = slotZSize;
        }

        if (formationsInGroup[1].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][1] = new Vector3(xOffset, 0, 0);
            xOffset -= 2;
        }
        else
        {
            groupFormations[2][1] = new Vector3(xOffset, 0, 0);
        }

        if (formationsInGroup[2].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][2] = new Vector3(-xOffset, 0, 0);
            xOffset -= 2;
        }
        else
        {
            groupFormations[2][2] = new Vector3(-xOffset, 0, 0);
        }

        groupFormations[2][3] = new Vector3(0, 0, zOffset);
        groupFormations[2][4] = new Vector3(0, 0, -zOffset);

        if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = slotXSize + 2;
        }
        else if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[4].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = slotXSize;
        }

        if (formationsInGroup[5].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][5] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            groupFormations[2][5] = new Vector3(xOffset, 0, -zOffset);
        }

        if (formationsInGroup[6].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][6] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            groupFormations[2][6] = new Vector3(-xOffset, 0, -zOffset);
        }

        if (formationsInGroup[5].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += slotXSize + 4;
        }
        else if (formationsInGroup[5].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[5].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset += slotXSize;
        }

        if (formationsInGroup[7].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][7] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            groupFormations[2][7] = new Vector3(xOffset, 0, -zOffset);
        }

        if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = slotXSize + 2;
        }
        else if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[4].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = slotXSize;
        }
        if (formationsInGroup[6].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += slotXSize + 4;
        }
        else if (formationsInGroup[6].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[6].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset += slotXSize;
        }

        if (formationsInGroup[8].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][8] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            groupFormations[2][8] = new Vector3(-xOffset, 0, -zOffset);
        }

        #endregion Triangle Formation
    }

    private void SpawnSlots()
    {
        //Spawn Formations in selected Group
        for (int i = 0; i < formationsInGroup.Count; i++)
        {
            formationPositions.Add(transform.position + groupFormations[(int)groupFormation][i].x * transform.right + groupFormations[(int)groupFormation][i].z * transform.forward);
            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Square && formationsInGroup[i].unitPrefab == null)
            {
                formations.Add(GameObject.Instantiate(formationSlotPrefab, formationPositions[i], Quaternion.identity));
                formations[i].transform.SetParent(transform);
            }
            else if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle && formationsInGroup[i].unitPrefab == null)
            {
                formations.Add(GameObject.Instantiate(formationSlotPrefab, formationPositions[i], Quaternion.identity));
                formations[i].transform.SetParent(transform);
                formations[i].transform.localScale += new Vector3(0, 0, 2);
            }
            else
            {
                formations.Add(GameObject.Instantiate(formationSlotPrefab, formationPositions[i], Quaternion.identity));
                formations[i].transform.SetParent(transform);
            }

            formations[i].transform.GetComponent<FormationSlotController>().ParentController = this;
            formations[i].transform.localScale *= formationsInGroup[i].unitSpacingFactor;
        }
    }

    private void UpdateSlots()
    {
        SlotSetup();

        for (int i = 0; i < formationsInGroup.Count; i++)
        {
            formationPositions[i] = transform.position + groupFormations[(int)groupFormation][i].x * transform.right * formationsInGroup[i].unitSpacingFactor + groupFormations[(int)groupFormation][i].z * transform.forward * formationsInGroup[i].unitSpacingFactor;
            formations[i].transform.position = formationPositions[i];

            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[i].formation == FormationDataStruct.FormationTypes.None)
            {
                formations[i].transform.localScale = new Vector3(slotXSize * formationsInGroup[i].unitSpacingFactor, 2, slotZSize * formationsInGroup[i].unitSpacingFactor);
            }
            else
            {
                formations[i].transform.localScale = new Vector3(slotXSize * formationsInGroup[i].unitSpacingFactor, 2, (slotZSize + 2) * formationsInGroup[i].unitSpacingFactor);
            }
        }
    }

    public void SetSlotUnitType(FormationSlotController controller, UIHandler.UnitType type)
    {
        int index = formations.IndexOf(controller.gameObject);
        FormationDataStruct temp = formationsInGroup[index];
        switch (type)
        {
            case UIHandler.UnitType.Unit:
                temp.unitPrefab = unitPrefab;
                formationsInGroup[index] = temp;
                formations[index].GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;

            case UIHandler.UnitType.HeavyUnit:
                temp.unitPrefab = heavyUnitPrefab;
                formationsInGroup[index] = temp;
                formations[index].GetComponent<MeshRenderer>().material.color = Color.blue;
                break;

            case UIHandler.UnitType.TwoHandedUnit:
                temp.unitPrefab = TwoHandedUnitPrefab;
                formationsInGroup[index] = temp;
                formations[index].GetComponent<MeshRenderer>().material.color = Color.red;
                break;

            case UIHandler.UnitType.RangeUnit:
                temp.unitPrefab = RangeUnitPrefab;
                formationsInGroup[index] = temp;
                formations[index].GetComponent<MeshRenderer>().material.color = Color.green;
                break;
        }

        //TODO: Change Slot Visuals
    }

    public void SetSlotFormation(FormationSlotController controller, FormationDataStruct.FormationTypes type)
    {
        int index = formations.IndexOf(controller.gameObject);
        FormationDataStruct temp = formationsInGroup[index];
        temp.formation = type;
        formationsInGroup[index] = temp;
        UpdateSlots();
    }

    public void SetSlotSpacing(FormationSlotController controller, int factor)
    {
        int index = formations.IndexOf(controller.gameObject);
        FormationDataStruct temp = formationsInGroup[index];
        temp.unitSpacingFactor = factor;
        formationsInGroup[index] = temp;

        UpdateSlots();
    }

    public void SpawnFormation()
    {
        GameObject temp = GameObject.Instantiate(UnitGroupPrefab, transform.position, transform.rotation);
        UnitGroupController temp2 = temp.GetComponent<UnitGroupController>();
        for (int i = 0; i < formationsInGroup.Count; i++)
        {
            FormationDataStruct formation = formationsInGroup[i];
            temp2.formationsInGroup.Add(formation);
        }
        for (int i = 0; i < groupFormations.Count; i++)
        {
            temp2.GroupFormations.Add(i, groupFormations[i]);
        }
        temp2.Formation = (int)groupFormation;
        temp2.Stance = stance;
        temp2.Team = team;
        temp2.Initiate();

        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Color tempcolor = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1);
        Gizmos.color = tempcolor;
    }
}