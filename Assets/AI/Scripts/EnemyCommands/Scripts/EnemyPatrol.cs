using System.Collections.Generic;
using UnityEngine;

internal enum LoopType
{ Reverse, StraightToStart };

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private RTS_UnitController targetUnit;
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private bool loop = true;
    [SerializeField] private LoopType loopType = LoopType.Reverse;

    private List<Vector3> patrolPositions = new List<Vector3>();

    private void Start()
    {
        targetUnit.aiUnit = true;
        ConvertTransformList();
        targetUnit.BezierCurve.DrawBezier(patrolPositions);
    }

    private void Update()
    {
        if (targetUnit.GameIsRunning)
        {
            if (loop && targetUnit.navPoints.Count <= 1 && loopType == LoopType.Reverse)
            {
                targetUnit.navPoints.Clear();

                patrolPositions.Reverse();

                targetUnit.BezierCurve.DrawBezier(patrolPositions);
            }
            else if (loop && targetUnit.navPoints.Count <= 1 && loopType == LoopType.StraightToStart)
            {
                targetUnit.navPoints.Clear();

                targetUnit.BezierCurve.DrawBezier(patrolPositions);
            }
        }
    }

    private void ConvertTransformList()
    {
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            if (i == 0)
            {
                patrolPositions.Add(targetUnit.transform.position);
            }
            else
            {
                patrolPositions.Add(patrolPoints[i].position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(patrolPoints[i].position, 1);
            if (i + 1 <= patrolPoints.Count -1)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i+1].position);
            }
            
        }
    }
}