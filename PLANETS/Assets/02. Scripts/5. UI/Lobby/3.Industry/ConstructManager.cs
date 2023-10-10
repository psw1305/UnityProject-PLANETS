using System.Collections;
using System.IO;
using UnityEngine;
using LitJson;

public class ConstructManager : MonoBehaviour
{
    [HideInInspector] public string raceName, shipType, shipPlus, shipName;
    [HideInInspector] public float damage, health, shield;
    [HideInInspector] public int credit, core, lvCheck, maintain;

    [Header("UI: Stat")]
    public UISprite blueprint;
    public UILabel nameLabel;
    public UILabel damageLabel, healthLabel, shieldLabel, maintainLabel;

    [Header("UI: Skill")]
    public UISprite skillIcon;
    public UILabel skillLevel;
    public UILabel skillNameLabel, skillInfoLabel;

    [Header("UI: Resource")]
    public UILabel creditLabel;
    public UILabel coreLabel;

    [Header("List")]
    public UIToggle[] toggles;

    [Header("Popup")]
    public GameObject shipBuy;
    public GameObject shipMax;
    public UISprite popupShip;
    public UILabel popupInfo;
    public UILabel shipMaxLabel;
    bool isPopup = false;

    [Header("Script")]
    public ShipManager sm;
    public RaceListSelect rls;
    public ResourceData rd;
    public MainBackButton mbb;

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

    public void HangarShipListChange(string rName)
    {
        raceName = rName;
        DataCheck();
    }

    public void Click_Destroyer()
    {
        if (toggles[0].value)
        {
            shipName = rls.shipNames[0].text;
            ToggleValueCheck("Destroyer");
        }
    }

    public void Click_Auxiliary()
    {
        if (toggles[1].value)
        {
            shipName = rls.shipNames[1].text;
            ToggleValueCheck("Auxiliary");
        }
    }

    public void Click_Cruiser()
    {
        if (toggles[2].value)
        {
            shipName = rls.shipNames[2].text;
            ToggleValueCheck("Cruiser");
        }
    }

    public void Click_Carrier()
    {
        if (toggles[3].value)
        {
            shipName = rls.shipNames[3].text;
            ToggleValueCheck("Carrier");
        }
    }

    public void Click_Battleship()
    {
        if (toggles[4].value)
        {
            shipName = rls.shipNames[4].text;
            ToggleValueCheck("Battleship");
        }
    }

    void ToggleValueCheck(string type)
    {
        shipType = type;
        shipPlus = PlayerPrefs.GetString("Research_" + raceName + "_" + shipType + "_Plus", "N");
        lvCheck  = PlayerPrefs.GetInt("Research_" + raceName + "_" + shipType + "_Level", 1);
        StageDataBase.Instance.ConstructDataParsing(this, raceName, shipType, shipPlus, lvCheck);
    }

    public void ShipConstructing()
    {
        PopupCancle();

        int idCheck = PlayerPrefs.GetInt("ShipCheck", 0);
        int capacity = PlayerPrefs.GetInt("Capacity");

        if (CostPay())
        {
            capacity += 1;
            PlayerPrefs.SetInt("Capacity", capacity);

            idCheck += 1;
            string idName = shipName + "_" + idCheck;
            StartCoroutine(CreateShipData(idCheck, lvCheck, raceName, shipType, shipPlus, idName));

            PlayerPrefs.SetInt("ShipCheck", idCheck);
            PopupCancle();

            sm.LoadShipCheck();
        }
    }

    IEnumerator CreateShipData(int id, int lv, string race, string type, string plus, string name)
    {
        NextChangeScene.Instance.shipLists.Add(new PlayerShipData(id, lv, race, type, plus, name, "Construct"));

        PlayerPrefs.SetInt("ShipLevel_" + name, lv);
        PlayerPrefs.SetInt("ShipMaintain_" + name, maintain);
        PlayerPrefs.SetString("ShipTypePlus_" + name, plus);

        JsonData jsonData = JsonMapper.ToJson(NextChangeScene.Instance.shipLists);
        File.WriteAllText(NextChangeScene.Instance.shipFilePath, jsonData.ToString());

        yield return null;
    }

    bool CostPay()
    {
        rd.mbb = mbb;
        rd.mbbNum = 1;
        rd.resourceCheck = true;

        int checkCredit = rd.PayResource("Credit", credit);
        int checkCore   = rd.PayResource("Core", core);

        if (rd.resourceCheck)
        {
            rd.ResourceLabel("Credit", checkCredit);
            rd.ResourceLabel("Core", checkCore);
            return true;
        }
        else
        {
            PopupCancle();
            return false;
        }
    }

    public void PopupActive()
    {
        popupInfo.text       = "Lv." + lvCheck + " " + shipPlus;
        popupShip.spriteName = "Base " + raceName + " " + shipPlus + " " + shipType;

        int capacity = PlayerPrefs.GetInt("Capacity", 0);
        int max;

        if (PlayerPrefs.HasKey("Building_HangarExpansion"))
        {
            max = 12;
            shipMaxLabel.text = "최대 생산 12대에 도달하여" + '\n' + "함선을 더 건조할 수 없습니다.";
        }
        else
        {
            max = 8;
            shipMaxLabel.text = "최대 생산 8대에 도달하여" + '\n' + "함선을 더 건조할 수 없습니다." + '\n' + "(추가 격납고 건설시 +4 확장)";
        }

        if (capacity < max)
        {
            isPopup = true;
            mbb.currentNum = 1;
            shipBuy.SetActive(true);
        }
        else
        {
            isPopup = true;
            mbb.currentNum = 1;
            shipMax.SetActive(true);
        }
    }

    public void PopupCancle()
    {
        if (isPopup && shipBuy.activeSelf)
        {
            isPopup = false;
            mbb.currentNum -= 1;
            shipBuy.SetActive(false);
        }
        else if (isPopup && shipMax.activeSelf)
        {
            isPopup = false;
            mbb.currentNum -= 1;
            shipMax.SetActive(false);
        }
    }
}
