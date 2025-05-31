using System.Collections.Generic;
using UnityEngine;

public class Quadraticqbezier : MonoBehaviour
{
    private const int Radius = 1;
    [SerializeField]
    private float _threshold;
    [SerializeField]
    private List<Vector3> _points = new List<Vector3>();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_points[0], _points[1]);
        Gizmos.DrawLine(_points[1], _points[2]);
        foreach (Vector3 p in _points)
        {
            Gizmos.DrawSphere(p, Radius);
        }
        Gizmos.DrawSphere(CalculateQuadraticBezier(_threshold, _points), Radius);
    }
    private Vector3 CalculateQuadraticBezier(float t, List<Vector3> points)
    {
        if (points.Count < 3)
        {
            Debug.LogError("Not enough points!");
            return points[0];
        }
        Vector3 p1 = Vector3.Lerp(points[0], points[1], t);
        Vector3 p2 = Vector3.Lerp(points[1], points[2], t);

        return Vector3.Lerp(p1, p2, t);
    }
}
