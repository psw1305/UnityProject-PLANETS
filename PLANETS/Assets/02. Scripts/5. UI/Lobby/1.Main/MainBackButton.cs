using UnityEngine;

public class MainBackButton : MonoBehaviour
{
    public MainCategory main;
    public enum Category { Building, Manage, Industry, Research, Military, Campaign };
    public Category category;
    [HideInInspector] public int currentNum = 0;

	void Update()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentNum == 0)
            {
                switch (category)
                {
                    case Category.Building:
                        main.OnBuilding();
                        break;
                    case Category.Manage:
                        main.OnManage();
                        break;
                    case Category.Military:
                        main.OnMilitary();
                        break;
                    case Category.Research:
                        main.OnResearch();
                        break;
                    case Category.Industry:
                        main.OnIndustry();
                        break;
                    case Category.Campaign:
                        main.OnCampaign();
                        break;
                }
            }
        }
        #endif
    }

    public void MainByBuildingButton()
    {
        if (main.mains[0].activeSelf == true)
        {
            currentNum = 0;
            main.OnBuilding();
        }
    }

    public void MainByManageButton()
    {
        if (main.mains[1].activeSelf == true)
        {
            currentNum = 0;
            main.OnManage();
        }
    }

    public void MainByIndustryButton()
    {
        if (main.mains[2].activeSelf == true)
        {
            currentNum = 0;
            main.OnIndustry();
        }
    }

    public void MainByMilitaryButton()
    {
        if (main.mains[3].activeSelf == true)
        {
            currentNum = 0;
            main.OnMilitary();
        }
    }

    public void MainByResearchButton()
    {
        if (main.mains[4].activeSelf == true)
        {
            currentNum = 0;
            main.OnResearch();
        }
    }

    public void MainByCampaignButton()
    {
        if (main.mains[5].activeSelf == true)
        {
            currentNum = 0;
            main.OnCampaign();
        }
    }
}