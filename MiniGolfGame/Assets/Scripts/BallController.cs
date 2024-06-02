using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float forceMultiplier = 1f;
    public float boostedForceMultiplier = 2f;
    public float maxForce = 5f;
    public float maxDragDistance = 1f;
    public float minVelocity = 0.1f;

    public ParticleSystem confettiParticles;

    private Vector3 dragStartPos;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private Vector3 ballStartingPos;
    private bool isInputActive;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        ballStartingPos = transform.position;
        isInputActive = true;
    }

    void Update()
    {
        //Stops any further code if the ball is in the hole or restarting its position
        if (!isInputActive || GameManager.Instance.isGamePaused())
        {
            return;
        }

        if (rb.velocity.magnitude <= minVelocity)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (rb.velocity.magnitude == 0)
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

                GameManager.Instance.addStrokeToCurrentLevel();
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

    //On ball collision with =a wall or an obstacle
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Obstacle"))
        {
            Vector3 normal = collision.contacts[0].normal;
            Vector3 vel = rb.velocity;
            rb.velocity = Vector3.Reflect(vel, normal);
        }
    }

    //Called when ball enters the hole
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hole"))
        {
            GameManager.Instance.setHoleUIActive(true);
            isInputActive = false;
            confettiParticles.Play();
        }
    }


    //A function to be called by the PLAY AGAIN button
    public void callRestartBall()
    {
        GameManager.Instance.setHoleUIActive(false);
        GameManager.Instance.resetStrokes();
        StartCoroutine(restartBall());
    }

    //Freezing the ball for 1 second after it gets restarted and moved to its starting position
    IEnumerator restartBall()
    {
        transform.position = ballStartingPos;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(1);
        rb.constraints = RigidbodyConstraints.None;
        isInputActive = true;
    }

    public void RaiseForceMultiplier()
    {
        forceMultiplier = boostedForceMultiplier;
    }
}
