using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSScript : MonoBehaviour
{
    public Text text;
    public float frequency = 0.5f;
    private float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > frequency)
        {
            timer = 0;
            text.text = "FPS: " + Mathf.Floor(1.0f / Time.deltaTime);
        }
    }
}
