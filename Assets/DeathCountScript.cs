using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathCountScript : MonoBehaviour
{
    private int deathCount;
    public Text deathCountText;
    bool isActivated = false;


    /* Updates the deathCount to equal the saved amount of deaths
     then replaces the canvas text to have the deathcount displayed
    man if only I could have been able to do the instantiate in 1 line*/
    void Start()
    {
        deathCount = PlayerPrefs.GetInt("DeathCount");
        deathCountText.text = "DeathCount: " + PlayerPrefs.GetInt("DeathCount").ToString();
    }

    public void Activate()
    {
        isActivated = true;
    }

    /* This is where it does a check if the current deathcount is larger than the
     * deathcount that has been saved. its the same way for highscore, but I think
     this can be placed at better timing (Ya know like when the Cabbit dies), FML*/
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
        deathCount++;
    }
}
