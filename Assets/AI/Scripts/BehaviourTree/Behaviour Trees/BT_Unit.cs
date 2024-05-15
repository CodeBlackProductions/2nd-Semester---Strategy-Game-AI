using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(UnitController))]
public class BT_Unit : BehaviourTreeClass
{
    public enum UnitBlackBoardKey
    {
        NavMeshAgent,
        UnitController,
        MoveToTarget,
        OldMoveToTarget,
        DistanceToTargetPoint,
        Team,
        Stance,
        UnitCohesion,
        VisionRange,
        AttackRange,
        MoveSpeed,
        RotationSpeed,
        TargetEnemy,
        TargetEnemyController,
        TargetEnemyPosition,
        DistanceToTargetEnemy,
        DistanceFormationPointToEnemy,
        IsRanged,
        IsInAnim,
        AlliesInCombat,
        EnemiesInSight
    }

    public override void Initialize()
    {
        SetupBlackboard();

        BuildTree();

        blackboard.Variables[(int)UnitBlackBoardKey.NavMeshAgent].speed = blackboard.Variables[(int)UnitBlackBoardKey.MoveSpeed];
    }

    /// <summary>
    /// Adds necessary blackboard values for the AI to function.
    /// </summary>
    public override void SetupBlackboard()
    {
        //Normally generated from external script (Unit-/ Formation-Controller).
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.MoveToTarget))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.MoveToTarget, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.UnitCohesion))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.UnitCohesion, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.Team))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.Team, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.VisionRange))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.VisionRange, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.AttackRange))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.AttackRange, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.MoveSpeed))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.MoveSpeed, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.RotationSpeed))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.RotationSpeed, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.AlliesInCombat))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.AlliesInCombat, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.Stance))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.Stance, null);
        }
        if (!blackboard.Variables.ContainsKey((int)UnitBlackBoardKey.EnemiesInSight))
        {
            blackboard.Variables.Add((int)UnitBlackBoardKey.EnemiesInSight, null);
        }

        //Internal
        blackboard.Variables.Add((int)UnitBlackBoardKey.NavMeshAgent, transform.gameObject.GetComponent<NavMeshAgent>());
        blackboard.Variables.Add((int)UnitBlackBoardKey.UnitController, transform.gameObject.GetComponent<UnitController>());
        blackboard.Variables.Add((int)UnitBlackBoardKey.DistanceToTargetPoint, Vector3.SqrMagnitude(blackboard.Variables[(int)UnitBlackBoardKey.MoveToTarget] - this.transform.position));
        blackboard.Variables.Add((int)UnitBlackBoardKey.TargetEnemy, null);
        blackboard.Variables.Add((int)UnitBlackBoardKey.TargetEnemyController, null);
        blackboard.Variables.Add((int)UnitBlackBoardKey.TargetEnemyPosition, null);
        blackboard.Variables.Add((int)UnitBlackBoardKey.DistanceToTargetEnemy, null);
        blackboard.Variables.Add((int)UnitBlackBoardKey.DistanceFormationPointToEnemy, null);
        blackboard.Variables.Add((int)UnitBlackBoardKey.OldMoveToTarget, null);
        blackboard.Variables.Add((int)UnitBlackBoardKey.IsInAnim, false);
        
    }

    /// <summary>
    /// Builds the actual BT. Adds nodes and decorators.
    /// </summary>
    public override void BuildTree()
    {
        startNode = new Selector();
        //Build tree structure
        //Layer 0
        AddChildren(startNode, new Selector(), new Selector(), new Sequence());

        #region Left Sub Tree

        //Layer 1
        Selector tempSelector = (Selector)startNode;
        AddChildren(tempSelector.childNodes[0], new Sequence(), new Sequence());

        //Layer 2 Left
        tempSelector = (Selector)tempSelector.childNodes[0];
        Sequence tempSequence = (Sequence)tempSelector.childNodes[0];
        AddChildren(tempSequence, new RotateTowards((int)UnitBlackBoardKey.TargetEnemyPosition, (int)UnitBlackBoardKey.RotationSpeed));
        AddChildren(tempSequence, new AttackTarget((int)UnitBlackBoardKey.UnitController, (int)UnitBlackBoardKey.TargetEnemyController));

        //Layer 2 Right
        tempSequence = (Sequence)tempSelector.childNodes[1];
        AddChildren(tempSequence, new MoveToPoint((int)UnitBlackBoardKey.NavMeshAgent, (int)UnitBlackBoardKey.TargetEnemyPosition, (int)UnitBlackBoardKey.OldMoveToTarget));
        AddChildren(tempSequence, new RotateTowards((int)UnitBlackBoardKey.TargetEnemyPosition, (int)UnitBlackBoardKey.RotationSpeed));

        //Add Decorators
        //Layer 0
        tempSelector = (Selector)startNode;

        tempSelector.childNodes[0].AddDecorator(new IsObjectSet((int)UnitBlackBoardKey.TargetEnemy, false));
        tempSelector.childNodes[0].decorators[0].setParent(tempSelector.childNodes[0]);
        tempSelector.childNodes[0].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.DistanceToTargetEnemy, (int)UnitBlackBoardKey.VisionRange, ValueComparison.ComparisonType.smaller));
        tempSelector.childNodes[0].decorators[1].setParent(tempSelector.childNodes[0]);
        tempSelector.childNodes[0].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.DistanceFormationPointToEnemy, (int)UnitBlackBoardKey.UnitCohesion, ValueComparison.ComparisonType.smaller));
        tempSelector.childNodes[0].decorators[2].setParent(tempSelector.childNodes[0]);

        //Layer 1
        tempSelector = (Selector)tempSelector.childNodes[0];

        //Layer 1 Left
        tempSelector.childNodes[0].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.DistanceToTargetEnemy, (int)UnitBlackBoardKey.AttackRange, ValueComparison.ComparisonType.smaller));
        tempSelector.childNodes[0].decorators[0].setParent(tempSelector.childNodes[0]);

        //Layer 1 Right
        tempSelector.childNodes[1].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.DistanceFormationPointToEnemy, (int)UnitBlackBoardKey.UnitCohesion, ValueComparison.ComparisonType.smaller));
        tempSelector.childNodes[1].decorators[0].setParent(tempSelector.childNodes[1]);
        tempSelector.childNodes[1].AddDecorator(new BoolComparison((int)UnitBlackBoardKey.IsRanged, true));
        tempSelector.childNodes[1].decorators[1].setParent(tempSelector.childNodes[1]);

        //Layer 2 Right
        tempSequence = (Sequence)tempSelector.childNodes[1];

        tempSequence.childNodes[0].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.TargetEnemyPosition, (int)UnitBlackBoardKey.OldMoveToTarget, ValueComparison.ComparisonType.notEqual));
        tempSequence.childNodes[0].decorators[0].setParent(tempSequence.childNodes[0]);

        #endregion Left Sub Tree

        #region Middle Sub Tree

        //Layer 1
        tempSelector = (Selector)startNode;
        AddChildren(tempSelector.childNodes[1], new SetVariable<BT_Unit>((int)UnitBlackBoardKey.AlliesInCombat, -1, (int)UnitBlackBoardKey.TargetEnemy), new Selector());

        //Layer 2 Right
        tempSelector = (Selector)tempSelector.childNodes[1];
        AddChildren(tempSelector.childNodes[1], new Sequence(), new Sequence());

        //Layer 3 Left
        tempSelector = (Selector)tempSelector.childNodes[1];
        tempSequence = (Sequence)tempSelector.childNodes[0];
        AddChildren(tempSequence, new RotateTowards((int)UnitBlackBoardKey.TargetEnemyPosition, (int)UnitBlackBoardKey.RotationSpeed));
        AddChildren(tempSequence, new AttackTarget((int)UnitBlackBoardKey.UnitController, (int)UnitBlackBoardKey.TargetEnemyController));

        //Layer 3 Right
        tempSequence = (Sequence)tempSelector.childNodes[1];
        AddChildren(tempSequence, new MoveToPoint((int)UnitBlackBoardKey.NavMeshAgent, (int)UnitBlackBoardKey.TargetEnemyPosition, (int)UnitBlackBoardKey.OldMoveToTarget));
        AddChildren(tempSequence, new RotateTowards((int)UnitBlackBoardKey.TargetEnemyPosition, (int)UnitBlackBoardKey.RotationSpeed));

        //Add Decorators
        //Layer 0
        tempSelector = (Selector)startNode;

        tempSelector.childNodes[1].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.AlliesInCombat, 0.0f, ValueComparison.ComparisonType.bigger));
        tempSelector.childNodes[1].decorators[0].setParent(tempSelector.childNodes[1]);
        tempSelector.childNodes[1].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.Stance, 0.0f, ValueComparison.ComparisonType.bigger));
        tempSelector.childNodes[1].decorators[1].setParent(tempSelector.childNodes[1]);

        //Layer 1
        tempSelector = (Selector)tempSelector.childNodes[1];

        //Layer 1 Left
        tempSelector.childNodes[0].AddDecorator(new IsObjectSet((int)UnitBlackBoardKey.TargetEnemy, true));
        tempSelector.childNodes[0].decorators[0].setParent(tempSelector.childNodes[0]);
        tempSelector.childNodes[0].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.EnemiesInSight,0.0f,ValueComparison.ComparisonType.equal));
        tempSelector.childNodes[0].decorators[1].setParent(tempSelector.childNodes[0]);

        //Layer 1 Right
        tempSelector.childNodes[1].AddDecorator(new IsObjectSet((int)UnitBlackBoardKey.TargetEnemy, false));
        tempSelector.childNodes[1].decorators[0].setParent(tempSelector.childNodes[1]);

        //Layer 2
        tempSelector = (Selector)tempSelector.childNodes[1];

        //Layer 2 Left
        tempSelector.childNodes[0].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.DistanceToTargetEnemy, (int)UnitBlackBoardKey.AttackRange, ValueComparison.ComparisonType.smaller));
        tempSelector.childNodes[0].decorators[0].setParent(tempSelector.childNodes[0]);

        //Layer 2 Right
        tempSelector.childNodes[1].AddDecorator(new BoolComparison((int)UnitBlackBoardKey.IsRanged, true));
        tempSelector.childNodes[1].decorators[0].setParent(tempSelector.childNodes[1]);

        #endregion Middle Sub Tree

        #region Right Sub Tree

        //Layer 1
        tempSelector = (Selector)startNode;
        AddChildren(tempSelector.childNodes[2], new MoveToPoint((int)UnitBlackBoardKey.NavMeshAgent, (int)UnitBlackBoardKey.MoveToTarget, (int)UnitBlackBoardKey.OldMoveToTarget));
        AddChildren(tempSelector.childNodes[2], new RotateTowards((int)UnitBlackBoardKey.MoveToTarget, (int)UnitBlackBoardKey.RotationSpeed));

        //Add Decorators
        //Layer 0
        tempSelector = (Selector)startNode;

        tempSelector.childNodes[2].AddDecorator(new ValueComparison((int)UnitBlackBoardKey.MoveToTarget, (int)UnitBlackBoardKey.OldMoveToTarget, ValueComparison.ComparisonType.notEqual));
        tempSelector.childNodes[2].decorators[0].setParent(tempSelector.childNodes[2]);

        #endregion Right Sub Tree
    }

    /// <summary>
    /// Updates internal blackboard values like distances.
    /// </summary>
    public override void LateUpdate()
    {
        blackboard.Variables[(int)UnitBlackBoardKey.DistanceToTargetPoint] = Vector3.SqrMagnitude(blackboard.Variables[(int)UnitBlackBoardKey.MoveToTarget] - this.transform.position);
        if (blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemy] != null)
        {
            if (blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemy].gameObject.activeSelf)
            {
                blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemyController] = blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemy].GetComponent<UnitController>();
                blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemyPosition] = blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemy].transform.position;
                blackboard.Variables[(int)UnitBlackBoardKey.DistanceToTargetEnemy] = Vector3.SqrMagnitude(blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemy].transform.position - this.transform.position);
                blackboard.Variables[(int)UnitBlackBoardKey.DistanceFormationPointToEnemy] = Vector3.SqrMagnitude(blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemy].transform.position - blackboard.Variables[(int)UnitBlackBoardKey.MoveToTarget]);

            }
            else
            {
                blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemy] = null;
            }
        }

        //Set InCombat status & Reduce moveSpeed when in combat
        if (blackboard.Variables[(int)UnitBlackBoardKey.TargetEnemy] != null && (blackboard.Variables[(int)UnitBlackBoardKey.DistanceToTargetEnemy] < blackboard.Variables[(int)UnitBlackBoardKey.UnitCohesion] * 0.5f || blackboard.Variables[(int)UnitBlackBoardKey.DistanceToTargetEnemy] < blackboard.Variables[(int)UnitBlackBoardKey.AttackRange]))
        {
            if (!blackboard.Variables[(int)UnitBlackBoardKey.UnitController].InCombat)
            {
                blackboard.Variables[(int)UnitBlackBoardKey.UnitController].InCombat = true;
                if (blackboard.Variables[(int)UnitBlackBoardKey.DistanceToTargetEnemy] < blackboard.Variables[(int)UnitBlackBoardKey.AttackRange])
                {
                    blackboard.Variables[(int)UnitBlackBoardKey.NavMeshAgent].speed = blackboard.Variables[(int)UnitBlackBoardKey.MoveSpeed] * 0.1f;
                    blackboard.Variables[(int)UnitBlackBoardKey.UnitController].ParentFormation.ParentGroup.UpdateGroupSpeed();
                }
            }
        }
        else
        {
            if (blackboard.Variables[(int)UnitBlackBoardKey.UnitController].InCombat)
            {
                blackboard.Variables[(int)UnitBlackBoardKey.UnitController].InCombat = false;
                blackboard.Variables[(int)UnitBlackBoardKey.NavMeshAgent].speed = blackboard.Variables[(int)UnitBlackBoardKey.MoveSpeed];
                blackboard.Variables[(int)UnitBlackBoardKey.UnitController].ParentFormation.ParentGroup.UpdateGroupSpeed();
            }
        }

        //Increase moveSpeed when far away from formation
        if (blackboard.Variables[(int)UnitBlackBoardKey.DistanceToTargetPoint] > blackboard.Variables[(int)UnitBlackBoardKey.UnitCohesion] * 0.5f)
        {
            blackboard.Variables[(int)UnitBlackBoardKey.NavMeshAgent].speed = blackboard.Variables[(int)UnitBlackBoardKey.MoveSpeed] * 1.25f;
        }
        else
        {
            blackboard.Variables[(int)UnitBlackBoardKey.NavMeshAgent].speed = blackboard.Variables[(int)UnitBlackBoardKey.MoveSpeed];
        }
    }

    //public void OnDrawGizmos()
    //{
    //    Color tempcolor = Gizmos.color;
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(this.transform.position, Mathf.Sqrt(blackboard.Variables[(int)UnitBlackBoardKeys.visionRange]));
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(blackboard.Variables[(int)UnitBlackBoardKeys.moveToTarget], Mathf.Sqrt(blackboard.Variables[(int)UnitBlackBoardKeys.unitCohesion]));
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(this.transform.position, Mathf.Sqrt(blackboard.Variables[(int)UnitBlackBoardKeys.attackRange]));
    //    Gizmos.color = tempcolor;
    //}
}