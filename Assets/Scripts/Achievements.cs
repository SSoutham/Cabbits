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
    /* For some God Awful reason, this list keeps resetting unless its static
     * too bad! */
    public static List<AchievementEnums> AchievementsAchieved = new List<AchievementEnums>();

    public void checkDeathAchievements()
    {
        int deathCount = PlayerPrefs.GetInt("DeathCount");

        /* Just Checks if the current Death is higher than a greater Number
         * This is honestly a terrible way of doing it. */
        if(deathCount >= 1)
        {
            if (!AchievementsAchieved.Contains(AchievementEnums.DIE))
            {
                AchievementsAchieved.Add(AchievementEnums.DIE);
                Debug.Log(AchievementsAchieved.Count);
            }
        }
        if (deathCount >= 10)
        {
            if (!AchievementsAchieved.Contains(AchievementEnums.DIETENTIMES))
            {
                AchievementsAchieved.Add(AchievementEnums.DIETENTIMES);
            }
        }
        if (deathCount >= 30)
        {
            if (!AchievementsAchieved.Contains(AchievementEnums.DIETHIRTYTIMES))
            {
                AchievementsAchieved.Add(AchievementEnums.DIETHIRTYTIMES);
            }
        }
        if (deathCount >= 50)
        {
            if (!AchievementsAchieved.Contains(AchievementEnums.DIEFIFTYTIMES))
            {
                AchievementsAchieved.Add(AchievementEnums.DIEFIFTYTIMES);
            }
        }

    }

}
