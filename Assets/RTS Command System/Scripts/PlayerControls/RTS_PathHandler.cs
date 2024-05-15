using UnityEngine;

public class RTS_PathHandler : MonoBehaviour
{
    [SerializeField] [Range(5, 20)] private float minPointDistance = 10;
    [SerializeField] private float maxCommandPoints = 10;

    private float currentCommandPoints = 0;
    private float commandPointsInPlanning = 0;

    public float CurrentCommandPoints
    {
        get => currentCommandPoints;
        set
        {
            currentCommandPoints = value;
            EventHandler.eventHandler.OnFloatValueChange.Invoke(currentCommandPoints, maxCommandPoints, (int)EventHandler.Value.currentCommandPoints);
        }
    }

    private float timer = 1;

    #region Internal Methods

    private RaycastHit GetMouseTarget(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Physics.Raycast(ray, out RaycastHit hitInfo);

        return hitInfo;
    }

    private float GetDistance(Vector3 startPos, Vector3 endPos)
    {
        float distance = Vector3.Distance(endPos, startPos);
        return distance;
    }

    #endregion Internal Methods

    #region Unity Methods

    private void Start()
    {
        CurrentCommandPoints = maxCommandPoints;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (CurrentCommandPoints < maxCommandPoints && timer <= 0)
        {
            CurrentCommandPoints += 1;
            timer = 10;
        }
    }

    #endregion Unity Methods

    public void AddPathPoint(Vector2 point, RTS_UnitController unit)
    {
        Vector3 possiblePoint = GetMouseTarget(point).point;

        if (CurrentCommandPoints > 0 && commandPointsInPlanning < 3)
        {
            if (unit.PathPoints.Count == 0)
            {
                unit.PathPoints.Add(unit.transform.position);
                unit.PathPoints.Add(possiblePoint);

                unit.BezierCurve.DrawBezier(unit.PathPoints);

                CurrentCommandPoints--;
                commandPointsInPlanning++;
            }
            else if (GetDistance(unit.PathPoints[unit.PathPoints.Count - 1], possiblePoint) >= minPointDistance)
            {
                unit.PathPoints.Add(possiblePoint);

                unit.BezierCurve.DrawBezier(unit.PathPoints);
            }

            if (unit.PathPoints.Count > 0)
            {
                if (unit.PathPoints.Count % 10 == 0)
                {
                    CurrentCommandPoints--;
                    commandPointsInPlanning++;
                }
            }
        }
    }

    public void ClearPath(RTS_UnitController unit)
    {
        unit.BezierCurve.ClearBezier();
        unit.PathPoints.Clear();
    }

    public void ResetCommands(bool refund)
    {
        if (refund)
        {
            CurrentCommandPoints += commandPointsInPlanning;
        }
        commandPointsInPlanning = 0;
    }
}