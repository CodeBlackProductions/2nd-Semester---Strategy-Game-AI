using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FormationController : MonoBehaviour
{
    #region Fields

    public FormationDataStruct FormationData { get; set; }

    private Dictionary<int, List<Vector3>> formations = new Dictionary<int, List<Vector3>>();
    private List<Vector3> unitPositions = new List<Vector3>();
    private List<GameObject> units = new List<GameObject>();
    private List<BT_Unit> unitAIs = new List<BT_Unit>();

    private float xOffset;
    private float zOffset;
    private Vector3 formationRotation;
    private Vector3 oldFormationRotation;
    private GameObject unitContainer;

    private UnitGroupController parentGroup;

    public UnitGroupController ParentGroup { get => parentGroup; }
    public List<BT_Unit> UnitAIs { get => unitAIs;}

    #endregion Fields

    public void Initiate()
    {
        FormationSetup();

        SpawnFormation();
    }

    private void Update()
    {
        formationRotation = transform.forward;

        if (unitPositions[0] != transform.position || formationRotation != oldFormationRotation)
        {
            UpdateUnitPositions();
            oldFormationRotation = formationRotation;
        }

        CheckForDeadUnits();
    }

    private void FormationSetup()
    {
        unitContainer = GameObject.Find("UnitContainer");
        if (unitContainer == null)
        {
            unitContainer = new GameObject("UnitContainer");
        }

        parentGroup = transform.parent.GetComponent<UnitGroupController>();
        xOffset = 1.2f;
        zOffset = 1.3f;
        formationRotation = transform.forward;
        oldFormationRotation = formationRotation;

        #region Square Formation

        if (!formations.ContainsKey(0))
        {
            formations.Add(0, new List<Vector3>());

            formations[0].Add(new Vector3(0, 0, 0));
            formations[0].Add(new Vector3(xOffset, 0, 0));
            formations[0].Add(new Vector3(xOffset, 0, zOffset));
            formations[0].Add(new Vector3(-xOffset, 0, 0));
            formations[0].Add(new Vector3(-xOffset, 0, -zOffset));
            formations[0].Add(new Vector3(0, 0, zOffset));
            formations[0].Add(new Vector3(-xOffset, 0, zOffset));
            formations[0].Add(new Vector3(0, 0, -zOffset));
            formations[0].Add(new Vector3(xOffset, 0, -zOffset));
        }
        else
        {
            formations.Clear();
            formations.Add(0, new List<Vector3>());

            formations[0].Add(new Vector3(0, 0, 0));
            formations[0].Add(new Vector3(xOffset, 0, 0));
            formations[0].Add(new Vector3(xOffset, 0, zOffset));
            formations[0].Add(new Vector3(-xOffset, 0, 0));
            formations[0].Add(new Vector3(-xOffset, 0, -zOffset));
            formations[0].Add(new Vector3(0, 0, zOffset));
            formations[0].Add(new Vector3(-xOffset, 0, zOffset));
            formations[0].Add(new Vector3(0, 0, -zOffset));
            formations[0].Add(new Vector3(xOffset, 0, -zOffset));
        }

        #endregion Square Formation

        #region Triangle Formation

        if (!formations.ContainsKey(1))
        {
            formations.Add(1, new List<Vector3>());

            formations[1].Add(new Vector3(0, 0, 0));
            formations[1].Add(new Vector3(xOffset, 0, 0));
            formations[1].Add(new Vector3(-xOffset, 0, 0));
            formations[1].Add(new Vector3(0, 0, zOffset));
            formations[1].Add(new Vector3(0, 0, -zOffset));
            formations[1].Add(new Vector3(xOffset, 0, -zOffset));
            formations[1].Add(new Vector3(-xOffset, 0, -zOffset));
            formations[1].Add(new Vector3(xOffset * 2, 0, -zOffset));
            formations[1].Add(new Vector3(-xOffset * 2, 0, -zOffset));
        }
        else
        {
            formations.Clear();
            formations.Add(1, new List<Vector3>());

            formations[1].Add(new Vector3(0, 0, 0));
            formations[1].Add(new Vector3(xOffset, 0, 0));
            formations[1].Add(new Vector3(-xOffset, 0, 0));
            formations[1].Add(new Vector3(0, 0, zOffset));
            formations[1].Add(new Vector3(0, 0, -zOffset));
            formations[1].Add(new Vector3(xOffset, 0, -zOffset));
            formations[1].Add(new Vector3(-xOffset, 0, -zOffset));
            formations[1].Add(new Vector3(xOffset * 2, 0, -zOffset));
            formations[1].Add(new Vector3(-xOffset * 2, 0, -zOffset));
        }

        #endregion Triangle Formation
    }

    private void SpawnFormation()
    {
        //Spawn Units in selected Formation
        for (int i = 0; i < formations[(int)FormationData.formation].Count; i++)
        {
            unitPositions.Add(transform.position + formations[(int)FormationData.formation][i].x * transform.right + formations[(int)FormationData.formation][i].z * transform.forward);
            units.Add(Instantiate<GameObject>(FormationData.unitPrefab, unitPositions[i], Quaternion.identity, unitContainer.transform));
            units[i].gameObject.layer = LayerMask.NameToLayer("IgnoreDecals");
            units[i].transform.LookAt(units[i].transform.position + transform.forward * 2);
            UnitController unitController = units[i].GetComponent<UnitController>();
            unitController.ParentFormation = this;
            BT_Unit unitAi = units[i].GetComponent<BT_Unit>();
            unitAIs.Add(unitAi);
            unitAi.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.MoveToTarget, unitPositions[i]);
            unitAi.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.UnitCohesion, parentGroup.Cohesion * parentGroup.Cohesion);
            unitAi.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.Team, parentGroup.Team);
            unitAi.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.Stance, ParentGroup.Stance);
            unitAi.Blackboard.Variables.Add((int)BT_Unit.UnitBlackBoardKey.EnemiesInSight, unitController.EnemyCount);
            unitAi.Initialize();
        }

        //Setup stats based on stance and formation;
        for (int i = 0; i < units.Count; i++)
        {
            UnitController temp = units[i].GetComponent<UnitController>();
            float armorBonus = 0;
            float damageBonus = 0;

            //NavMeshAgent priority
            units[i].GetComponent<NavMeshAgent>().avoidancePriority += parentGroup.Stance;

            //Stance bonus
            armorBonus += temp.Armor * (0.5f * (2 - parentGroup.Stance));
            damageBonus += temp.AttackDamage * (0.25f * parentGroup.Stance);

            //Formation bonus
            switch (FormationData.formation)
            {
                case FormationDataStruct.FormationTypes.Square:
                    armorBonus += temp.Armor * 0.5f;
                    break;

                case FormationDataStruct.FormationTypes.Triangle:
                    damageBonus += temp.AttackDamage * 0.25f;
                    break;
            }

            temp.Armor += armorBonus;
            temp.AttackDamage += damageBonus;
        }
    }

    private void UpdateUnitPositions()
    {
        for (int i = 0; i < formations[(int)FormationData.formation].Count; i++)
        {
            unitPositions[i] = transform.position + formations[(int)FormationData.formation][i].x * transform.right + formations[(int)FormationData.formation][i].z * transform.forward;
        }

        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] != null)
            {
                unitAIs[i].Blackboard.Variables[(int)BT_Unit.UnitBlackBoardKey.MoveToTarget] = unitPositions[i];
            }
        }
    }

    private void CheckForDeadUnits()
    {
        int deadUnits = 0;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].activeSelf == false)
            {
                deadUnits++;
            }
        }

        if (units.Count <= deadUnits)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Color tempcolor = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);
        Gizmos.color = tempcolor;
    }
}