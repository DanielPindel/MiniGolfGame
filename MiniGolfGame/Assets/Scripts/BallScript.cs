using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    public Rigidbody myRigidBody;
    public Vector3 firstMousePos;
    public Vector3 lastMousePos;
    public float vectorBound = 50;
    public float shootForce = 5;
    public Vector3 ballDirection;
    public ConstantForce gravity;

    private bool ballHover;
    private bool ballWasClicked;

    void Start()
    {
        ballHover = false;

        //Some gravity stuff, idk
        /*Physics.queriesHitTriggers = true;
        gravity = gameObject.AddComponent<ConstantForce>();
        gravity.force = new Vector3(0.0f, -10, 0.0f);
        gameObject.GetComponent<Rigidbody>().useGravity = false;*/
    }

    void Update()
    {

        Vector3? worldPoint = CastMouseClickRay();

        if (!worldPoint.HasValue)
        {
            return;
        }


        /*When mouse button CLICKED*/
        if (Input.GetButtonDown("Fire1"))
        {
            //If mouse is on the ball
            if (ballHover)
            {
                firstMousePos = Input.mousePosition;
                ballWasClicked = true;
            }
        }

        /*When mouse button RELEASED*/
        if (Input.GetButtonUp("Fire1"))
        {
            //If mouse was earlier clicked on the ball
            if (ballWasClicked)
            {

				Vector3 horizontalWorldPoint = new Vector3(worldPoint.Value.x, transform.position.y, worldPoint.Value.z);

                Vector3 direction = -(horizontalWorldPoint - transform.position).normalized;

                float strength = Vector3.Distance(transform.position, horizontalWorldPoint);
                myRigidBody.AddForce(direction.normalized * strength * shootForce);


                ballWasClicked = false;
            }

        }
    }

    void OnMouseOver()
    {
        ballHover = true;
    }
    void OnMouseExit()
    {
        ballHover = false;
    }

    public bool getBallHover()
    {
        return ballHover;
    }

    private Vector3? CastMouseClickRay()
    {
        Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        RaycastHit hit;
        if (Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, float.PositiveInfinity))
        {
            return hit.point;
        }
        else
        {
            return null;
        }
    }

}
