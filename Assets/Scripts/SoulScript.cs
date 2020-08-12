using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
public class SoulScript : MonoBehaviour 
{
    [SerializeField] float speed;

	void Start ()
	{
        GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
        Destroy(gameObject, 10.0f);
	}
}
