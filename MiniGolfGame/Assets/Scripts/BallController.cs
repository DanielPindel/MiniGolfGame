using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float forceMultiplier;
    public float boostedForceMultiplier;
    public float maxForce;
    //public float maxDragDistance = 1f;
    public float minVelocity;

    public ParticleSystem confettiParticles;
    public AudioClip[] wallHitSoundClips;
    public AudioClip[] obstacleHitSoundClips;
    public AudioClip[] whooshSoundClips;
    public AudioClip[] ballInHoleSoundClips;
    public AudioClip[] confettiSoundClips;


    private Vector3 dragStartPos;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private Vector3 ballStartingPos;
    private bool isInputActive;
    private bool clickHoldStarted;


    void Start()
    {
        forceMultiplier = 20f;
        boostedForceMultiplier = 20f;
        maxForce = 10f;
        minVelocity = 0.1f;
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        ballStartingPos = transform.position;
        isInputActive = true;
        confettiParticles = GameObject.Find("ConfettiParticles").GetComponent<ParticleSystem>();
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
                clickHoldStarted = true;
            }

            // If the ball stops after we click and keep holding the mouse, the line will not show but the ball
            // will register the input and shoot after we let go of the mouse button. This is here to prevent that
            if (!clickHoldStarted)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 dragCurrentPos = GetMousePositionOnXZPlane();
                Vector3 direction = (dragStartPos - dragCurrentPos).normalized;
                float distance = Vector3.Distance(dragStartPos, dragCurrentPos) / maxForce;
                Vector3 force = new Vector3(direction.x, 0, direction.z) * distance * forceMultiplier;
                force = Vector3.ClampMagnitude(force, maxForce);

                Vector3 endPosition = transform.position + direction * force.magnitude / maxForce;

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, endPosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3 dragEndPos = GetMousePositionOnXZPlane();
                Vector3 direction = (dragStartPos - dragEndPos).normalized;
                float distance = Vector3.Distance(dragStartPos, dragEndPos) / maxForce;
                Vector3 force = new Vector3(direction.x, 0, direction.z) * distance * forceMultiplier;
                force = Vector3.ClampMagnitude(force, maxForce);

                lineRenderer.enabled = false;
                clickHoldStarted = false;

                // Allows the player to cancel shooting with letting mouse button go while cursor is on the ball.
                if (force.magnitude < 0.2)
                {
                    return;
                }

                rb.AddForce(force, ForceMode.Impulse);

                GameManager.Instance.playRandomSFXClip(whooshSoundClips, transform, force.magnitude / maxForce / 2);
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

    //On ball collision with a wall or an obstacle
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Obstacle"))
        {
            Vector3 normal = collision.contacts[0].normal;
            Vector3 vel = rb.velocity;
            if (collision.collider.CompareTag("Wall"))
            {
                GameManager.Instance.playRandomSFXClip(wallHitSoundClips, transform, vel.magnitude);
            }
            else
            {
                GameManager.Instance.playRandomSFXClip(obstacleHitSoundClips, transform, vel.magnitude);
            }
            rb.velocity = Vector3.Reflect(vel, normal);
        }
    }

    // Called when ball enters the hole
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hole"))
        {
            GameManager.Instance.playRandomSFXClip(ballInHoleSoundClips, transform, 0.2f);
            GameManager.Instance.playAllSFXClip(confettiSoundClips, transform, 1f);
            GameManager.Instance.setHoleUIActive(true);
            isInputActive = false;
            confettiParticles.Play();
        }
        // If ball goes outside the tracks
        if (other.CompareTag("Out"))
        {
            isInputActive = false;
            StartCoroutine(restartBall());
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

    public void InverseForce()
    {
        forceMultiplier *= -1;
    }
}
