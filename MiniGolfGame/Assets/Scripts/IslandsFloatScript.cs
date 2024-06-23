using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandsFloatScript : MonoBehaviour
{
    public float moveBy = 0.005f;
    //public Vector3 direction = Vector3.right;
    public Vector3 endPos;
    Vector3 startPos;
    int direction = 1;
    bool changedDir;
    bool isStart;

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

        transform.localPosition += moveBy * new Vector3(0, direction, 0);
    }
}
