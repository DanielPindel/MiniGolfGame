using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

/**
 *  A class controlling help animation on the first level of the game
 */
public class CursorDrag : MonoBehaviour
{
    /**
    * A public Texture object for referencing cursor texture for the animation
    */
    public Texture cursorTexture;

    /**
    * A public Texture object for referencing cursor click texture for the animation
    */
    public Texture cursorClickTexture;

    /**
    * A public float to set the drag amount of the animation
    */
    public float drag;

    /**
    * A public float to decide on the length of the animation
    */
    public float counter;

    /**
    * A public float for the max distance for the cursor to move during the animation
    */
    public float maxDistance;

    /**
    * A public float for the fade distance for the cursor to be moving during the fade animation
    */
    public float fadeDistance;

    /**
    * A public float for the fade amount for the cursor to be fading during the animation
    */
    public float fade;

    /**
    * A public float for the wait time of each section of the animation
    */
    public float waitTime;

    /**
    * A public RawImage object for referencing the cursor icon
    */
    public RawImage icon;

    /**
    * A public Game Object for refencing the object storing the animation components in the scene
    */
    public GameObject helpAnimation;

    /**
    * A private bool variable for freezing the animation (stopping it between animation sections)
    */
    private bool freeze = false;

    /**
    * A private Vector3 object for storing starting position of the cursor in the animation
    */
    Vector3 startPos = new Vector3();

    /**
    * A member function called at the start of the scene.
    */
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

    /**
    * A member function called every frame.
    */
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

    /**
     * A private coroutine for freezing the animation
     */
    IEnumerator wait(float waitTime)
    {
        freeze = true;
        yield return new WaitForSecondsRealtime(waitTime);
        freeze = false;
    }

    /**
     * A private coroutine for freezing the animation and changing the texture to cursor click
     */
    IEnumerator waitClick(float waitTime)
    {
        freeze = true;
        yield return new WaitForSecondsRealtime(waitTime);
        icon.texture = cursorClickTexture;
        freeze = false;
    }

    /**
     * A private coroutine for freezing the animation and changing the texture to cursor
     */
    IEnumerator unclickWait(float waitTime)
    {
        freeze = true;
        icon.texture = cursorTexture;
        yield return new WaitForSecondsRealtime(waitTime);
        freeze = false;
    }


}
