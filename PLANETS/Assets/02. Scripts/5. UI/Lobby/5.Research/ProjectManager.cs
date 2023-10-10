using UnityEngine;

public class ProjectManager : MonoBehaviour
{
    [HideInInspector] public string raceName, shipType, shipPlus;
    [HideInInspector] public string skillID, skillLv;
    [HideInInspector] public int credit, core, dmatter, lvCheck;

    [Header("UI")]
    public UIToggle[] toggles;
    public UIButton startBtn;

    [Header("UI: Next")]
    public UISprite shipImage;
    public UILabel shipInfo;
    public UILabel[] nextShipStats;
    public UILabel skillName;
    public UILabel skilllnfo;

    [Header("UI: Resource")]
    public UILabel creditLabel;
    public UILabel coreLabel;
    public UILabel dmatterLabel;

    [Header("Popup")]
    public GameObject popup;
    public UISprite popupIcon;
    public UILabel popupLv;
    bool isPopup = false;

    [Header("Script")]
    public ProjectData[] pd;
    public RaceListSelect rls;
    public ResourceData rd;
    public MainBackButton mbb;
    [HideInInspector] public ProjectData curPd;

    void Start()
    {
        raceName = "Terran";
        Click_Destroyer();
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
        switch (shipType)
        {
            case "Destroyer":
                Click_Destroyer();
                break;
            case "Auxiliary":
                Click_Auxiliary();
                break;
            case "Cruiser":
                Click_Cruiser();
                break;
            case "Carrier":
                Click_Carrier();
                break;
            case "Battleship":
                Click_Battleship();
                break;
        }
    }

    public void ProjectShipListChange(string rName)
    {
        raceName = rName;
        ToggleValueCheck();
    }

    public void Click_Destroyer()
    {
        if (toggles[0].value)
        {
            shipType = "Destroyer";
            ToggleValueCheck();
        }
    }

    public void Click_Auxiliary()
    {
        if (toggles[1].value)
        {
            shipType = "Auxiliary";
            ToggleValueCheck();
        }
    }

    public void Click_Cruiser()
    {
        if (toggles[2].value)
        {
            shipType = "Cruiser";
            ToggleValueCheck();
        }
    }

    public void Click_Carrier()
    {
        if (toggles[3].value)
        {
            shipType = "Carrier";
            ToggleValueCheck();
        }
    }

    public void Click_Battleship()
    {
        if (toggles[4].value)
        {
            shipType = "Battleship";
            ToggleValueCheck();
        }
    }

    void ToggleValueCheck()
    {
        lvCheck  = PlayerPrefs.GetInt("Research_" + raceName + "_" + shipType + "_Level", 1);
        shipPlus = PlayerPrefs.GetString("Research_" + raceName + "_" + shipType + "_Plus", "N");

        ProjectDataCheck();
    }

    public void ProjectDataCheck()
    {
        for (int i = 0; i < pd.Length; i++)
        {
            pd[i].icon.spriteName = "Skill_" + raceName + "_" + shipType + "_" + pd[i].plus;

            if (pd[i].level == lvCheck && pd[i].plus == shipPlus)
            {
                pd[i].ProjectDataParsing();

                pd[i].isResearch = true;
                pd[i].OnComplete();

                pd[i].toggle.value = true;
                pd[i].OnValueChange();
            }
            else if (pd[i].level < lvCheck && pd[i].plus == shipPlus)
            {
                pd[i].isResearch = true;
                pd[i].OnComplete();
            }
            else if (pd[i].level < lvCheck && pd[i].plus == "N")
            {
                pd[i].isResearch = true;
                pd[i].OnComplete();
            }
            else
            {
                pd[i].isResearch = false;
                pd[i].OnUncomplete();
            }
        }

        if (curPd.nextPd != null)
        {
            for (int j = 0; j < curPd.nextPd.Length; j++)
            {
                if (curPd.nextPd[j].plus == "N")
                    curPd.nextPd[j].OnRelease();
                else
                {
                    if (PlayerPrefs.GetInt("Unlock_" + raceName + "_" + shipType + "_" + curPd.nextPd[j].plus) == 1)
                        curPd.nextPd[j].OnRelease();
                }
            }
        }
    }

    public void ProjectStart()
    {
        PopupCancle();

        if (CostPay())
        {
            PlayerPrefs.SetInt("Research_" + raceName + "_" + shipType + "_Level", lvCheck);
            PlayerPrefs.SetString("Research_" + raceName + "_" + shipType + "_Plus", shipPlus);

            ProjectDataCheck();
        }
    }

    bool CostPay()
    {
        rd.mbb = mbb;
        rd.mbbNum = 1;
        rd.resourceCheck = true;

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
        popupIcon.spriteName = "Skill_" + raceName + "_" + shipType + "_" + skillID;
        popupLv.text = skillLv;

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
