using System.IO;
using UnityEngine;
using LitJson;

public class ManageData : MonoBehaviour
{
    [HideInInspector] public bool isUpgrade = false, isRepair = false;
    [HideInInspector] public int dataID, dataLv;
    [HideInInspector] public string dataRace, dataType, dataPlus, dataName, dataStat;
    [HideInInspector] public float cCredit, cCore, uCredit, uCore, rCredit, rCore;
    int upLv; string upPlus;

    [Header("UI: Click")]
    public UIToggle toggle;
    public UIButton upgrade;
    public UIButton repair;

    [Header("UI: Tween")]
    public TweenAlpha[] ta;

    [Header("UI: Background")]
    public UISprite background;
    public UISprite backgroundLayer;

    [Header("UI: Image")]
    public UISprite shipImage;
    public UISprite stateImage;

    [Header("UI: Icon")]
    public UISprite typeIcon;
    public GameObject upgradeIcon;

    [Header("UI: Label")]
    public UILabel nameLabel;
    public UILabel infoLabel;
    public UILabel stateLabel;
    
    [HideInInspector] public ShipManager sm;
    [HideInInspector] public GameObject embarkShip;

    public void DataParsing(ShipManager SM, int ID, int Lv, string Race, string Type, string Plus, string Name, string Stat)
    {
        sm       = SM;   dataID   = ID;   dataLv   = Lv;
        dataRace = Race; dataType = Type; dataPlus = Plus;
        dataName = Name; dataStat = Stat;

        string shipState = PlayerPrefs.GetString("ShipState_" + dataName, "Normal");

        nameLabel.text  = dataName;
        infoLabel.text  = "Lv." + dataLv + " " + dataPlus;
        stateLabel.text = shipState;

        shipImage.spriteName = "Base " + dataRace + " " + dataPlus + " " + dataType;
        typeIcon.spriteName  = "Ship_Type_" + dataType;

        // 함선 수리 상태 체크
        if (shipState == "Damaged")
        {
            isRepair = true;
            repair.enabled = true;
            ta[1].PlayReverse();
            stateLabel.text = "소파";

            background.spriteName      = "Manage_Frame_Damaged";
            backgroundLayer.spriteName = "Manage_Frame_Damaged Layer";
            backgroundLayer.gameObject.SetActive(true);
        }
        else if (shipState == "Deadly")
        {
            isRepair = true;
            repair.enabled = true;
            ta[1].PlayReverse();
            stateLabel.text = "대파";

            background.spriteName      = "Manage_Frame_Deadly";
            backgroundLayer.spriteName = "Manage_Frame_Deadly Layer";
            backgroundLayer.gameObject.SetActive(true);
        }
        else if (shipState == "Destroyed")
        {
            repair.enabled = false;
            ta[1].PlayForward();
            stateLabel.text = "파괴됨";

            background.spriteName = "Manage_Frame_Destroyed";
        }
        else
        {
            stateLabel.text = "정상";
            repair.enabled = false;
            ta[1].PlayForward();
        }

        PlayerDataBase.Instance.RepairDataParsing(this, shipState);
    }

    public void ResearchLevelCheck()
    {
        // 함선 업그레이드 가능 체크
        int rLevel   = PlayerPrefs.GetInt("Research_" + dataRace + "_" + dataType + "_Level", 1);
        string rPlus = PlayerPrefs.GetString("Research_" + dataRace + "_" + dataType + "_Plus", "N");

        if (dataLv != rLevel || dataPlus != rPlus)
        {
            isUpgrade = true;
            upgradeIcon.SetActive(true);
            upgrade.enabled = true;
            ta[0].PlayReverse();
        }
        else
        {
            upgrade.enabled = false;
            ta[0].PlayForward();
        }

        upLv = rLevel;
        upPlus = rPlus;
        PlayerDataBase.Instance.UpgradeDataParsing(this, dataLv, upLv, dataPlus, upPlus);
    }

    public void UpgradeButtonClick()
    {
        float fCredit = uCredit - cCredit;
        float fCore = uCore - cCore;

        sm.upgrade = this;

        sm.currentShip.spriteName = "Base " + dataRace + " " + dataPlus + " " + dataType;
        sm.nextShip.spriteName    = "Base " + dataRace + " " + upPlus + " " + dataType;

        sm.currentLabel.text = dataType + " " + dataLv + dataPlus;
        sm.nextLabel.text    = dataType + " " + upLv + upPlus;

        sm.upgradeCredit.text = fCredit.ToString();
        sm.upgradeCore.text   = fCore.ToString();
        sm.PopupActiveUpgrade();
    }

