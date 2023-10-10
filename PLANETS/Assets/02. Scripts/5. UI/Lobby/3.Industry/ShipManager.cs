using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class ShipManager : MonoBehaviour
{
    public List<ManageData> manageShips = new List<ManageData>();
    public List<EmbarkData> embarkShips = new List<EmbarkData>();

    [Header("UI")]
    public UIGrid grid_manage;
    public UIGrid grid_embark;

    [Header("Prefab")]
    public GameObject manageShipPrefab;
    public GameObject embarkShipPrefab;

    [Header("Popup")]
    public GameObject shipRepair;
    public UILabel repairCredit, repairCore;
    public GameObject shipUpgrade;
    public UISprite currentShip, nextShip;
    public UILabel currentLabel, nextLabel;
    public UILabel upgradeCredit, upgradeCore;

    [HideInInspector] public ManageData repair, upgrade;
    bool isPopup = false;

    [Header("Script")]
    public EmbarkManager em;
    public UIScrollView scroll;
    public ResourceData rd;
    public MainBackButton mbb;

    void Start()
    {
        StartCoroutine("StartShipData");
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

    public void LoadShipCheck()
    {
        StartCoroutine("LoadShipData");
    }

    public void ShipLevelCheck()
    {
        for (int i = 0; i < manageShips.Count; i++)
            manageShips[i].ResearchLevelCheck();
    }

    IEnumerator StartShipData()
    {
        if (File.Exists(NextChangeScene.Instance.shipFilePath))
        {
            string jsonStr = File.ReadAllText(NextChangeScene.Instance.shipFilePath);
            JsonData jsonData = JsonMapper.ToObject(jsonStr);

            for (int i = 0; i < jsonData.Count; i++)
            {
                int jID = (int)jsonData[i]["ID"];
                int jLv = (int)jsonData[i]["Lv"];

                string jRace = jsonData[i]["Race"].ToString();
                string jType = jsonData[i]["Type"].ToString();
                string jPlus = jsonData[i]["Plus"].ToString();
                string jName = jsonData[i]["Name"].ToString();
                string jStat = jsonData[i]["Stat"].ToString();

                NextChangeScene.Instance.shipLists.Add(new PlayerShipData(jID, jLv, jRace, jType, jPlus, jName, jStat));

                if (jStat == "New")
                {
                    // 출정창
                    GameObject embarkShipClone = Instantiate(embarkShipPrefab) as GameObject;
                    embarkShipClone.GetComponent<EmbarkData>().DataParsing(em, jID, jLv, jRace, jType, jPlus, jName, jStat);
                    embarkShipClone.GetComponent<UIDragScrollView>().scrollView = scroll;

                    embarkShipClone.transform.parent = grid_embark.transform;
                    embarkShipClone.transform.localScale = new Vector3(1, 1, 1);
                    embarkShips.Add(embarkShipClone.GetComponent<EmbarkData>());

                    // 관리창
                    GameObject manageShipClone = Instantiate(manageShipPrefab) as GameObject;
                    manageShipClone.GetComponent<ManageData>().embarkShip = embarkShipClone;
                    manageShipClone.GetComponent<ManageData>().DataParsing(this, jID, jLv, jRace, jType, jPlus, jName, jStat);

                    manageShipClone.transform.parent = grid_manage.transform;
                    manageShipClone.transform.localScale = new Vector3(1, 1, 1);
                    manageShips.Add(manageShipClone.GetComponent<ManageData>());
                }
            }
        }

        grid_manage.repositionNow = true;
        grid_embark.repositionNow = true;
        yield return null;
    }

    IEnumerator LoadShipData()
    {
        if (File.Exists(NextChangeScene.Instance.shipFilePath))
        {
            string jsonStr = File.ReadAllText(NextChangeScene.Instance.shipFilePath);
            JsonData jsonData = JsonMapper.ToObject(jsonStr);

            for (int i = 0; i < jsonData.Count; i++)
            {
                int jID = (int)jsonData[i]["ID"];
                int jLv = (int)jsonData[i]["Lv"];

                string jRace = jsonData[i]["Race"].ToString();
                string jType = jsonData[i]["Type"].ToString();
                string jPlus = jsonData[i]["Plus"].ToString();
                string jName = jsonData[i]["Name"].ToString();
                string jStat = jsonData[i]["Stat"].ToString();

                if (jStat == "Construct")
                {
                    // 출정창
                    GameObject embarkShipClone = Instantiate(embarkShipPrefab) as GameObject;
                    embarkShipClone.GetComponent<EmbarkData>().DataParsing(em, jID, jLv, jRace, jType, jPlus, jName, "New");
                    embarkShipClone.GetComponent<UIDragScrollView>().scrollView = scroll;

                    embarkShipClone.transform.parent = grid_embark.transform;
                    embarkShipClone.transform.localScale = new Vector3(1, 1, 1);
                    embarkShips.Add(embarkShipClone.GetComponent<EmbarkData>());

                    // 관리창
                    GameObject manageShipClone = Instantiate(manageShipPrefab) as GameObject;
                    manageShipClone.GetComponent<ManageData>().embarkShip = embarkShipClone;
                    manageShipClone.GetComponent<ManageData>().DataParsing(this, jID, jLv, jRace, jType, jPlus, jName, "New");

                    manageShipClone.transform.parent = grid_manage.transform;
                    manageShipClone.transform.localScale = new Vector3(1, 1, 1);
                    manageShips.Add(manageShipClone.GetComponent<ManageData>());

                    NextChangeScene.Instance.shipLists[i].Stat = "New";
                }
                else if (jName == manageShips[i].dataName)
                {
                    embarkShips[i].DataParsing(em, jID, jLv, jRace, jType, jPlus, jName, jStat);
                    manageShips[i].DataParsing(this, jID, jLv, jRace, jType, jPlus, jName, jStat);
                }

            }

            em.FleetDataLoad();

            jsonData = JsonMapper.ToJson(NextChangeScene.Instance.shipLists);
            File.WriteAllText(NextChangeScene.Instance.shipFilePath, jsonData.ToString());
        }

        grid_manage.repositionNow = true;
        grid_embark.repositionNow = true;
        yield return null;
    }

    public void ManageShipRepair()
    {
        repair.ShipRepairing();
    }

    public void ManageShipUpgrade()
    {
        upgrade.ShipUpgrading();
    }

    public void PopupActiveRepair()
    {
        isPopup = true;
        mbb.currentNum = 1;
        shipRepair.SetActive(true);
    }

    public void PopupActiveUpgrade()
    {
        isPopup = true;
        mbb.currentNum = 1;
        shipUpgrade.SetActive(true);
    }

    public void PopupCancle()
    {
        if (isPopup && shipRepair.activeSelf)
        {
            isPopup = false;
            mbb.currentNum -= 1;
            shipRepair.SetActive(false);
        }
        else if (isPopup && shipUpgrade.activeSelf)
        {
            isPopup = false;
            mbb.currentNum -= 1;
            shipUpgrade.SetActive(false);
        }
    }
}
