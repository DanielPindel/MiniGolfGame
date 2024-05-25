using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float forceMultiplier = 1f;
    public float maxForce = 5f; 
    public float maxDragDistance = 1f;
    public float minVelocity = 0.02f;

    private Vector3 dragStartPos;
    private Rigidbody rb;
    private LineRenderer lineRenderer;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (rb.velocity.magnitude <= minVelocity)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (rb.velocity.magnitude <= minVelocity / 2) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStartPos = GetMousePositionOnXZPlane();
                lineRenderer.enabled = true;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 dragCurrentPos = GetMousePositionOnXZPlane();
                Vector3 direction = (dragStartPos - dragCurrentPos).normalized;
                float distance = Mathf.Min(Vector3.Distance(dragStartPos, dragCurrentPos), maxDragDistance);
                Vector3 endPosition = transform.position + direction * distance;

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, endPosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3 dragEndPos = GetMousePositionOnXZPlane();
                Vector3 direction = (dragStartPos - dragEndPos).normalized;
                float distance = Vector3.Distance(dragStartPos, dragEndPos);

                Vector3 force = new Vector3(direction.x, 0, direction.z) * distance * forceMultiplier;
                force = Vector3.ClampMagnitude(force, maxForce);
                rb.AddForce(force, ForceMode.Impulse);



                lineRenderer.enabled = false;
            }
        }
    }

    private Vector3 GetMousePositionOnXZPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xzPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        xzPlane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 vel = rb.velocity;
        rb.velocity = Vector3.Reflect(vel, normal);
        //rb.AddForce(Vector3.Reflect(vel, normal), ForceMode.Impulse);
        Debug.Log("angle: " + Vector3.Angle(vel, Vector3.Reflect(vel, normal)));
    }
}
