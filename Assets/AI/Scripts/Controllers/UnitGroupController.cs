using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitGroupController : MonoBehaviour
{
    #region Fields

    public enum GroupFormation
    { Line, Square, Triangle/*, Custom*/ }

    public enum GroupStance
    { Stationary, Defensive, Aggressive }

    [SerializeField] private GroupFormation groupFormation;
    [SerializeField] public List<FormationDataStruct> formationsInGroup;
    [SerializeField] private GameObject formationPrefab;
    [SerializeField] [Range(1, 2)] private int team;
    [SerializeField] private GroupStance groupStance;
    [SerializeField] private AudioSource localAmbienceSource;
    [SerializeField] private AudioSource localSFXSource;

    private Dictionary<int, List<Vector3>> groupFormations = new Dictionary<int, List<Vector3>>();
    private List<Vector3> formationPositions = new List<Vector3>();
    private List<GameObject> formations = new List<GameObject>();
    private NavMeshAgent navAgent;

    private float cohesion;
    private int stance;

    private float xOffset;
    private float zOffset;
    private Vector3 groupRotation;
    private Vector3 oldgroupRotation;

    public int Formation { get => (int)groupFormation; set => groupFormation = (GroupFormation)value; }
    public int Team { get => team; set => team = value; }
    public float Cohesion { get => cohesion; }

    public int Stance
    {
        get => stance;
        set
        {
            stance = value;
            groupStance = (GroupStance)value;
        }
    }

    public NavMeshAgent NavMeshAgent { get => navAgent; }

    private float groupMoveSpeed = 100;
    public float GroupCombatSpeed { get => groupMoveSpeed - ((groupMoveSpeed * 0.05f) * UnitsInCombat.Count); }

    private List<UnitController> unitsInCombat = new List<UnitController>();
    public List<UnitController> UnitsInCombat { get => unitsInCombat; set => unitsInCombat = value; }

    public Dictionary<int, List<Vector3>> GroupFormations { get => groupFormations; set => groupFormations = value; }

    public RTS_UnitController rtsController { get; set; } = null;

    bool standingStill = true;
    float timer = 1;
    #endregion Fields

    private void Start()
    {
        if (formationsInGroup.Count > 0)
        {
            Initiate();
        }
    }

    public void Initiate()
    {
        rtsController = GetComponent<RTS_UnitController>();

        navAgent = GetComponent<NavMeshAgent>();

        for (int i = 0; i < formationsInGroup.Count; i++)
        {
            if (formationsInGroup[i].unitPrefab != null && formationsInGroup[i].unitPrefab.GetComponent<UnitController>().MoveSpeed < groupMoveSpeed)
            {
                groupMoveSpeed = formationsInGroup[i].unitPrefab.GetComponent<UnitController>().MoveSpeed - formationsInGroup[i].unitPrefab.GetComponent<UnitController>().MoveSpeed * 0.4f;
                navAgent.speed = groupMoveSpeed;
            }
        }

        cohesion = ((int)groupStance + 1) * 5 + ((int)groupStance * 5);
        stance = (int)groupStance;

        if (groupFormations.Count == 0)
        {
            GroupSetup();
        }
        else
        {
            groupRotation = transform.forward;
            oldgroupRotation = groupRotation;
        }

        SpawnGroup();
    }

    private void Update()
    {
        groupRotation = transform.forward;

        if (formationPositions[0] != transform.position || groupRotation != oldgroupRotation)
        {
            UpdateFormationPositions();
            oldgroupRotation = groupRotation;
        }

        CheckForDeadFormations();

        if (timer <= 0)
        {
            if (UnitsInCombat.Count > 0)
            {
                UpdateLocalAmbience(Ambience.BattleSounds, true);
                timer = 1;
            }
            else if (NavMeshAgent.isActiveAndEnabled && NavMeshAgent.velocity != Vector3.zero)
            {
                if (standingStill)
                {
                    SoundManager.soundManager.PlayOneShotLocal(SFX.WarHorn2, localSFXSource);
                    standingStill = false;
                }
                UpdateLocalAmbience(Ambience.ArmyMarch, true);
                timer = 1;
            }
            else if (NavMeshAgent.velocity == Vector3.zero)
            {
                standingStill = true;
                SoundManager.soundManager.SwitchLocalAmbienceState(false, localAmbienceSource);
                timer = 1;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    private void GroupSetup()
    {
        groupRotation = transform.forward;
        oldgroupRotation = groupRotation;

        groupFormations.Clear();

        #region Line Formation

        groupFormations.Add(0, new List<Vector3>());

        for (int i = 0; i < 9; i++)
        {
            groupFormations[0].Add(new Vector3(0, 0, 0));
        }

        if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = 4;
        }
        else if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[0].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = 2.5f;
        }

        for (int i = 1; i <= 7; i += 2)
        {
            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle)
            {
                xOffset += 3;
            }
            else
            {
                xOffset += 1;
            }

            groupFormations[0][i] = new Vector3(xOffset, 0, 0);

            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle)
            {
                xOffset += 4;
            }
            else if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[i].formation == FormationDataStruct.FormationTypes.None)
            {
                xOffset += 2.5f;
            }
        }

        if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = 4;
        }
        else if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[0].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = 2.5f;
        }

        for (int i = 2; i <= 8; i += 2)
        {
            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle)
            {
                xOffset += 3;
            }
            else
            {
                xOffset += 1;
            }

            groupFormations[0][i] = new Vector3(-xOffset, 0, 0);

            if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Triangle)
            {
                xOffset += 4;
            }
            else if (formationsInGroup[i].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[i].formation == FormationDataStruct.FormationTypes.None)
            {
                xOffset += 2.5f;
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
            xOffset = 4;
            zOffset = 4;
        }
        else if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[0].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = 2.5f;
            zOffset = 4;
        }

        if (formationsInGroup[1].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2.5f;
            groupFormations[1][1] = new Vector3(xOffset, 0, 0);
            xOffset -= 2.5f;
        }
        else
        {
            xOffset += 1;
            groupFormations[1][1] = new Vector3(xOffset, 0, 0);
            xOffset -= 1;
        }

        if (formationsInGroup[2].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2.5f;
            groupFormations[1][2] = new Vector3(-xOffset, 0, 0);
            xOffset -= 2.5f;
        }
        else
        {
            xOffset += 1;
            groupFormations[1][2] = new Vector3(-xOffset, 0, 0);
            xOffset -= 1;
        }

        groupFormations[1][3] = new Vector3(0, 0, zOffset);
        groupFormations[1][4] = new Vector3(0, 0, -zOffset);

        if (formationsInGroup[3].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = 4;
        }
        else if (formationsInGroup[3].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[3].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = 2.5f;
        }

        if (formationsInGroup[5].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2.5f;
            groupFormations[1][5] = new Vector3(xOffset, 0, zOffset);
            xOffset -= 2.5f;
        }
        else
        {
            xOffset += 1;
            groupFormations[1][5] = new Vector3(xOffset, 0, zOffset);
            xOffset -= 1;
        }

        if (formationsInGroup[6].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2.5f;
            groupFormations[1][6] = new Vector3(-xOffset, 0, zOffset);
            xOffset -= 2.5f;
        }
        else
        {
            xOffset += 1;
            groupFormations[1][6] = new Vector3(-xOffset, 0, zOffset);
            xOffset -= 1;
        }

        if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = 4;
        }
        else if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[4].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = 2.5f;
        }

        if (formationsInGroup[7].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2.5f;
            groupFormations[1][7] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 2.5f;
        }
        else
        {
            xOffset += 1;
            groupFormations[1][7] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 1;
        }

        if (formationsInGroup[8].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2.5f;
            groupFormations[1][8] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 2.5f;
        }
        else
        {
            xOffset += 1;
            groupFormations[1][8] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 1;
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
            xOffset = 4;
            zOffset = 4;
        }
        else if (formationsInGroup[0].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[0].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = 3.5f;
            zOffset = 4;
        }

        if (formationsInGroup[1].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][1] = new Vector3(xOffset, 0, 0);
            xOffset -= 2;
        }
        else
        {
            xOffset += 1;
            groupFormations[2][1] = new Vector3(xOffset, 0, 0);
            xOffset -= 1;
        }

        if (formationsInGroup[2].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][2] = new Vector3(-xOffset, 0, 0);
            xOffset -= 2;
        }
        else
        {
            xOffset += 1;
            groupFormations[2][2] = new Vector3(-xOffset, 0, 0);
            xOffset -= 1;
        }

        groupFormations[2][3] = new Vector3(0, 0, zOffset);
        groupFormations[2][4] = new Vector3(0, 0, -zOffset);

        if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = 4;
        }
        else if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[4].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = 3.5f;
        }

        if (formationsInGroup[5].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][5] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            xOffset += 1;
            groupFormations[2][5] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 1;
        }

        if (formationsInGroup[6].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][6] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            xOffset += 1;
            groupFormations[2][6] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 1;
        }

        if (formationsInGroup[5].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 6;
        }
        else if (formationsInGroup[5].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[5].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset += 4.5f;
        }

        if (formationsInGroup[7].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][7] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            xOffset += 1;
            groupFormations[2][7] = new Vector3(xOffset, 0, -zOffset);
            xOffset -= 1;
        }

        if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset = 4;
        }
        else if (formationsInGroup[4].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[4].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset = 3.5f;
        }
        if (formationsInGroup[6].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 6;
        }
        else if (formationsInGroup[6].formation == FormationDataStruct.FormationTypes.Square || formationsInGroup[6].formation == FormationDataStruct.FormationTypes.None)
        {
            xOffset += 4.5f;
        }

        if (formationsInGroup[8].formation == FormationDataStruct.FormationTypes.Triangle)
        {
            xOffset += 2;
            groupFormations[2][8] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 2;
        }
        else
        {
            xOffset += 1;
            groupFormations[2][8] = new Vector3(-xOffset, 0, -zOffset);
            xOffset -= 1;
        }

        #endregion Triangle Formation
    }

    private void SpawnGroup()
    {
        //Spawn Formations in selected Group
        if (formations.Count == 0)
        {
            for (int i = 0; i < formationsInGroup.Count; i++)
            {
                formationPositions.Add(transform.position + groupFormations[(int)groupFormation][i].x * transform.right + groupFormations[(int)groupFormation][i].z * transform.forward);
                if (formationsInGroup[i].formation != FormationDataStruct.FormationTypes.None && formationsInGroup[i].unitPrefab != null)
                {
                    formations.Add(Instantiate<GameObject>(formationPrefab, formationPositions[i], Quaternion.identity, transform));
                    FormationController controller = formations[i].GetComponent<FormationController>();
                    controller.FormationData = formationsInGroup[i];
                    formations[i].transform.LookAt(formations[i].transform.position + transform.forward * 2);
                    controller.Initiate();
                }
                else
                {
                    formations.Add(Instantiate<GameObject>(formationPrefab, formationPositions[i], Quaternion.identity, transform));
                    formations[i].SetActive(false);
                }
            }
        }
    }

    private void UpdateFormationPositions()
    {
        for (int i = 0; i < formationsInGroup.Count; i++)
        {
            formationPositions[i] = transform.position + groupFormations[(int)groupFormation][i].x * transform.right + groupFormations[(int)groupFormation][i].z * transform.forward;
            if (formations[i].gameObject.activeSelf == false)
            {
                continue;
            }
            formations[i].transform.position = formationPositions[i];
        }
    }

    private void CheckForDeadFormations()
    {
        int deadFormations = 0;
        for (int i = formations.Count - 1; i >= 0; i--)
        {
            if (formations[i].gameObject.activeSelf == false)
            {
                deadFormations++;
            }
        }

        if (formations.Count <= deadFormations)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void UpdateGroupSpeed()
    {
        NavMeshAgent.speed = GroupCombatSpeed;
    }

    private void UpdateLocalAmbience(Ambience ambience, bool shouldPlay) 
    {
        SoundManager.soundManager.ChangeLocalAmbience(ambience, localAmbienceSource);
        SoundManager.soundManager.SwitchLocalAmbienceState(shouldPlay, localAmbienceSource);
    }

    private void OnDrawGizmos()
    {
        Color tempcolor = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1);
        Gizmos.color = tempcolor;
    }
}