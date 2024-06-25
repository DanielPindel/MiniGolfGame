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
    public Vector3 m_EulerAngleVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_EulerAngleVelocity = new Vector3(0, 50, 0);
    }

    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
