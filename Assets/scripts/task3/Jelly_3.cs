using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jelly_3 : MonoBehaviour
{
    public float maxSpeed = 50f;          // Maximum speed 
    public float minSpeed = 20f;          // Minimum speed
    public Vector3 initialVelocity;
    private Rigidbody rb;
    private ShapeManager shapeManager;

    public GameObject ParentToDestroy;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = initialVelocity;
        rb.useGravity = false;

        //ShapeManager in the scene
        shapeManager = FindObjectOfType<ShapeManager>();
        if (shapeManager == null)
        {
            Debug.LogError("ShapeManager not found in the scene!");
        }
    }

    void Update()
    { 
        AdjustSpeed();

        // Switch shape
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchShape();
        }
    }

    private void AdjustSpeed()
    {
        float currentSpeed = rb.velocity.magnitude;

        if (currentSpeed > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        else if (currentSpeed < minSpeed)
        {
            rb.velocity = rb.velocity.normalized * minSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 reflectDirection = Vector3.Reflect(rb.velocity, normal);

        rb.velocity = reflectDirection;

        // Adjust speed after reflection
        AdjustSpeed();
    }

    private void SwitchShape()
    {
        if (shapeManager == null) return;

        // Get the next shape
        GameObject nextShapePrefab = shapeManager.GetNextShape();

        // Instantiate the new shape at the current position and velocity
        GameObject newShape = Instantiate(nextShapePrefab, transform.position, Quaternion.identity);

        if (newShape.GetComponent<Bounce1>())
        {
            newShape.GetComponent<Bounce1>().initialVelocity = rb.velocity;
        }


        // Destroy the old shape
        Destroy(ParentToDestroy);
    }
}