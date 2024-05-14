using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilitiesScript : MonoBehaviour
{
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
