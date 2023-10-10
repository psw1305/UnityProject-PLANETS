using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public GameObject popup;
    public UILabel popupName;
    public UILabel creditLabel, coreLabel, acoreLabel;

    [HideInInspector] public GameObject building;
    [HideInInspector] public string buildName;
    [HideInInspector] public int credit, core, acore;
    [HideInInspector] public bool isPopup = false;

    [Header("Script")]
    public BuildingData[] bd;
    public BuildingData check;
    public ResourceData rd;
    public MainBackButton mbb;

    void Start()
    {
        for (int i = 0; i < bd.Length; i++)
        {
            StageDataBase.Instance.BuildingDataParsing(bd[i], this);
            bd[i].DataCheck();
        }
    }

    void Update()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPopup && mbb.currentNum == 1)
                PopupCancle();
        }
        #endif
    }

    public void OnBuilding()
    {
        PopupCancle();

        if (CostPay())
        {
            PlayerPrefs.SetInt("Building_" + buildName, 1);
            building.SetActive(true);
            check.buildBtn.isEnabled = false;
            check.buildBtnLabel.text = "건설완료";
            mbb.MainByBuildingButton();
        }
    }

    bool CostPay()
    {
        rd.mbb = mbb;
        rd.mbbNum = 1;
        rd.resourceCheck = true;

        int checkCredit = rd.PayResource("Credit", credit);
        int checkCore   = rd.PayResource("Core", core);
        int checkACore  = rd.PayResource("ACore", acore);

        if (rd.resourceCheck)
        {
            rd.ResourceLabel("Credit", checkCredit);
            rd.ResourceLabel("Core", checkCore);
            rd.ResourceLabel("ACore", checkACore);
            return true;
        }
        else
        {
            PopupCancle();
            return false;
        }
    }

    public void PopupCancle()
    {
        if (isPopup && popup.activeSelf)
        {
            isPopup = false;
            mbb.currentNum -= 1;
            popup.SetActive(false);
        }
    }
}
