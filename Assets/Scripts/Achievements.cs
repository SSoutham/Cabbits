using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* An Enum of Achievements possible to get, I honestly just thought this
 * would be a cool way to do it.*/
public enum AchievementEnums
{
    DIE = 0,
    DIETENTIMES = 1,
    DIETHIRTYTIMES = 2,
    DIEFIFTYTIMES = 3,
}

public class Achievements : MonoBehaviour
{
    private List<AchievementEnums> AchievementsAchieved;


    void Start()
    {
        AchievementsAchieved = new List<AchievementEnums>();
    }

    void checkDeathAchievements()
    {
        int deathCount = PlayerPrefs.GetInt("DeathCount");

        /* Just Checks if the current Death is higher than a greater Number
         * This is honestly a terrible way of doing it. */
        if(deathCount >= 1)
        {
            AchievementsAchieved.Add(AchievementEnums.DIE);
        }
        if (deathCount >= 10)
        {
            AchievementsAchieved.Add(AchievementEnums.DIETENTIMES);
        }
        if (deathCount >= 30)
        {
            AchievementsAchieved.Add(AchievementEnums.DIETHIRTYTIMES);
        }
        if (deathCount >= 50)
        {
            AchievementsAchieved.Add(AchievementEnums.DIEFIFTYTIMES);
        }

    }

}
