using UnityEngine;

public class StageSystem : MonoBehaviour
{
    [HideInInspector] public string race;
    public GameObject stage;
    public GameObject stageInfo;
    public GameObject fleetManage;
    public GameObject select;

    [Header("Script")]
    public StageInformation[] si;
    public CampaignData[] cd;
    public EmbarkManager em;
    public MainBackButton mbb;

    void Start()
    {
        for (int i = 0; i < si.Length; i++)
            si[i].system = this;
    }

    void Update()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (em.isPopup && mbb.currentNum == 3)
                em.PopupCancle();
            else if (!em.isPopup && mbb.currentNum == 2)
                StageReturn();
            else if (!em.isPopup && mbb.currentNum == 1)
                CampaignReturn();
        }
        #endif
    }

    public void CampaignSelect()
    {
        stage.SetActive(false);
        stageInfo.SetActive(true);
        mbb.currentNum = 1;
    }

    public void CampaignReturn()
    {
        stage.SetActive(true);
        stageInfo.SetActive(false);
        mbb.currentNum -= 1;
    }

    public void StageSelect()
    {
        stageInfo.SetActive(false);
        fleetManage.SetActive(true);
        mbb.currentNum += 1;
    }

    public void StageReturn()
    {
        stageInfo.SetActive(true);
        fleetManage.SetActive(false);
        mbb.currentNum -= 1;
    }

    public void StageProgressCheck()
    {
        cd[0].toggle.value = true;
        cd[0].StageSelect();

        for (int i = 0; i < cd.Length; i++)
            cd[i].StageCheck(PlayerPrefs.GetString("StartRace"));
    }
}