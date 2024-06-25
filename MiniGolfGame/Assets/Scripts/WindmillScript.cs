using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  A class controlling animation of windmill obstacles.
 */
public class WindmillScript : MonoBehaviour
{
    /**
    * A private rigid body object referencing rigid body of the windmill
    */
    Rigidbody rb;

    /**
    * A public Vector3 object for setting angle of the rotation
    */
    public Vector3 m_EulerAngleVelocity;

    /**
    * A member function called at the start of the scene.
    */
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_EulerAngleVelocity = new Vector3(0, 50, 0);
    }

    /**
    * A member function called every frame.
    */
    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
