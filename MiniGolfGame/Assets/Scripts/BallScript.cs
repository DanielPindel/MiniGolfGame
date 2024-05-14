using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    public Rigidbody myRigidBody;
    public Vector3 firstMousePos;
    public Vector3 lastMousePos;
    public float vectorBound = 50;
    public float strength = 5;
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
                lastMousePos = Input.mousePosition;

				
				ballDirection = new Vector3(firstMousePos.x - lastMousePos.x, 0, firstMousePos.y - lastMousePos.y);
                bindVector(ref ballDirection, vectorBound);
                myRigidBody.velocity = ballDirection.normalized * strength;

                //
                myRigidBody.AddForce(ballDirection.normalized * strength);


                //
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

    private void bindVector(ref Vector3 vec, float vectorBound)
    {
        if (vec.x > vectorBound)
        {
            vec.z = vectorBound * vec.z / vec.x;
            vec.x = vectorBound;
        }
        if (vec.x < -vectorBound)
        {
            vec.z = -vectorBound * vec.z / vec.x;
            vec.x = -vectorBound;
        }
        if (vec.y > vectorBound)
        {
            vec.x = vectorBound * vec.x / vec.z;
            vec.z = vectorBound;
        }
        if (vec.y < -vectorBound)
        {
            vec.x = -vectorBound * vec.x / vec.z;
            vec.z = -vectorBound;
        }
    }

    public bool getBallHover()
    {
        return ballHover;
    }

}
