using UnityEngine;

public class RaceListName : MonoBehaviour
{
    public RaceListSelect rls;
    public string raceName;
    public UISprite raceFrame;
    public UILabel raceLabel;

    public void SymbolChange(string rName)
    {
        raceName = rName;

        switch (rName)
        {
            case "Terran":
                raceLabel.text = "테란";
                break;
            case "Kalas":
                raceLabel.text = "칼라스";
                break;
            case "ShadowFang":
                raceLabel.text = "쉐도우팽";
                break;
            case "Aridrian":
                raceLabel.text = "에이드리언";
                break;
            case "Harbinger":
                raceLabel.text = "하빈저";
                break;
        }

        raceFrame.spriteName = "Race_Select_" + rName;
        GetComponent<UIButton>().normalSprite = "Race_Select_" + rName + " On";
    }

    public void ClickMeRace()
    {
        rls.SymbolRaceChange(raceName);
    }
}
