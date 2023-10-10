using UnityEngine;

public class CampaignData : MonoBehaviour
{
    [HideInInspector] public string stageRace, stageLevel;
    public string stageName;
    public int stageID;
    public StageSystem ss;

    [Header("UI")]
    public UIToggle toggle;
    public UISprite stageSprite;
    public UISprite bonusSprite;
    public UILabel bonusLabel;
    public CampaignData[] nexts;
    public UISprite[] routes;

    void Awake()
    {
        if (stageID != 1)
        {
            PlayerPrefs.SetInt("Kalas_Check_" + stageID, 0);
            PlayerPrefs.SetInt("ShadowFang_Check_" + stageID, 0);
            PlayerPrefs.SetInt("Aridrian_Check_" + stageID, 0);
            PlayerPrefs.SetInt("Harbinger_Check_" + stageID, 0);
        }
        else
        {
            PlayerPrefs.SetInt("Kalas_Check_" + stageID, 1);
            PlayerPrefs.SetInt("ShadowFang_Check_" + stageID, 1);
            PlayerPrefs.SetInt("Aridrian_Check_" + stageID, 1);
            PlayerPrefs.SetInt("Harbinger_Check_" + stageID, 1);
        }    
    }

    public void StageCheck(string race)
    {
        stageRace = race;
        StageDataBase.Instance.StageDataParsing(stageRace, stageID, this);

        if (PlayerPrefs.GetInt(stageRace + "_Stage_" + stageID) == 1)
        {
            stageSprite.spriteName = "Stage_Player";
            toggle.enabled = true;

            for (int i = 0; i < nexts.Length; i++)
            {
                if (PlayerPrefs.GetInt(stageRace + "_Stage_" + nexts[i].stageID, 0) == 0)
                    PlayerPrefs.SetInt(stageRace + "_Check_" + nexts[i].stageID, 1);
            }

            for (int i = 0; i < routes.Length; i++)
                routes[i].spriteName = "Stage_Route_Unlock";
        }
        else
        {
            if (PlayerPrefs.GetInt(stageRace + "_Check_" + stageID) == 1)
            {
                stageSprite.spriteName = "Stage_Enemy";
                toggle.enabled = true;
            }
            else
            {
                stageSprite.spriteName = "Stage_Hunt";
                toggle.enabled = false;
            }

            for (int i = 0; i < routes.Length; i++)
                routes[i].spriteName = "Stage_Route_Lock";
        }
    }

    public void StageSelect()
    {
        if (toggle.value)
        {
            PlayerPrefs.SetInt("StageID", stageID);
            PlayerPrefs.SetString("Operation", stageName);
            PlayerPrefs.SetString("Difficulty", stageLevel);
            StageDataBase.Instance.MissionRewardDataParsing(stageLevel + "_Mission_Reward");
        }
    }
}
