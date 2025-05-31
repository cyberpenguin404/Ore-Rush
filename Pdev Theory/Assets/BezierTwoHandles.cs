using System.Collections.Generic;
using UnityEngine;

public class BezierTwoHandles : MonoBehaviour
{
    private const int Radius = 1;
    [SerializeField]
    private float _threshold;
    [SerializeField, Range(1,100)]
    private int _resolution;
    [SerializeField]
    private List<Vector3> _points = new List<Vector3>();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_points[0], _points[3]);
        foreach (Vector3 p in _points)
        {
            Gizmos.DrawSphere(p, Radius);
        }
        Gizmos.DrawSphere(CalculateBezierTwoHandles(_threshold, _points), Radius);
        for (int i = 1; i < _resolution; i++)
        {
            float treshold = (1 / (float)_resolution) * i;
            Debug.Log(treshold);
            Gizmos.DrawSphere(CalculateBezierTwoHandles(treshold, _points), Radius);
        }
    }
    private Vector3 CalculateBezierTwoHandles(float t, List<Vector3> points)
    {
        if (points.Count < 4)
        {
            Debug.LogError("Not enough points!");
            return points[0];
        }
        Vector3 p1 = Vector3.Lerp(points[1], points[2], t);
        Vector3 p2 = Vector3.Lerp(points[0], points[1], t);
        Vector3 p3 = Vector3.Lerp(points[2], points[3], t);


        Vector3 p4 = Vector3.Lerp(p1, p2, t);
        Vector3 p5 = Vector3.Lerp(p2, p3, t);
        Vector3 p6 = Vector3.Lerp(p1, p3, t);

        Vector3 p7 = Vector3.Lerp(p4, p5, t);
        Vector3 p8 = Vector3.Lerp(p5, p6, t);

        return Vector3.Lerp(p7, p8, t);
    }
}
