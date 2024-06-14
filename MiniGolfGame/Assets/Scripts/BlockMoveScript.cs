using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BlockMoveScript : MonoBehaviour
{
    public float moveBy = 0.005f;
    public Vector3 direction = Vector3.right;
    public Vector3 endPos;
    Vector3 startPos;
    bool changedDir;
    bool isStart;

    void Start()
    {
        changedDir = false;
        isStart = true;
        startPos = transform.localPosition;
    }

    void FixedUpdate()
    {
        Vector3 localPos = transform.localPosition;

        // If it's close to the start or the end AND direction wasn't just changed
        if ((Vector3.Distance(localPos, endPos) < moveBy * 2 || Vector3.Distance(localPos, startPos) < moveBy * 2) & !changedDir)
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

        transform.localPosition += moveBy * direction;
    }

}
