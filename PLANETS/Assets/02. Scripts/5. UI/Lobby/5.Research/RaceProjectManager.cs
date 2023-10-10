using UnityEngine;

public class RaceProjectManager : MonoBehaviour
{
    [HideInInspector] public string raceName, shipType, shipPlus;
    [HideInInspector] public int credit, core, dmatter, lvCheck;
    string typePlus;

    [Header("UI")]
    public UIToggle[] toggles;
    public UILabel creditLabel, dmatterLabel;
    public UIButton startBtn;

    [Header("UI: Next")]
    public UISprite shipImage;
    public UILabel shipInfo;
    public UISprite skillImage;
    public UILabel skillName;
    public UILabel skillInfo;

    [Header("Popup")]
    public GameObject popup;
    public UISprite popupIcon;
    public UILabel popupPlus;
    bool isPopup = false;

    [Header("Script")]
    public ProjectManager pm;
    public RaceProjectData[] rpd;
    public ResourceData rd;
    public MainBackButton mbb;
    [HideInInspector] public RaceProjectData curRpd;

    void Awake()
    {
        if (PlayerPrefs.GetInt("Race_Project_Start", 1) == 1)
        {
            PlayerPrefs.SetInt("Race_Project_Start", 0);

            RaceTypeList("Kalas");
            RaceTypeList("ShadowFang");
            RaceTypeList("Aridrian");
            RaceTypeList("Harbinger");

            PlayerPrefs.SetInt("Unlock_Terran_Destroyer_A", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Destroyer_B", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Auxiliary_A", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Auxiliary_B", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Cruiser_A", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Cruiser_B", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Carrier_A", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Carrier_B", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Battleship_A", 1);
            PlayerPrefs.SetInt("Unlock_Terran_Battleship_B", 1);
        }
    }

    void RaceTypeList(string rName)
    {
        string[] shipTypes = { "Destroyer_A", "Destroyer_B", "Auxiliary_A", "Auxiliary_B", "Cruiser_A", "Cruiser_B", "Carrier_A", "Carrier_B", "Battleship_A", "Battleship_B" };

        for (int i = 0; i < 10; i++)
        {
            int ranIdx = Random.Range(i, 10);

            string tmp = shipTypes[ranIdx];
            shipTypes[ranIdx] = shipTypes[i];
            shipTypes[i] = tmp;

            PlayerPrefs.SetString(rName + "_ProjectType_" + i, shipTypes[i]);
        }
    }

    void Start()
    {
        raceName = "Kalas";
        Click_Kalas();
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

    public void DataCheck()
    {
        switch (raceName)
        {
            case "Kalas":
                Click_Kalas();
                break;
            case "ShadowFang":
                Click_ShadowFang();
                break;
            case "Aridrian":
                Click_Aridrian();
                break;
            case "Harbinger":
                Click_Harbinger();
                break;
        }
    }

    public void Click_Kalas()
    {
        if (toggles[0].value)
        {
            raceName = "Kalas";
            ToggleValueCheck();
        }
    }

    public void Click_ShadowFang()
    {
        if (toggles[1].value)
        {
            raceName = "ShadowFang";
            ToggleValueCheck();
        }
    }

    public void Click_Aridrian()
    {
        if (toggles[2].value)
        {
            raceName = "Aridrian";
            ToggleValueCheck();
        }
    }

    public void Click_Harbinger()
    {
        if (toggles[3].value)
        {
            raceName = "Harbinger";
            ToggleValueCheck();
        }
    }

    void ToggleValueCheck()
    {
        lvCheck = PlayerPrefs.GetInt("Unlock_" + raceName + "_Level", 0);

        for (int i = 0; i < rpd.Length; i++)
        {
            typePlus = PlayerPrefs.GetString(raceName + "_ProjectType_" + i);
            rpd[i].ShipTypeInfo(raceName, typePlus);

            if (rpd[i].level == lvCheck )
            {
                rpd[i].isResearch = true;
                rpd[i].OnComplete();

                rpd[i].toggle.value = true;
                rpd[i].OnValueChange();
            }
            else if (rpd[i].level < lvCheck)
            {
                rpd[i].isResearch = true;
                rpd[i].OnComplete();
            }
            else
            {
                rpd[i].isResearch = false;
                rpd[i].OnUncomplete();
            }
        }

        if (lvCheck == 0)
        {
            rpd[0].OnRelease();
            rpd[0].toggle.value = true;
            rpd[0].OnValueChange();
        }
        else
        {
            if (curRpd.nextRpd != null)
                curRpd.nextRpd.OnRelease();
        }
    }

    public void ProjectStart()
    {
        PopupCancle();

        if (CostPay())
        {
            PlayerPrefs.SetInt("Unlock_" + raceName + "_Level", lvCheck);
            PlayerPrefs.SetInt("Unlock_" + raceName + "_" + shipType + "_" + shipPlus, 1);

            ToggleValueCheck();
            pm.ProjectDataCheck();
        }
    }

    bool CostPay()
    {
        rd.mbb = mbb;
        rd.mbbNum = 1;
        rd.resourceCheck = true;

        if (PlayerPrefs.HasKey("Building_ParticleAccelerator1") && PlayerPrefs.HasKey("Building_ParticleAccelerator2"))
        {
            credit = (int)(credit * 0.7);
            dmatter = (int)(dmatter * 0.7);
        }
        else if (PlayerPrefs.HasKey("Building_ParticleAccelerator1") || PlayerPrefs.HasKey("Building_ParticleAccelerator2"))
        {
            credit = (int)(credit * 0.85);
            dmatter = (int)(dmatter * 0.85);
        }

        int checkCredit  = rd.PayResource("Credit", credit);
        int checkCore    = rd.PayResource("Core", core);
        int checkDMatter = rd.PayResource("DMatter", dmatter);

        if (rd.resourceCheck)
        {
            rd.ResourceLabel("Credit", checkCredit);
            rd.ResourceLabel("Core", checkCore);
            rd.ResourceLabel("DMatter", checkDMatter);
            return true;
        }
        else
            return false;
    }

    void CostRefund()
    {
        rd.RefundResource("Credit", credit);
        rd.RefundResource("Core", core);
        rd.RefundResource("DMatter", dmatter);
    }

    public void PopupActive()
    {
        popupIcon.spriteName = "Skill_" + raceName + "_" + shipType + "_N";
        popupPlus.text = shipPlus;

        isPopup = true;
        mbb.currentNum = 1;
        popup.SetActive(true);
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
