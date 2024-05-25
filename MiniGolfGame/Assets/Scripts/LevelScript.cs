using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelScript : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public float mouseSensitivity = 5f;
    public float smoothingFactor = 2f;
    public float decelerationFactor = 0.95f;

    private float currentHorizontal;
    private float currentVertical;
    private float horizontalVelocity;
    private float verticalVelocity;

    void Start()
    {
        freeLookCamera.m_XAxis.m_InputAxisName = "";
        freeLookCamera.m_YAxis.m_InputAxisName = "";
    }
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float targetHorizontal = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 100f;
            float targetVertical = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            currentHorizontal = Mathf.SmoothDamp(currentHorizontal, targetHorizontal, ref horizontalVelocity, smoothingFactor * Time.deltaTime * 10f);
            currentVertical = Mathf.SmoothDamp(currentVertical, targetVertical, ref verticalVelocity, smoothingFactor * Time.deltaTime);
        }
        else
        {
            currentHorizontal *= decelerationFactor;
            currentVertical *= decelerationFactor;

            if(Mathf.Abs(currentHorizontal) < 0.01f)
            {
                currentHorizontal = 0f;
            }
            if(Mathf.Abs(currentVertical) < 0.01f)
            {
                currentVertical = 0f;
            }
        }
        freeLookCamera.m_XAxis.Value += currentHorizontal;
        freeLookCamera.m_YAxis.Value -= currentVertical;
    }
}
