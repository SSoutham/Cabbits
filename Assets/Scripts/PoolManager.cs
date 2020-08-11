using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolDefinition
    {
        public string id;
        public ObjectPool pool;
    }

    public List<PoolDefinition> definitions = null;

    public Dictionary<string, ObjectPool> pools { get; set; } = new Dictionary<string, ObjectPool>();

    void Start ()
	{
        foreach (var definition in definitions)
        {
            pools[definition.id] = definition.pool;
        }
	}
}
