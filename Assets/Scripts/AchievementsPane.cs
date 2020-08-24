using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsPane : MonoBehaviour
{
    [SerializeField] Canvas achievementPane;

    private bool isActive = false;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isActive == false)
            {
                achievementPane.enabled = true;
                isActive = true;
            }
            else
            {
                achievementPane.enabled = false;
                isActive = false;
            }
        }
    }
}