    public void ShipUpgrading()
    {
        sm.PopupCancle();

        if (isUpgrade && CostPay_Upgrade())
        {
            isUpgrade = false;

            if (File.Exists(NextChangeScene.Instance.shipFilePath))
            {
                string jsonStr = File.ReadAllText(NextChangeScene.Instance.shipFilePath);
                JsonData jsonData = JsonMapper.ToObject(jsonStr);

                for (int i = 0; i < jsonData.Count; i++)
                {
                    string jName = jsonData[i]["Name"].ToString();

                    if (jName == dataName)
                    {
                        dataLv   = upLv;
                        dataPlus = upPlus;

                        NextChangeScene.Instance.shipLists[i].Lv   = dataLv;
                        NextChangeScene.Instance.shipLists[i].Plus = dataPlus;

                        PlayerPrefs.SetInt("ShipLevel_" + dataName, dataLv);
                        PlayerPrefs.SetString("ShipTypePlus_" + dataName, dataPlus);
                    }
                }

                jsonData = JsonMapper.ToJson(NextChangeScene.Instance.shipLists);
                File.WriteAllText(NextChangeScene.Instance.shipFilePath, jsonData.ToString());
            }

            infoLabel.text = "Lv." + dataLv + " " + dataPlus;
            shipImage.spriteName = "Base " + dataRace + " " + dataPlus + " " + dataType;
            upgradeIcon.SetActive(false);
            upgrade.enabled = false;

            ta[0].PlayForward();
            toggle.value = false;
        }
    }

    public void RepairButtonClick()
    {
        sm.repair = this;
        sm.repairCredit.text = rCredit.ToString();
        sm.repairCore.text   = rCore.ToString();
        sm.PopupActiveRepair();
    }

    public void ShipRepairing()
    {
        sm.PopupCancle();

        if (isRepair && CostPay_Repair())
        {
            isRepair = false;
            PlayerPrefs.SetString("ShipState_" + dataName, "Normal");

            background.spriteName = "Manage_Frame";
            backgroundLayer.gameObject.SetActive(false);
            stateLabel.text = "Normal";

            repair.enabled = false;

            ta[1].PlayForward();
            toggle.value = false;
        }
    }
    
    public void ShipBreaking()
    {
        if (File.Exists(NextChangeScene.Instance.shipFilePath))
        {
            for (int i = 0; i < NextChangeScene.Instance.shipLists.Count; i++)
            {
                if (NextChangeScene.Instance.shipLists[i].ID == dataID)
                {
                    //CostRefund();

                    int capacity = PlayerPrefs.GetInt("Capacity");
                    PlayerPrefs.SetInt("Capacity", capacity - 1);

                    sm.embarkShips.Remove(embarkShip.GetComponent<EmbarkData>());
                    Destroy(embarkShip);

                    sm.manageShips.Remove(this);
                    Destroy(gameObject);

                    NextChangeScene.Instance.shipLists.Remove(NextChangeScene.Instance.shipLists[i]);
                }
            }

            JsonData jsonData = JsonMapper.ToJson(NextChangeScene.Instance.shipLists);
            File.WriteAllText(NextChangeScene.Instance.shipFilePath, jsonData.ToString());

            sm.grid_embark.repositionNow = true;
            sm.grid_manage.repositionNow = true;
        }
    }

    bool CostPay_Upgrade()
    {
        sm.rd.mbb = sm.mbb;
        sm.rd.mbbNum = 1;
        sm.rd.resourceCheck = true;

        float fCredit = uCredit - cCredit;
        float fCore   = uCore - cCore;

        if (PlayerPrefs.HasKey("Building_ControlCenter"))
        {
            fCredit = fCredit * 0.8f;
            fCore   = fCore * 0.8f;
        }

        int checkCredit = sm.rd.PayResource("Credit", (int)fCredit);
        int checkCore   = sm.rd.PayResource("Core", (int)fCore);

        if (sm.rd.resourceCheck)
        {
            sm.rd.ResourceLabel("Credit", checkCredit);
            sm.rd.ResourceLabel("Core", checkCore);
            return true;
        }
        else
        {
            sm.PopupCancle();
            return false;
        }
    }

    bool CostPay_Repair()
    {
        sm.rd.mbb = sm.mbb;
        sm.rd.mbbNum = 1;
        sm.rd.resourceCheck = true;

        if (PlayerPrefs.HasKey("Building_ControlCenter"))
        {
            rCredit = rCredit * 0.8f;
            rCore   = rCore * 0.8f;
        }

        int checkCredit = sm.rd.PayResource("Credit", (int)rCredit);
        int checkCore   = sm.rd.PayResource("Core", (int)rCore);

        if (sm.rd.resourceCheck)
        {
            sm.rd.ResourceLabel("Credit", checkCredit);
            sm.rd.ResourceLabel("Core", checkCore);
            return true;
        }
        else
        {
            sm.PopupCancle();
            return false;
        }
    }

    void CostRefund()
    {
        sm.rd.RefundResource("Credit", 1000);
        sm.rd.RefundResource("Core", 10);
    }
}
