using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsFloatScript : MonoBehaviour
{
    /**
    * A public float variable for the amount of movement per update
    */
    public float moveBy = 0.005f;

    /**
    * A public Vector3 for storing the end position in the animation
    */
    public Vector3 endPos;

    /**
    * A private Vector3 for storing the starting position in the animation
    */
    Vector3 startPos;

    /**
    * A private int describing direction of the animation
    */
    int direction = 1;

    /**
    * A private bool signifyig the change of direction of the animation
    */
    bool changedDir;

    /**
    * A private bool signaling start of the animation
    */
    bool isStart;

    /**
    * A member function called at the start of the scene.
    */
    void Start()
    {
        changedDir = false;
        isStart = true;

        var random = new System.Random();
        var list = new List<int> { -1, 1 };
        int index = random.Next(list.Count);
        direction = list[index];

        startPos = transform.localPosition;
        endPos = new Vector3(startPos.x, startPos.y + UnityEngine.Random.Range(0.5f, 1f) * direction, startPos.z);
    }

    /**
    * A member function called every frame.
    */
    void FixedUpdate()
    {
        Vector3 localPos = transform.localPosition;

        float distanceToStart = Vector3.Distance(localPos, startPos);
        float distanceToEnd = Vector3.Distance(localPos, endPos);

        float totalDistance = Vector3.Distance(startPos, endPos);
        float normalizedDistance = Mathf.Min(distanceToStart, distanceToEnd) / totalDistance;

        float adjustedSpeed = moveBy * Mathf.Sin(normalizedDistance * Mathf.PI);
        adjustedSpeed = Mathf.Max(adjustedSpeed, moveBy * 0.1f);

        // If it's close to the start or the end AND direction wasn't just changed
        if ((distanceToEnd < moveBy * 2 || distanceToStart < moveBy * 2) & !changedDir)
        {
            // If it's not a start of the level
            if (!isStart)
            {
                direction *= -1;
                // Set direction as just changed
                changedDir = true;
            }
        }
        // If it's not close to the start or the end
        else
        {
            // Set direction as not changed anymore
            changedDir = false;
            if (isStart) { isStart = false; }
        }

        transform.localPosition += adjustedSpeed * new Vector3(0, direction, 0);
    }
}
