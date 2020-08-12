using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
public class GrowingGrass : GrowingBush 
{
    public void Initialize(int xN, int yN, MapGenerator genN, Cell left, Cell right, Cell top, Cell bot)
    {
        Initialize(xN, yN, genN);

        if ((left == Cell.GRASS || left == Cell.GROWING_GRASS) &&
            (right == Cell.GRASS || right == Cell.GROWING_GRASS) &&
            (top == Cell.GRASS || top == Cell.GROWING_GRASS) &&
            (bot == Cell.GRASS || bot == Cell.GROWING_GRASS))
        {
            GetComponent<Animator>().CrossFade("GrowingGrass", 0.1f);
        }
    }

    public override void GrowEnd()
    {
        //gen.GrowGrass(x, y);
        Destroy(gameObject);
    }
}
