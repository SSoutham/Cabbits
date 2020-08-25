using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsPane : MonoBehaviour
{
    [SerializeField] Canvas achievementPane;
    [SerializeField] GameObject die1;
    [SerializeField] GameObject die10;
    [SerializeField] GameObject die30;
    [SerializeField] GameObject die50;

    private bool isActive = false;


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
