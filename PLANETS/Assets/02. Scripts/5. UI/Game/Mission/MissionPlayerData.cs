using UnityEngine;

public class MissionPlayerData : MonoBehaviour
{
    public int id;
    public TweenAlpha ta;

    [Header("UI: Image")]
    public UISprite background;
    public UISprite shipImage;
    public UISprite shipState;
    public UISprite shipIcon;

    [Header("UI: Label")]
    public UILabel shipName;
    public UILabel shipInfo;

    int dataLv;
    string dataRace, dataType, dataPlus, dataName;

    public void DataParsing(int dLv, string dRace, string dType, string dPlus, string dName)
    {
        dataLv = dLv; dataRace = dRace; dataType = dType;
        dataPlus = dPlus; dataName = dName;
    }

    public void ResultData()
    {
        string dataState = PlayerPrefs.GetString("ShipState_" + dataName, "Normal");

        shipName.text = dataName;
        shipInfo.text = "Lv." + dataLv + " " + dataPlus;
        shipIcon.spriteName = "Ship_Type_" + dataType;
        shipImage.spriteName = "Base " + dataRace + " " + dataPlus + " " + dataType;
        shipState.spriteName = "ShipState_" + dataState;

        ta.PlayForward();
    }
}
