using UnityEngine;

public class Simple_Path : MonoBehaviour
{
    public Transform[] waypoints;       // Array of waypoints for movement
    public GameObject Points_Parent;
    
    [Space]
    public float speed = 5f;            // Speed of movement of Player
    private int currentWaypointIndex = 0;
    
    // to draw path of way
    private LineRenderer lineRenderer;
    public Color PathColor = Color.red;


    [Header("Player")]
    public GameObject Player;

    private void Start()
    {
        UpdateWaypoints();
        InitializeLineRenderer();
        UpdateLineRendererPositions();
    }

    private void UpdateWaypoints()
    {
        // Get all points that are children of the Points_Parent GameObject
        waypoints = new Transform[Points_Parent.transform.childCount];
        for (int i = 0; i < Points_Parent.transform.childCount; i++)
            waypoints[i] = Points_Parent.transform.GetChild(i);
    }

    private void InitializeLineRenderer()
    {
        // Initialize the LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        //Add if Line Renderer is missing
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure the LineRenderer
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = PathColor;
        lineRenderer.endColor = PathColor;
        lineRenderer.loop = true;
    }

    private void UpdateLineRendererPositions()
    {
        // Set the positions of the LineRenderer
        if (lineRenderer != null && waypoints != null)
        {
            lineRenderer.positionCount = waypoints.Length;
            for (int i = 0; i < waypoints.Length; i++)
            {
                lineRenderer.SetPosition(i, waypoints[i].position);
            }
        }
    }

    void Update()
    {
        if (waypoints.Length == 0)
            return;
        
        // Move towards the current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - Player.transform.position;
        Player.transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Rotate to face the moving direction
        if (direction != Vector3.zero)
            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speed);

        // Check if the object reached the waypoint
        if (Vector3.Distance(Player.transform.position, targetWaypoint.position) < 0.1f)
        {
            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        UpdateLineRendererPositions();
    }

    private void OnDrawGizmos()
    {
        if (Points_Parent == null || Points_Parent.transform.childCount < 2)
            return;

        // Update waypoints
        UpdateWaypoints();

        Gizmos.color = PathColor;

        // Draw lines
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        //close the loop if needed
        if (waypoints.Length > 2)
        {
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}
