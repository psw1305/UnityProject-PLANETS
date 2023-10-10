using UnityEngine;

public class RaceProjectData : MonoBehaviour
{
    [HideInInspector] public string shipType, shipPlus;
    [HideInInspector] public bool isResearch = false, isNext = false;
    [HideInInspector] public int credit, dmatter;
    public int level;

    [Header("UI")]
    public UIToggle toggle;
    public UISprite frame;
    public UISprite check;
    public UISprite icon;
    public UILabel plus;

    [Header("Script")]
    public RaceProjectData nextRpd;
    public RaceProjectManager rpm;

    void Start()
    {
        PlayerDataBase.Instance.RaceProjectResourceDataParsing(this);
    }

    public void ShipTypeInfo(string rName, string rTypePlus)
    {
        TypePlusList(rTypePlus);
        icon.spriteName = "Skill_" + rName + "_" + shipType + "_N";
        plus.text = shipPlus;
    }

    void TypePlusList(string rTypePlus)
    {
        if (rTypePlus.Contains("Destroyer"))
            shipType = "Destroyer";
        else if (rTypePlus.Contains("Auxiliary"))
            shipType = "Auxiliary";
        else if (rTypePlus.Contains("Cruiser"))
            shipType = "Cruiser";
        else if (rTypePlus.Contains("Carrier"))
            shipType = "Carrier";
        else if (rTypePlus.Contains("Battleship"))
            shipType = "Battleship";

        if (rTypePlus.Contains("_A"))
            shipPlus = "A";
        else if (rTypePlus.Contains("_B"))
            shipPlus = "B";
    }

    public void OnValueChange()
    {
        if (toggle.value)
        {
            rpm.lvCheck  = level;
            rpm.shipType = shipType;
            rpm.shipPlus = shipPlus;

            rpm.credit  = credit;
            rpm.dmatter = dmatter;

            rpm.creditLabel.text  = credit.ToString();
            rpm.dmatterLabel.text = dmatter.ToString();

            PlayerDataBase.Instance.RaceProjectDataParsing(this, rpm);
            rpm.curRpd = this;

            if (!isNext)
                rpm.startBtn.gameObject.SetActive(false);
            else
                rpm.startBtn.gameObject.SetActive(true);
        }
    }

    public void OnRelease()
    {
        isNext = true;
        check.gameObject.SetActive(false);
        frame.spriteName = "Lab_Skill_Frame";
    }

    public void OnComplete()
    {
        if (isResearch)
        {
            isNext = false;
            check.gameObject.SetActive(true);
            frame.spriteName = "Lab_Skill_Frame Complete";
            check.spriteName = "Lab_Skill_Complete";
        }
    }

    public void OnUncomplete()
    {
        if (!isResearch)
        {
            isNext = false;
            check.gameObject.SetActive(true);
            frame.spriteName = "Lab_Skill_Frame";
            check.spriteName = "Captain_Trait_Lock";
        }
    }
}
