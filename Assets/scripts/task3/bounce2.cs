using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bounce2 : MonoBehaviour
{
    public float maxSpeed = 50f;          // Maximum speed 
    public Vector3 initialVelocity;
    private Vector3 velocity;             // Current velocity

    private ShapeManager shapeManager;


    void Start()
    {
        // Init
        velocity = initialVelocity.normalized * Mathf.Clamp(initialVelocity.magnitude, 0, maxSpeed);

        //ShapeManager in the scene
        shapeManager = FindObjectOfType<ShapeManager>();
        if (shapeManager == null)
        {
            Debug.LogError("ShapeManager not found in the scene!");
        }
    }

    void Update()
    {
        // Update the position based on the current velocity
        transform.position += velocity * Time.deltaTime;

        // Check for collision
        CheckCollisions();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchShape();
        }
    }

    private void CheckCollisions()
    {
        // current position
        Vector3 position = transform.position;

        // room data
        float roomLeft = -9.76f + .5f;
        float roomRight = 9.76f - .5f;
        float roomBottom = 0f + .5f;
        float roomTop = 9.76f - .5f;
        float roomBack = -9.76f + .5f;
        float roomFront = 9.76f - .5f;

        // Reflect the velocity if the object hits a wall
        if (position.x <= roomLeft || position.x >= roomRight)
        {
            velocity.x = -velocity.x;
            position.x = Mathf.Clamp(position.x, roomLeft, roomRight);
        }
        if (position.y <= roomBottom || position.y >= roomTop)
        {
            velocity.y = -velocity.y;
            position.y = Mathf.Clamp(position.y, roomBottom, roomTop);
        }
        if (position.z <= roomBack || position.z >= roomFront)
        {
            velocity.z = -velocity.z;
            position.z = Mathf.Clamp(position.z, roomBack, roomFront);
        }

        // Update the position with clamped values to prevent moving outside the bounds
        transform.position = position;
    }

    

    private void SwitchShape()
    {
        if (shapeManager == null) return;

        // Get the next shape
        GameObject nextShapePrefab = shapeManager.GetNextShape();

        // Instantiate the new shape at the current position and velocity
        GameObject newShape = Instantiate(nextShapePrefab, transform.position, Quaternion.identity);

        if (newShape.GetComponent<bounce2>())
        {
            newShape.GetComponent<bounce2>().initialVelocity = velocity;
        }

        // Destroy the old shape
        Destroy(gameObject);
    }
}