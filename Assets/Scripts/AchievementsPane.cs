using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementsPane : MonoBehaviour
{
    [SerializeField] Canvas achievementPane;
    [SerializeField] GameObject die1;
    [SerializeField] GameObject die10;
    [SerializeField] GameObject die30;
    [SerializeField] GameObject die50;

    private int deathBySquash;
    private int deathByDrowning;
    private int deathByFire;
    private int deathByLightning;
    public TextMeshProUGUI SquashText;
    public TextMeshProUGUI DrownText;
    public TextMeshProUGUI FireText;
    public TextMeshProUGUI LightningText;

    private bool isActive = false;

    private void Start()
    {
        SquashText.text = PlayerPrefs.GetInt("SquashCount").ToString();
        DrownText.text = PlayerPrefs.GetInt("DrownCount").ToString();
        FireText.text = PlayerPrefs.GetInt("BurnCount").ToString();
        LightningText.text = PlayerPrefs.GetInt("LightningCount").ToString();
    }


    // Update is called once per frame
    void Update()
    {
        /* User Presses Escape, Achievement Pane pops up,
         * simple as that. */
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isActive == false)
            {
                achievementPane.enabled = true;
                isActive = true;
                CheckAchievements(Achievements.AchievementsAchieved);

            }
            else
            {
                achievementPane.enabled = false;
                isActive = false;
            }
        }
    }

    void CheckAchievements(List<AchievementEnums> achievements)
    {
        if(achievements.Count != 0)
        {
            for(int i = 0; i < achievements.Count; i++)
            {
                if(achievements[i] == AchievementEnums.DIE)
                {
                    die1.SetActive(true);
                }
                else if(achievements[i] == AchievementEnums.DIETENTIMES)
                {
                    die10.SetActive(true);
                }
                else if(achievements[i] == AchievementEnums.DIETHIRTYTIMES)
                {
                    die30.SetActive(true);
                }
                else
                {
                    die50.SetActive(true);
                }
            }
        }
    }
}
