using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    [SerializeField] [Range(0.01f, 0.5f)] private float stepSize;

    private LineRenderer lineRenderer;

    private List<Vector3> bezierPoints = new List<Vector3>();

    public List<Vector3> BezierPoints { get => bezierPoints;}

    public void DrawBezier(List<Vector3> inputPoints)
    {
        bezierPoints.Clear();

        for (float s = 0; s < 1; s += stepSize)
        {
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < inputPoints.Count; i++)
            {
                points.Add(inputPoints[i]);
            }

            bezierPoints.Add(Bezier(points, s));
        }

        lineRenderer.positionCount = bezierPoints.Count;

        for (int i = 0; i < bezierPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, bezierPoints[i]);
        }

        lineRenderer.enabled = true;
    }

    public void ClearBezier()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        bezierPoints.Clear();
    }

    public void SwitchBezierVisibility(bool visible)
    {
        if (visible && lineRenderer.positionCount > 0)
        {
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    public void UpdateBezier()
    {
        bool selected = false;
        if (lineRenderer.enabled)
        {
            selected = true;
            lineRenderer.enabled = false;
        }

        lineRenderer.positionCount = bezierPoints.Count;

        for (int i = 0; i < bezierPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, bezierPoints[i]);
        }

        if (selected)
        {
            lineRenderer.enabled = true;
        }
       
    }

    #region Internal Methods

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private Vector3 Bezier(List<Vector3> points, float t)
    {
        while (points.Count > 1)
        {
            BezierStep(t, points);
        }

        return points[0];
    }

    private void BezierStep(float t, List<Vector3> points)
    {
        List<Vector3> tempPoints = new List<Vector3>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            tempPoints.Add(Vector3.Lerp(points[i], points[i + 1], t));
        }

        points.Clear();
        for (int i = 0; i < tempPoints.Count; i++)
        {
            points.Add(tempPoints[i]);
        }
    }

    #endregion Internal Methods
}