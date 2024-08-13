using System.Collections.Generic;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    public GameObject[] shapes;
    private int currentShapeIndex = 0;

    public GameObject GetNextShape()
    {
        currentShapeIndex = (currentShapeIndex + 1) % shapes.Length;
        return shapes[currentShapeIndex];
    }
}
