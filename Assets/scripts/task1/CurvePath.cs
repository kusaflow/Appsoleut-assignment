using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvePath : MonoBehaviour
{
    public GameObject Points_Parent;    // Parent GameObject containing CurvePointSets
    [Space]
    public float speed = 5f;           // Speed of movement of player

    private int currentSegment = 0;     // Current curve segment index
    private float t = 0f;               // interpolate along the Bezier curve

    private List<Transform[]> segments = new List<Transform[]>(); // List of control point segments

    [Header("Player")]
    public GameObject Player;

    [Header("Transition")]
    public float transitionSpeed = 10f; // Speed during transition to the next segment, filling the gap
    private bool b_transitioning = false; // Flag to check if we're transitioning

    private LineRenderer lineRenderer;  // LineRenderer for visualizing the curve
    public int lineSegments = 100;      // Number of segments for drawing the line
    public Color PathColor = Color.red; // Color of the path line

    private void Start()
    {
        // Initialize control point segments
        InitializeSegments();

        // Initialize the LineRenderer
        InitializeLineRenderer();

        // Draw the curve initially
        DrawCurve();
    }

    private void InitializeSegments()
    {
        segments.Clear();

        // Going through each child GameObject of Points_Parent
        for (int i = 0; i < Points_Parent.transform.childCount; i++)
        {
            // Get the control point group
            Transform group = Points_Parent.transform.GetChild(i);

            // Ensure the group contains exactly 4 points
            if (group.childCount == 4)
            {
                Transform[] controlPoints = new Transform[4];
                for (int j = 0; j < 4; j++)
                {
                    controlPoints[j] = group.GetChild(j);
                }
                segments.Add(controlPoints);
            }
            else
            {
                Debug.LogWarning("Each control point group must contain exactly 4 points.");
            }
        }
    }

    private void InitializeLineRenderer()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.widthMultiplier = .3f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = PathColor;
        lineRenderer.endColor = PathColor;
        lineRenderer.positionCount = lineSegments * segments.Count;
    }

    private void DrawCurve()
    {
        if (lineRenderer == null || segments.Count == 0)
            return;

        int index = 0;

        foreach (var controlPoints in segments)
        {
            for (int i = 0; i < lineSegments; i++)
            {
                float t = i / (float)lineSegments;
                Vector3 point = BezierCurve.GetPoint(
                    controlPoints[0].position,
                    controlPoints[1].position,
                    controlPoints[2].position,
                    controlPoints[3].position,
                    t);
                lineRenderer.SetPosition(index++, point);
            }
        }
    }

    void Update()
    {
        if (segments.Count == 0 || b_transitioning)
            return;

        // Get the current segment's control points
        Transform[] controlPoints = segments[currentSegment];

        // Calculate the position on the Bezier curve
        Vector3 newPosition = BezierCurve.GetPoint(
            controlPoints[0].position,
            controlPoints[1].position,
            controlPoints[2].position,
            controlPoints[3].position,
            t);

        // Move to the calculated position
        Player.transform.position = newPosition;

        // Rotate to face the moving direction
        Vector3 direction = BezierCurve.GetPoint(
            controlPoints[0].position,
            controlPoints[1].position,
            controlPoints[2].position,
            controlPoints[3].position,
            t + 0.01f) - newPosition;

        if (direction != Vector3.zero)
            Player.transform.rotation = Quaternion.LookRotation(direction);

        // Increment t to move along the curve
        t += Time.deltaTime * speed / Vector3.Distance(controlPoints[0].position, controlPoints[3].position);

        // Reset t and switch to the next segment when reaching the end of the curve
        if (t > 1f)
        {
            t = 1f;
            StartCoroutine(SmoothTransitionToNextSegment());
            //currentSegment = (currentSegment + 1) % segments.Count;
        }
    }

    private IEnumerator SmoothTransitionToNextSegment()
    {
        b_transitioning = true;

        // Get the start point of the next segment
        int nextSegmentIndex = (currentSegment + 1) % segments.Count;
        Transform[] nextControlPoints = segments[nextSegmentIndex];
        Vector3 nextStartPoint = nextControlPoints[0].position;

        // Transition to the next segment's starting point
        while (Vector3.Distance(Player.transform.position, nextStartPoint) > 0.01f)
        {
            Player.transform.position = Vector3.MoveTowards(Player.transform.position, nextStartPoint, transitionSpeed * Time.deltaTime);

            Vector3 direction = nextStartPoint - Player.transform.position;
            if (direction != Vector3.zero)
                Player.transform.rotation = Quaternion.LookRotation(direction);

            yield return null;
        }

        // Move to the next segment
        currentSegment = nextSegmentIndex;
        t = 0f;
        b_transitioning = false;
    }

    private void OnDrawGizmos()
    {
        if (Points_Parent == null)
            return;

        // Visualize each Bezier curve segment
        for (int i = 0; i < Points_Parent.transform.childCount; i++)
        {
            Transform group = Points_Parent.transform.GetChild(i);

            if (group.childCount == 4)
            {
                Transform[] controlPoints = new Transform[4];
                for (int j = 0; j < 4; j++)
                {
                    controlPoints[j] = group.GetChild(j);
                }
                Gizmos.color = Color.white;
                Gizmos.DrawLine(controlPoints[1].position, controlPoints[0].position);
                Gizmos.DrawLine(controlPoints[2].position, controlPoints[3].position);

                Gizmos.color = Color.red;
                for (float t = 0; t < 1; t += 0.01f)
                {
                    Vector3 point = BezierCurve.GetPoint(
                        controlPoints[0].position,
                        controlPoints[1].position,
                        controlPoints[2].position,
                        controlPoints[3].position,
                        t);
                    Gizmos.DrawSphere(point, 0.1f);
                }
            }
        }
    }
}