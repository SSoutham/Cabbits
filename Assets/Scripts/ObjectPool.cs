using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
public class ObjectPool : MonoBehaviour 
{
    [SerializeField] GameObject prefab = null;
    [SerializeField] int size = 10;
    [SerializeField] bool grow = true;
    [SerializeField] float returnIn = 0;

    public bool keepTrack = false;
    private Dictionary<string, GameObject> track = new Dictionary<string, GameObject>();

    Queue<GameObject> pool = new Queue<GameObject>();
    int id = 1;

    void Start ()
	{
        Grow(size);
	}

    public GameObject Get(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (pool.Count == 0 && grow)
        {
            Grow(size);
            Debug.Log("I just grew, new size is " + pool.Count);
        }

        GameObject go = pool.Dequeue();
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.parent = parent;
        go.SetActive(true);

        if (returnIn > 0) StartCoroutine(ReturnInSeconds(go, returnIn));
        if (keepTrack) track[position.x + " " + position.y] = go;
        return go;
    }

    public void Return(Vector3Int pos)
    {
        string s = pos.x + " " + pos.y;
        if (track.ContainsKey(s)) Return(track[s]);
    }

    public void Return(GameObject go)
    {
        if (!go.activeSelf) return;
        pool.Enqueue(go);
        go.transform.parent = transform;
        go.SetActive(false);
    }

    public IEnumerator ReturnInSeconds(GameObject go, float wait)
    {
        yield return new WaitForSeconds(wait);

        Return(go);
    }

    void Grow(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject go = Instantiate(prefab, transform);
            go.name = prefab.name + id++;
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }
}
