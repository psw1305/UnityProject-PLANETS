using UnityEngine;

public class RaceListSelect : MonoBehaviour
{
    bool isCheck = false;

    [Header("Ship")]
    public UISprite[] shipBases;
    public UISprite[] shipClick;
    public UILabel[] shipNames;

    [Header("UI")]
    public TweenAlpha blind;
    public RaceListName main;
    public RaceListName[] others;

    [Header("Script")]
    public ConstructManager cm;
    public ProjectManager pm;
    public MainBackButton mbb;

    void Update()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isCheck && mbb.currentNum == 1)
            {
                isCheck = false;
                mbb.currentNum -= 1;

                blind.PlayForward();
                blind.GetComponent<Collider>().enabled = false;

                for (int i = 0; i < others.Length; i++)
                {
                    others[i].raceFrame.GetComponent<TweenAlpha>().PlayReverse();
                    others[i].GetComponent<TweenPosition>().PlayReverse();
                    others[i].GetComponent<Collider>().enabled = false;
                }
            }
        }
        #endif
    }

    public void RaceClick()
    {
        if (!isCheck)
        {
            isCheck = true;
            mbb.currentNum += 1;

            blind.PlayReverse();
            blind.GetComponent<Collider>().enabled = true;

            for (int i = 0; i < others.Length; i++)
            {
                others[i].raceFrame.GetComponent<TweenAlpha>().PlayForward();
                others[i].GetComponent<TweenPosition>().PlayForward();
                others[i].GetComponent<Collider>().enabled = true;
            }
        }
        else if (isCheck)
        {
            isCheck = false;
            mbb.currentNum -= 1;

            blind.PlayForward();
            blind.GetComponent<Collider>().enabled = false;

            for (int i = 0; i < others.Length; i++)
            {
                others[i].raceFrame.GetComponent<TweenAlpha>().PlayReverse();
                others[i].GetComponent<TweenPosition>().PlayReverse();
                others[i].GetComponent<Collider>().enabled = false;
            }
        }
    }

    public void SymbolRaceChange(string rName)
    {
        shipBases[0].spriteName = "Base " + rName + " N Destroyer";
        shipBases[1].spriteName = "Base " + rName + " N Auxiliary";
        shipBases[2].spriteName = "Base " + rName + " N Cruiser";
        shipBases[3].spriteName = "Base " + rName + " N Carrier";
        shipBases[4].spriteName = "Base " + rName + " N Battleship";

        shipClick[0].spriteName = "Base " + rName + " N Destroyer";
        shipClick[1].spriteName = "Base " + rName + " N Auxiliary";
        shipClick[2].spriteName = "Base " + rName + " N Cruiser";
        shipClick[3].spriteName = "Base " + rName + " N Carrier";
        shipClick[4].spriteName = "Base " + rName + " N Battleship";

        switch (rName)
        {
            case "Terran":
                main.SymbolChange("Terran");
                others[0].SymbolChange("Kalas");
                others[1].SymbolChange("ShadowFang");
                others[2].SymbolChange("Aridrian");
                others[3].SymbolChange("Harbinger");

                shipNames[0].text = "Gladius";
                shipNames[1].text = "Scutum";
                shipNames[2].text = "Pilum";
                shipNames[3].text = "Centurion";
                shipNames[4].text = "Spatha";
                break;
            case "Kalas":
                main.SymbolChange("Kalas");
                others[0].SymbolChange("Terran");
                others[1].SymbolChange("ShadowFang");
                others[2].SymbolChange("Aridrian");
                others[3].SymbolChange("Harbinger");

                shipNames[0].text = "Titan";
                shipNames[1].text = "Colossus";
                shipNames[2].text = "Cyclops";
                shipNames[3].text = "Argos";
                shipNames[4].text = "Hekaton";
                break;
            case "ShadowFang":
                main.SymbolChange("ShadowFang");
                others[0].SymbolChange("Terran");
                others[1].SymbolChange("Kalas");
                others[2].SymbolChange("Aridrian");
                others[3].SymbolChange("Harbinger");

                shipNames[0].text = "Hound";
                shipNames[1].text = "Teeth";
                shipNames[2].text = "Claw";
                shipNames[3].text = "Wolfpack";
                shipNames[4].text = "Fenrir";
                break;
            case "Aridrian":
                main.SymbolChange("Aridrian");
                others[0].SymbolChange("Terran");
                others[1].SymbolChange("Kalas");
                others[2].SymbolChange("ShadowFang");
                others[3].SymbolChange("Harbinger");

                shipNames[0].text = "Shark";
                shipNames[1].text = "Remora";
                shipNames[2].text = "Karken";
                shipNames[3].text = "Behemoth";
                shipNames[4].text = "Leviathan";
                break;
            case "Harbinger":
                main.SymbolChange("Harbinger");
                others[0].SymbolChange("Terran");
                others[1].SymbolChange("Kalas");
                others[2].SymbolChange("ShadowFang");
                others[3].SymbolChange("Aridrian");

                shipNames[0].text = "Spark";
                shipNames[1].text = "Conductor";
                shipNames[2].text = "Lightning";
                shipNames[3].text = "Torrent";
                shipNames[4].text = "Storm";
                break;
        }

        if (cm != null)
            cm.HangarShipListChange(rName);

        if (pm != null)
            pm.ProjectShipListChange(rName);

        isCheck = false;
        mbb.currentNum -= 1;

        blind.PlayForward();
        blind.GetComponent<Collider>().enabled = false;

        for (int i = 0; i < others.Length; i++)
        {
            others[i].raceFrame.GetComponent<TweenAlpha>().PlayReverse();
            others[i].GetComponent<TweenPosition>().PlayReverse();
            others[i].GetComponent<Collider>().enabled = false;
        }
    }
}
