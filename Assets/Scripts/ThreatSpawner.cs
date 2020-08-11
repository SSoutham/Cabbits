using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
public class ThreatSpawner : MonoBehaviour 
{
    [SerializeField] List<GameObject> threats = new List<GameObject>();
    [SerializeField] float timer = 20;
    [SerializeField] float interval = 10;
    [SerializeField] float intervalDecay = -0.01f;
    [SerializeField] MapGenerator map;

    void Update () 
	{
        if (!map.isGameStarted()) return;

        timer -= Time.deltaTime;
        if (interval > .1f)
            interval += intervalDecay * Time.deltaTime;

        if (timer <= 0)
        {
            timer = interval;

            GameObject threat = Instantiate(threats[Random.Range(0, threats.Count)], new Vector2(Random.Range(0, map.worldWidth), Random.Range(0, map.worldWidth)), Quaternion.identity);
            threat.GetComponent<ThreatScript>().map = map;
        }
	}
}
