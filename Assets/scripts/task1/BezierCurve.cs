using UnityEngine;

public class BezierCurve
{
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;       // 1sr term
        p += 3 * uu * t * p1;       // 2nd term
        p += 3 * u * tt * p2;       // 3rd term
        p += ttt * p3;              // 4th term

        return p;
    }
}
