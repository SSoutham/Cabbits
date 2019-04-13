using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Iaroslav Titov (c)
public class LightningScript : ThreatScript 
{
	void Start ()
	{
        StartCoroutine(Life());
	}
    
    private IEnumerator Life()
    {
        yield return new WaitForSeconds(1.2f);

        map.SetOnFire(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, .1f, LayerMask.GetMask("Carriable"));
        foreach (Collider2D c in hits)
        {
            CarryScript carry = c.GetComponent<CarryScript>();
            if (carry)
            {
                carry.Die(DeathReason.ELECTRICITY);
            }
        }

        Destroy(gameObject);
    }
}

// Base class
public class ThreatScript : MonoBehaviour
{
    public MapGenerator map;
}
