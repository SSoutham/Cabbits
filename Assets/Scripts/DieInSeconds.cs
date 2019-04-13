using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieInSeconds : MonoBehaviour
{
    public float seconds;

    void Start()
    {
        Destroy(gameObject, seconds);
    }
}
