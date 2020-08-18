using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathCountScript : MonoBehaviour
{
    private int deathCount = 0;
    public Text deathCountText;
    bool isActivated = false;

    void Start()
    {
        deathCount = PlayerPrefs.GetInt("DeathCount");
        deathCountText.text = "DeathCount: " + PlayerPrefs.GetInt("DeathCount").ToString();
    }

    public void Activate()
    {
        isActivated = true;
    }

    public void Deactivate()
    {
        isActivated = false;
        int currentDeathCount = PlayerPrefs.GetInt("DeathCount");
        if (deathCount > currentDeathCount)
            PlayerPrefs.SetInt("DeathCount", deathCount);
    }

    private void ResetDeathCount()
    {
        if(Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.E))
        {
            PlayerPrefs.SetInt("DeathCount", 0);
        }
    }

    public void AddDeath()
    {
        if (!isActivated) return;
        deathCount++;
    }
}
