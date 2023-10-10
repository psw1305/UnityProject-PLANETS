using UnityEngine;

public class StageInformation : MonoBehaviour
{
    [HideInInspector] public StageSystem system;
    public string race;
    public UILabel stageClear;
    int clearNum = 0;

    void Start()
    {
        for (int i = 0; i < 15; i++)
        {
            if (PlayerPrefs.GetInt(race + "_Stage_" + i, 0) == 1)
                clearNum += 1;
        }

        stageClear.text = clearNum.ToString();
    }

    public void StageSetting()
    {
        PlayerPrefs.SetString("StartRace", race);

        system.race = race;
        system.CampaignSelect();
    }
}
