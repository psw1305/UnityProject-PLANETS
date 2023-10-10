using UnityEngine;

public class EmbarkData : MonoBehaviour
{
    public UIButton button;

    [Header("UI: Image")]
    public UISprite background;
    public UISprite shipImage;
    public UISprite shipState;
    public UISprite shipIcon;
    public GameObject select, destroyed;

    [Header("UI: Label")]
    public UILabel shipName;
    public UILabel shipInfo;

    [HideInInspector] public int dataID, dataLv, dataCap;
    [HideInInspector] public string dataRace, dataName, dataType, dataPlus, dataStat;
    [HideInInspector] public string state;
    [HideInInspector] public EmbarkManager em;

    private void Start()
    {
        if (PlayerPrefs.GetString("ShipState_" + dataName, "Normal") == "Destroyed")
        {
            button.disabledColor = new Color32(80, 80, 80, 255);
            button.isEnabled = false;
            destroyed.SetActive(true);
        }
    }

    public void OnClick()
    {
        for (int i = 0; i < em.fsb.Length; i++)
        {
            if (em.fsb[i].toggle.value)
            {
                if (em.ShipCapacityCheck())
                {
                    em.fsb[i].Equip(dataRace, dataName, dataType, dataPlus, dataLv, this);
                    em.fsb[i].toggle.value = false;
                    em.fsb[i].ed.button.isEnabled = false;
                    em.fsb[i].ed.select.SetActive(true);
                }
            }
        }
    }

    public void DataParsing(EmbarkManager EM, int ID, int Lv, string Race, string Type, string Plus, string Name, string Stat)
    {
        em       = EM;   dataID   = ID;   dataLv   = Lv;
        dataRace = Race; dataType = Type; dataPlus = Plus;
        dataName = Name; dataStat = Stat;

        dataCap = PlayerPrefs.GetInt("ShipMaintain_" + dataName);
        //string dataState = PlayerPrefs.GetString("ShipState_" + dataName, "Normal");

        shipName.text = dataName;
        shipInfo.text = "Lv." + dataLv + " " + dataPlus;
        shipIcon.spriteName  = "Ship_Type_" + dataType;
        shipImage.spriteName = "Base " + dataRace + " " + dataPlus + " " + dataType;
        shipState.spriteName = "";
    }
}
