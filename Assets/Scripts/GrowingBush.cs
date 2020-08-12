using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Author: Iaroslav Titov (c)
public class GrowingBush : MonoBehaviour 
{
    public int x;
    public int y;
    public MapGenerator gen;
    public UnityEvent action;

    public void Initialize(int xN, int yN, MapGenerator genN)
    {
        x = xN;
        y = yN;
        gen = genN;
    }

    public virtual void GrowEnd()
    {
        //action.Invoke(x, y);
        //gen.GrowBush(x, y);
        Destroy(gameObject);
    }
}
