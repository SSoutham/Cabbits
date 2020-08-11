using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
public class DisappearingAsh : GrowingBush
{
    public override void GrowEnd()
    {
        gen.DisappearAsh(x, y);
        Destroy(gameObject);
    }
}
