using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
public class ThreatSpawner : MonoBehaviour 
{
    [SerializeField] List<GameObject> threats = new List<GameObject>();
    [SerializeField] MapGenerator map;
    float timer = 4;
    float interval = 2;
    float mininterval = 0.25f;
    float intervalDecay = -0.01f;

    void Update () 
	{
        if (!map.isGameStarted()) return;

        timer -= Time.deltaTime;
        if (interval > mininterval)
            interval += intervalDecay * Time.deltaTime;

        if (timer <= 0)
        {
            timer = interval + (Random.Range(1, 5) * .1f);

            GameObject threat = Instantiate(threats[Random.Range(0, threats.Count)], new Vector2(Random.Range(0, map.worldWidth), Random.Range(0, map.worldWidth)), Quaternion.identity);
            threat.GetComponent<ThreatScript>().map = map;
        }
	}
}
