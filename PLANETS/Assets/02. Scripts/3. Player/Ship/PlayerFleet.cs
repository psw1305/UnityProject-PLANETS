using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerFleet : MonoBehaviour
{
    GameObject player, prefab;
    int shipNumber;

    [Header("Object: Battle")]
    public GameObject[] terran;
    public GameObject[] kalas, shadowFang, aridrian, harbinger;

    [Header("Object: Type.A")]
    public GameObject[] terran_A;
    public GameObject[] kalas_A, shadowFang_A, aridrian_A, harbinger_A;

    [Header("Object: Type.B")]
    public GameObject[] terran_B;
    public GameObject[] kalas_B, shadowFang_B, aridrian_B, harbinger_B;

    [Header("Object: Mission")]
    public GameObject[] traderShips;
    public GameObject ancientWeapon, miningShip;

    [Header("Control")]
    public Transform[] moveTargets;
    public Transform defenseTarget;
    
    [HideInInspector] public List<GameObject> playerShips = new List<GameObject>();
    [HideInInspector] public int missionShipCheck = 0;
    [HideInInspector] public bool isNormal = false, isMission = false, isStart = true;
    [HideInInspector] public StageMainPlanet smp;

    void Start()
    {
        if (isNormal)
        {
            moveTargets[0].localPosition = new Vector3(36, 36, 0);
            moveTargets[1].localPosition = new Vector3(36, 0, 0);
            moveTargets[2].localPosition = new Vector3(36, -36, 0);

            moveTargets[3].localPosition = new Vector3(0, 54, 0);
            moveTargets[4].localPosition = new Vector3(0, 18, 0);
            moveTargets[5].localPosition = new Vector3(0, -18, 0);
            moveTargets[6].localPosition = new Vector3(0, -54, 0);

            moveTargets[7].localPosition = new Vector3(-36, 36, 0);
            moveTargets[8].localPosition = new Vector3(-36, 0, 0);
            moveTargets[9].localPosition = new Vector3(-36, -36, 0);

            PlayerFleetGenerator();
        }

        if (isMission)
            MissionPlayerFleetGenerator();
    }

    public void MissionFailedCheck(int check)
    {
        shipNumber -= check;

        if (isNormal && shipNumber <= 0)
        {
            isNormal = false;
            smp.MissionFailed();
        }
    }

    void PlayerFleetGenerator()
    {
        for (int i = 0; i < 10; i++)
        {
            string raceName = PlayerPrefs.GetString("PlayerRace_" + i);
            string shipType = PlayerPrefs.GetString("PlayerType_" + i);
            string shipName = PlayerPrefs.GetString("PlayerName_" + i);

            int shipLevel   = PlayerPrefs.GetInt("ShipLevel_" + shipName);
            string typePlus = PlayerPrefs.GetString("ShipTypePlus_" + shipName);

            switch (shipType)
            {
                case "Destroyer":
                    PlayerShipRace(raceName, typePlus, 0);
                    PlayerShipSetting(i, shipLevel, shipName);
                    break;
                case "Auxiliary":
                    PlayerShipRace(raceName, typePlus, 1);
                    PlayerShipSetting(i, shipLevel, shipName);
                    break;
                case "Cruiser":
                    PlayerShipRace(raceName, typePlus, 2);
                    PlayerShipSetting(i, shipLevel, shipName);
                    break;
                case "Carrier":
                    PlayerShipRace(raceName, typePlus, 3);
                    PlayerShipSetting(i, shipLevel, shipName);
                    break;
                case "Battleship":
                    PlayerShipRace(raceName, typePlus, 4);
                    PlayerShipSetting(i, shipLevel, shipName);
                    break;
            }
        }
    }

    void PlayerShipRace(string race, string plus, int num)
    {
        switch (race)
        {
            case "Terran":
                if (plus == "N")
                    prefab = terran[num];
                else if (plus == "A")
                    prefab = terran_A[num];
                else if (plus == "B")
                    prefab = terran_B[num];
                break;
            case "Kalas":
                if (plus == "N")
                    prefab = kalas[num];
                else if (plus == "A")
                    prefab = kalas_A[num];
                else if (plus == "B")
                    prefab = kalas_B[num];
                break;
            case "ShadowFang":
                if (plus == "N")
                    prefab = shadowFang[num];
                else if (plus == "A")
                    prefab = shadowFang_A[num];
                else if (plus == "B")
                    prefab = shadowFang_B[num];
                break;
            case "Aridrian":
                if (plus == "N")
                    prefab = aridrian[num];
                else if (plus == "A")
                    prefab = aridrian_A[num];
                else if (plus == "B")
                    prefab = aridrian_B[num];
                break;
            case "Harbinger":
                if (plus == "N")
                    prefab = harbinger[num];
                else if (plus == "A")
                    prefab = harbinger_A[num];
                else if (plus == "B")
                    prefab = harbinger_B[num];
                break;
        }
    }

    void PlayerShipSetting(int num, int level, string shipName)
    {
        player = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        player.transform.parent = transform;
        player.transform.position = moveTargets[num].position;

        PlayerShipManager psm = player.GetComponent<PlayerShipManager>();
        psm.pf = gameObject.GetComponent<PlayerFleet>();
        psm.startPosition = player.transform.position;

        psm.shipPosID = num;
        psm.shipLevel = level;
        psm.shipName  = shipName;

        PlayerMovingType(player);
        playerShips.Add(player);

        smp.mpd[shipNumber].DataParsing(level, psm.raceType.ToString(), psm.shipType.ToString(), psm.typePlus, shipName);
        shipNumber += 1;
        smp.playerShipCnt = shipNumber;
    }

    void MissionPlayerFleetGenerator()
    {
        switch (smp.missionID)
        {
            case 2:
                int posY = 35;

                for (int i = 0; i < 3; i++)
                {
                    missionShipCheck += 1;
                    player = Instantiate(miningShip, transform.position, Quaternion.identity) as GameObject;
                    player.transform.parent = transform.parent;
                    player.transform.position = new Vector3(defenseTarget.position.x, defenseTarget.position.y + posY, 0);

                    smp.md.pme[i] = player.GetComponent<PlayerMiningEffect>();
                    player.GetComponent<PlayerShipManager>().pf = GetComponent<PlayerFleet>();
                    player.GetComponent<PlayerShipManager>().startPosition = player.transform.position;
                    player.GetComponent<PlayerShipManager>().isMission = true;
                    player.GetComponent<PlayerShipMoving>().movingType = PlayerShipMoving.MovingType.None;

                    playerShips.Add(player);
                    shipNumber += 1;
                    posY -= 35;
                }
                break;
            case 3:
                missionShipCheck += 1;

                player = Instantiate(ancientWeapon, transform.position, Quaternion.identity) as GameObject;
                player.transform.parent   = transform.parent;
                player.transform.position = defenseTarget.position;
                
                player.GetComponent<PlayerShipManager>().pf = GetComponent<PlayerFleet>();
                player.GetComponent<PlayerShipManager>().startPosition = player.transform.position;
                player.GetComponent<PlayerShipManager>().isMission = true;

                PlayerMovingType(player);
                playerShips.Add(player);
                break;
        }
    }

    void PlayerMovingType(GameObject player)
    {
        switch (smp.missionID)
        {
            case 1:
                player.GetComponent<PlayerShipMoving>().movingType = PlayerShipMoving.MovingType.Normal;
                break;
            case 2:
                player.GetComponent<PlayerShipMoving>().movingType = PlayerShipMoving.MovingType.Defense;
                break;
            case 3:
                player.GetComponent<PlayerShipMoving>().movingType = PlayerShipMoving.MovingType.Defense;
                break;
            case 4:
                player.GetComponent<PlayerShipMoving>().movingType = PlayerShipMoving.MovingType.Normal;
                break;
            case 5:
                player.GetComponent<PlayerShipMoving>().movingType = PlayerShipMoving.MovingType.Normal;
                break;
        }
    }

    public void PlayerShipRemove(GameObject player)
    {
        playerShips.Remove(player);
    }

    public void ShipOnline()
    {
        for (int i = 0; i < playerShips.Count; i++)
        {
            if (playerShips[i].GetComponent<PlayerShipManager>().shipLevel >= 2)
                playerShips[i].GetComponent<PlayerShipManager>().RaceBalanceSystem(true);
            else
                playerShips[i].GetComponent<PlayerShipManager>().RaceBalanceSystem(false);
        }
    }

    public void Warp()
    {
        for (int i = 0; i < playerShips.Count; i++)
        {
            playerShips[i].transform.position = playerShips[i].GetComponent<PlayerShipManager>().startPosition;
            playerShips[i].transform.rotation = transform.rotation;
        }

        StartCoroutine("ShipWarp");
    }

    IEnumerator ShipWarp()
    {
        yield return new WaitForSeconds(1);
        Camera.main.GetComponent<CameraManager>().OnTouch = true;

        for (int i = 0; i < playerShips.Count; i++)
        {
            playerShips[i].SetActive(true);
            playerShips[i].GetComponent<PlayerShipManager>().core.SetActive(true);
            playerShips[i].GetComponent<PlayerShipManager>().gage.SetActive(true);
            playerShips[i].GetComponent<PlayerShipManager>().indicatorClone.SetActive(true);
            playerShips[i].GetComponent<PlayerShipManager>().pt.isEnable = true;

            if (playerShips[i].GetComponent<PlayerShipManager>().skillBtn != null)
            {
                GameObject skill = playerShips[i].GetComponent<PlayerShipManager>().skillBtn;
                skill.GetComponent<BoxCollider>().enabled = true;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ShipEscape()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < playerShips.Count; i++)
        {
            playerShips[i].GetComponent<PlayerShipManager>().core.SetActive(false);
            playerShips[i].GetComponent<PlayerShipManager>().gage.SetActive(false);
            playerShips[i].GetComponent<PlayerShipManager>().indicatorClone.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void BattleEnd()
    {
        Camera.main.GetComponent<CameraManager>().OnTouch = false;

        for (int i = 0; i < playerShips.Count; i++)
        {
            playerShips[i].GetComponent<PlayerShipManager>().pt.isEnable = false;

            if (playerShips[i].GetComponent<PlayerShipManager>().skillBtn != null)
            {
                GameObject skill = playerShips[i].GetComponent<PlayerShipManager>().skillBtn;
                skill.GetComponent<UIToggle>().value = false;
                skill.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
