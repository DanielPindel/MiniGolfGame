using System.Collections;
using UnityEngine;

/**
 *  A ball controller class. It controls the ball behavior and everything related to it such as ball sounds.
 */
public class BallController : MonoBehaviour
{
    /**
     * A public float for adjusting the ball force
     */
    public float forceMultiplier;

    /**
     * A public float for the force multiplier after choosing the boost card in the game
     */
    public float boostedForceMultiplier;

    /**
     * A public float for the maximal force for the ball
     */
    public float maxForce;

    /**
     * A public float for the minimal velocity of the ball
     */
    public float minVelocity;

    /**
     * A public float for the strength with which ball will be drawn to the hole after activating the Magnet card in the game
     */
    public float magnetStrength = 2f;

    /**
     * A public float for the spin speed of the ball
     */
    public float spinSpeed = 360f;

    /**
    * A public Particle System variable for referencing confetti particles.
    */
    public ParticleSystem confettiParticles;

    /**
    * A public Audio Clip array for storing wall hit sound effect clips used when the ball hits a wall.
    */
    public AudioClip[] wallHitSoundClips;

    /**
    * A public Audio Clip array for storing obstacle hit sound effect clips used when the ball hits an obstacle.
    */
    public AudioClip[] obstacleHitSoundClips;

    /**
    * A public Audio Clip array for storing whoosh sound effect clips used when player shoots the ball.
    */
    public AudioClip[] whooshSoundClips;

    /**
    * A public Audio Clip array for storing sound effect clips used when ball falls into the hole.
    */
    public AudioClip[] ballInHoleSoundClips;

    /**
    * A public Audio Clip array for storing confetti sound effect clips used confetti shoots out after the ball lands in the hole.
    */
    public AudioClip[] confettiSoundClips;

    /**
     * A public Transform object representing the hole position.
     */
    public Transform hole;

    /**
     * A private Vector3 representing position of the start of the mouse drag.
     */
    private Vector3 dragStartPos;

    /**
     * A private Rigidbody variable for referencing ball's rigid body component.
     */
    private Rigidbody rb;

    /**
     * A private line renderer variable for referencing ball's line renderer component.
     */
    private LineRenderer lineRenderer;

    /**
    * A private Vector3 variable for storing starting position of the ball at the start of the level.
    */
    private Vector3 ballStartingPos;

    /**
    * A public bool variable checking whether the mouse input for the ball is set to be active.
    */
    public bool isInputActive;

    /**
    * A private bool variable checking whether the mouse hold was started.
    */
    private bool clickHoldStarted;

    /**
    * A private bool variable checking whether the magnet power-up was activated
    */
    private bool isMagnetActive = false;

    /**
    * A method called when the script instance is being loaded
    */
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        lineRenderer = this.GetComponent<LineRenderer>();
        confettiParticles = GameObject.Find("ConfettiParticles").GetComponent<ParticleSystem>();
    }


    /**
    * A member function called at the start of the scene.
    */
    void Start()
    {
        forceMultiplier = 20f;
        boostedForceMultiplier = 40f;
        maxForce = 10f;
        minVelocity = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        ballStartingPos = transform.position;
        isInputActive = true;
    }

    /**
    * A member function called every frame.
    */
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

        if(isMagnetActive)
        {
            ApplyMagnetForce();
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

    /**
     * A private member function for getting mouse position on XZ place.
     * This function is used for getting mouse position on XZ place for mouse input in the game, it uses raycasting.
     */
    private Vector3 GetMousePositionOnXZPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xzPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        xzPlane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }


    /**
     * A private member function called on collision.
     * This function is used to detect collision of the ball with a wall or an obstacle, 
     * to then reflect the balls directory and play a sound.
     * @param collision the collision detected.
     */
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Obstacle"))
        {
            lineRenderer.enabled = false;

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

    /**
     * A private member function called on trigger.
     * This function is used to detect whether the ball entered the hole with a "Hole" tagged trigger 
     * or whether it went out of the tracks and triggered an "Out" tagged trigger.
     * @param other the trigger collider detected.
     */
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hole"))
        {
            GameManager.Instance.playRandomSFXClip(ballInHoleSoundClips, transform, 0.2f);
            GameManager.Instance.playAllSFXClip(confettiSoundClips, transform, 1f);
            GameManager.Instance.setHoleUIActive(true);
            isInputActive = false;
            confettiParticles.Play();
            GameManager.Instance.scoreboard.SetActive(true);
        }
        // If ball goes outside the tracks
        if (other.CompareTag("Out"))
        {
            isInputActive = false;
            StartCoroutine(restartBall());
        }
    }


    /**
     * A public member function that restarts the level.
     * This function is used to reset strokes, turn off the HoleUI (final menu) start the ball restart coroutine.
     */
    public void callRestartBall()
    {
        GameManager.Instance.setHoleUIActive(false);
        GameManager.Instance.resetStrokes();
        GameManager.Instance.ResetCardEffects();
        StartCoroutine(restartBall());
    }

    /**
     * A private coroutine for restarting the ball.
     * It freezes the ball for one second after it gets restarted to its original position.
     */
    IEnumerator restartBall()
    {
        transform.position = ballStartingPos;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(1);
        rb.constraints = RigidbodyConstraints.None;
        isInputActive = true;
    }

    /**
     * A public member function to raise the force multiplier.
     * This function is used for applying boost to the ball force.
     */
    public void RaiseForceMultiplier()
    {
        if(forceMultiplier < 0)
        {
            forceMultiplier = -boostedForceMultiplier;
        }
        else
        {
            forceMultiplier = boostedForceMultiplier;
        }
    }

    /**
     * A public member function to inverse the direction of the force.
     * This function is used for hindrance card that reverses the direction of the force applied to the ball.
     */
    public void InverseForce()
    {
        forceMultiplier *= -1;
    }

    /**
     * A public member function to apply force attracting ball to the hole.
     * This function is used for magnet power-up card.
     */
    public void ApplyMagnetForce()
    {
        if(hole == null)
        {
            return;
        }

        Vector3 directionToHole = (hole.position - transform.position).normalized;
        rb.AddForce(directionToHole * magnetStrength);
    }

    /**
     * A public member function to activate magnet power-up
     */
    public void ActivateMagnet()
    {
        isMagnetActive = true;
        StartCoroutine(DeactivateMagnet(5f));
    }

    /**
     * A private coroutine for applying the magnet power-up for a certain amount of time.
     */
    private IEnumerator DeactivateMagnet(float delay)
    {
        yield return new WaitForSeconds(delay);
        isMagnetActive = false;
    }
}
