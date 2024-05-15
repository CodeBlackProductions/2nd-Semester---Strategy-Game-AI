using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BezierCurve))]
public class RTS_UnitController : MonoBehaviour
{
    private bool gameIsRunning = false;

    private bool selected = false;

    public bool aiUnit = false;

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            EventHandler.eventHandler.OnSelection?.Invoke();
            BezierCurve.SwitchBezierVisibility(value);
        }
    }

    public BezierCurve BezierCurve { get; private set; }

    public bool GameIsRunning { get => gameIsRunning;}

    public List<Vector3> PathPoints { get; set; } = new List<Vector3>();

    private NavMeshAgent agent;
    public Stack<Vector3> navPoints = new Stack<Vector3>();

    private void OnStartGame()
    {
        gameIsRunning = true;
    }

    private void Awake()
    {
        BezierCurve = GetComponent<BezierCurve>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        EventHandler.eventHandler.RTSStartBattle += OnStartGame;
    }

    private void Update()
    {
        if (BezierCurve.BezierPoints.Count > 0)
        {
            if (gameIsRunning)
            {
                if (!navPoints.Contains(BezierCurve.BezierPoints[BezierCurve.BezierPoints.Count - 1]))
                {
                    navPoints.Clear();
                }

                if (navPoints.Count == 0)
                {
                    for (int i = BezierCurve.BezierPoints.Count - 1; i > 0; i--)
                    {
                        navPoints.Push(BezierCurve.BezierPoints[i]);
                    }
                }
                else
                {
                    if (agent.remainingDistance <= 1 && navPoints.Count > 1)
                    {
                        agent.SetDestination(navPoints.Peek());
                        BezierCurve.BezierPoints.RemoveAt(BezierCurve.BezierPoints.IndexOf(navPoints.Pop()) - 1);
                        BezierCurve.UpdateBezier();
                    }
                    else if (agent.remainingDistance <= 1 && navPoints.Count <= 1)
                    {
                        BezierCurve.ClearBezier();
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        EventHandler.eventHandler.RTSStartBattle -= OnStartGame;
    }
}