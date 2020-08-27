using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathCountScript : MonoBehaviour
{
    private int deathCount;
    private int deathBySquash;
    private int deathByDrowning;
    private int deathByFire;
    private int deathByLightning;
    private int deathByIllness;
    private int deathByStarvation;
    public Text deathCountText;
    bool isActivated = false;


    /* Updates the deathCount to equal the saved amount of deaths
     then replaces the canvas text to have the deathcount displayed
    man if only I could have been able to do the instantiate in 1 line*/
    void Start()
    {
        deathCount = PlayerPrefs.GetInt("DeathCount");
        deathBySquash = PlayerPrefs.GetInt("SquashCount");
        deathByDrowning = PlayerPrefs.GetInt("DrownCount");
        deathByFire = PlayerPrefs.GetInt("BurnCount");
        deathByLightning = PlayerPrefs.GetInt("LightningCount");
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
        {
            PlayerPrefs.SetInt("DeathCount", deathCount);
        }

        int currentSquashCount = PlayerPrefs.GetInt("SquashCount");
        if (deathBySquash > currentSquashCount)
        {
            PlayerPrefs.SetInt("SquashCount", deathBySquash);
        }

        int currentDrownCount = PlayerPrefs.GetInt("DrownCount");
        if (deathByDrowning > currentDrownCount)
        {
            PlayerPrefs.SetInt("DrownCount", deathByDrowning);
        }

        int currentBurnCount = PlayerPrefs.GetInt("BurnCount");
        if (deathByFire > currentBurnCount)
        {
            PlayerPrefs.SetInt("BurnCount", deathByFire);
        }

        int currentLightningCount = PlayerPrefs.GetInt("LightningCount");
        if (deathByLightning > currentLightningCount)
        {
            PlayerPrefs.SetInt("LightningCount", deathByLightning);
        }

    }

    private void ResetDeathCount()
    {
        if(Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.E))
        {
            PlayerPrefs.SetInt("DeathCount", 0);
        }
    }

    public void AddDeath(DeathReason reason)
    {
        deathCount++;

        if (reason == DeathReason.SMASH)
        {
            deathBySquash++;
        }
        else if (reason == DeathReason.DROWN)
        {
            deathByDrowning++;
        }
        else if (reason == DeathReason.BURN)
        {
            deathByFire++;
        }
        else if( reason == DeathReason.ELECTRICITY)
        {
            deathByLightning++;
        }
    }
}
