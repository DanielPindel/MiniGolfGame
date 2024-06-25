using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  A cards controller class. It controls the power-up and hindrances cards in the game.
 */
public class WindmillScript : MonoBehaviour
{

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
