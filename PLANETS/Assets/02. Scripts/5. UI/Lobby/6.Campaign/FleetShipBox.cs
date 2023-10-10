using UnityEngine;

public class FleetShipBox : MonoBehaviour
{
    public int posID;

    [Header("UI")]
    public UIToggle toggle;
    public UISprite shipImage;
    public UISprite shipIcon;
    public UILabel shipName;
    public UILabel shipInfo;

    [Header("Script")]
    public ShipManager sm;
    public EmbarkManager em;
    [HideInInspector] public EmbarkData ed;

    [HideInInspector] public bool isEmpty = true;
    [HideInInspector] public string dataRace, dataType, dataName, dataPlus;
    [HideInInspector] public int dataLv;

    public void DataLoad()
    {
        if (PlayerPrefs.GetInt("ShipBox_" + posID, 0) == 1)
        {
            dataRace = PlayerPrefs.GetString("PlayerRace_" + posID);
            dataType = PlayerPrefs.GetString("PlayerType_" + posID);
            dataName = PlayerPrefs.GetString("PlayerName_" + posID);

            if (PlayerPrefs.GetString("ShipState_" + dataName, "Normal") != "Destroyed")
            {
                for (int i = 0; i < sm.embarkShips.Count; i++)
                {
                    if (sm.embarkShips[i].dataName == dataName)
                        ed = sm.embarkShips[i];
                }

                Equip(ed.dataRace, ed.dataName, ed.dataType, ed.dataPlus, ed.dataLv, ed);
                ed.button.isEnabled = false;
                ed.select.SetActive(true);
            }
            else
                Empty();
        }
        else
            Empty();
    }

    void OnDrag()
    {
        if (!isEmpty)
            transform.position = UICamera.currentCamera.ScreenPointToRay(UICamera.lastEventPosition).origin;
    }

    void OnDragEnd()
    {
        if (!isEmpty)
        {
            Swap(UICamera.lastHit.collider != null ? UICamera.lastHit.collider.gameObject : null);
            transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void OnValueChange()
    {
        if (toggle.value && isEmpty)
        {
            em.blind.SetActive(false);
        }
        else if (toggle.value && !isEmpty)
        {
        }
        else
        {
            em.blind.SetActive(true);
        }
    }

    public void Equip(string dRace, string dName, string dType, string dPlus, int dLv, EmbarkData dEd)
    {
        PlayerPrefs.SetInt("ShipBox_" + posID, 1);
        isEmpty = false;

        dataRace = dRace; dataName = dName; dataType = dType;
        dataPlus = dPlus; dataLv = dLv; ed = dEd;

        PlayerPrefs.SetString("PlayerRace_" + posID, dataRace);
        PlayerPrefs.SetString("PlayerType_" + posID, dataType);
        PlayerPrefs.SetString("PlayerName_" + posID, dataName);

        shipName.text = dataName;
        shipInfo.text = "Lv." + dataLv + " " + dataPlus;
        shipIcon.spriteName = "Ship_Type_" + dataType;
        shipImage.spriteName = "Base " + dataRace + " " + dataPlus + " " + dataType;

        em.CapacitySetting();
    }

    public void Empty()
    {
        PlayerPrefs.SetInt("ShipBox_" + posID, 0);
        isEmpty = true;

        dataRace = "Race"; dataName = "Name"; dataType = "Type";
        dataPlus = "Plus"; dataLv   = 0; ed = null;

        PlayerPrefs.SetString("PlayerRace_" + posID, dataRace);
        PlayerPrefs.SetString("PlayerType_" + posID, dataType);
        PlayerPrefs.SetString("PlayerName_" + posID, dataName);

        shipName.text = "";
        shipInfo.text = "";
        shipIcon.spriteName  = "";
        shipImage.spriteName = "Icon Ship None";

        em.CapacitySetting();
    }

    public void Swap(GameObject change)
    {
        if (change.GetComponent<FleetShipBox>() != null)
        {
            string cRace = dataRace, cName = dataName, cType = dataType, cPlus = dataPlus;
            int cLv = dataLv;
            EmbarkData cEd = ed;

            FleetShipBox cData = change.GetComponent<FleetShipBox>();

            if (!cData.isEmpty)
            {
                Equip(cData.dataRace, cData.dataName, cData.dataType, cData.dataPlus, cData.dataLv, cData.ed);
                cData.Equip(cRace, cName, cType, cPlus, cLv, cEd);
            }
            else
            {
                Empty();
                cData.Equip(cRace, cName, cType, cPlus, cLv, cEd);
            }
        }
        else
        {
            if (ed != null)
            {
                ed.button.isEnabled = true;
                ed.select.SetActive(false);
            }

            Empty();
            return;
        }
    }
}
