using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class CursorDrag : MonoBehaviour
{
    public Texture cursorTexture;
    public Texture cursorClickTexture;

    public float drag;
    public float counter;
    public float maxDistance;
    public float fadeDistance;
    public float fade;
    public float waitTime;
    public RawImage icon;
    public GameObject helpAnimation;
    private bool freeze = false;
    
    Vector3 startPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        icon = GetComponent<RawImage>();
        helpAnimation = GameObject.Find("HelpAnimation");
        drag = 1.5f;
        counter = 0;
        maxDistance = 50;
        fadeDistance = 30;
        fade = 0.2f;
        icon.texture = cursorTexture;
        waitTime = 0.7f;
        helpAnimation.SetActive(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!helpAnimation.activeSelf)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            helpAnimation.SetActive(false);
        }

        if (freeze) { return; }


        if (counter == 0)
        {
            StartCoroutine(waitClick(waitTime));
            counter++;
        }
        if(counter < maxDistance)
        {
            transform.position = transform.position + new Vector3(drag, -drag, 0);
            counter++;
        }
        else
        {
            if (counter < maxDistance + fadeDistance) 
            {
                if(icon.texture == cursorClickTexture)
                {
                    StartCoroutine(unclickWait(waitTime));
                    return;
                }
                var color = icon.color;
                color.a = color.a - fade;
                icon.color = color;
                counter++;
            }
            else
            {
                transform.position = startPos;
                counter = 0;
                var color = icon.color;
                color.a = 1;
                icon.color = color;
            }
        }
    }


    IEnumerator wait(float waitTime)
    {
        freeze = true;
        yield return new WaitForSecondsRealtime(waitTime);
        freeze = false;
    }

    IEnumerator waitClick(float waitTime)
    {
        freeze = true;
        yield return new WaitForSecondsRealtime(waitTime);
        icon.texture = cursorClickTexture;
        freeze = false;
    }

    IEnumerator unclickWait(float waitTime)
    {
        freeze = true;
        icon.texture = cursorTexture;
        yield return new WaitForSecondsRealtime(waitTime);
        freeze = false;
    }


}
