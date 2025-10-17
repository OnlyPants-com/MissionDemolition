using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DottedLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public int numPoints = 30;        // Number of points in the dotted trajectory
    public float timeStep = 0.1f;     // Time step between each point in seconds

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    public void DrawDottedLine(Vector3 startPos, Vector3 startVelocity)
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < numPoints; i++)
        {
            float t = i * timeStep;

            float x = startPos.x + startVelocity.x * t;
            float y = startPos.y + startVelocity.y * t + 0.5f * Physics2D.gravity.y * t * t;

            points.Add(new Vector3(x, y, 0));
        }

        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }

    }
}
