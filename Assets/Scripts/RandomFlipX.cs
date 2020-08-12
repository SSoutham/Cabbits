using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
[RequireComponent(typeof(SpriteRenderer))]
public class RandomFlipX : MonoBehaviour 
{
	void Start ()
	{
        GetComponent<SpriteRenderer>().flipX = Random.value > 0.5f;
	}
}
