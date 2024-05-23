using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ArrowScript : MonoBehaviour
{
    //public BallScript ballScript;
    public Vector3 firstMousePos;
    public Vector3 lastMousePos;
    public float maxArrowLength = 200;
    public Vector2 arrowVector;
    public GameObject ball;
    public int num = 30;

	private bool buttonDown = false;
    private Vector3 ballPos;

    void Start()
    {
		GetComponent<Renderer>().enabled = false;
	}


    void Update()
    {
        /*When mouse button CLICKED*/
        if (Input.GetButtonDown("Fire1"))
        {
			transform.position = ball.transform.position;
            buttonDown = true;
            firstMousePos = Input.mousePosition;
			GetComponent<Renderer>().enabled = true;
		}

        /*On mouse button HOLD*/
        if (buttonDown)
        {
            lastMousePos = Input.mousePosition;
            arrowVector = new Vector2(firstMousePos.x - lastMousePos.x, firstMousePos.y - lastMousePos.y);
            arrowVector = new Vector2(firstMousePos.x - lastMousePos.x, firstMousePos.y - lastMousePos.y);

            //Setting position and rotation of the arrow depending on arrow vector
            setArrowPosAndRot(arrowVector);

            /*When mouse button RELEASED*/
            if (Input.GetButtonUp("Fire1"))
            {
                buttonDown = false;
				GetComponent<Renderer>().enabled = false;
			}
        }

    }


    //Set beginning of the arrow at the center of the ball
    private void setArrowPosAndRot(Vector2 vec)
    {
		//Scaling the arrow
		transform.localScale = new Vector3(0.1f, boundHypotenuse(vec, maxArrowLength) / num, 0.1f);

		//Find the appropriate angle and rotate the arrow
		float vectorAngle = getVectorAngle(vec);
		transform.eulerAngles = new Vector3(-90, 0, vectorAngle);

		//Find half of the length of the arrow (with taking the scaling into account)
		float halfArrowLength = GetComponent<Renderer>().localBounds.size.y * transform.localScale.y / 2;

		//Convert the angle to radians and use it with the half-arrow-length to calculate offset of the arrow
		float angleRad = vectorAngle * (Mathf.PI / 180);
		Vector3 posOffset = new Vector3(Mathf.Cos(angleRad) * halfArrowLength, 0, Mathf.Sin(angleRad) * halfArrowLength);

		//Position the arrow at the ball position and reposition the arrow by the offset
		ballPos = ball.transform.position;
		transform.position = new Vector3(ballPos.x - posOffset.z, ballPos.y, ballPos.z - posOffset.x);
	}


    //Get the angle of the vector
    private float getVectorAngle(Vector2 vec)
    {
        float tg = vec.y / vec.x;
        float angleRad = Mathf.Atan(tg);

        //if vec.y and vec.x were both 0, it results in a Not a Number value (NaN), so we just make the angle 0
        if(float.IsNaN(angleRad))
        {
            angleRad = 0;
        }
        float angleDeg = angleRad * (180 / Mathf.PI);

        //Adjusting the angle cuz the arctan made it weird basically
        angleDeg = angleDeg + 90;
        if (vec.x > 0)
        {
            angleDeg = -angleDeg;
            angleDeg = angleDeg - (90 + angleDeg) * 2;
        }

        //Adding 180 degrees cuz at the moment it was in the direction of the vector, and combining it with subtraction from 360,
        //cuz it was going from 0 to 360 clockwise instead of counterclockwise + adding it to camera y rotation to follow camera movement
        return Camera.main.transform.localEulerAngles.y + 180 - angleDeg;
    }




    //Find hypotenuse, limit it so that the arrow has it's max length and then return it
    public float boundHypotenuse(Vector2 vec, float maxLength)
    {
        float hyp = hypotenuse(vec.x, vec.y);
        if (hyp > maxLength)
        {
            return maxLength;
        }
        return hyp;
    }

    public float hypotenuse(float x, float y)
    {

        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
    }



}
