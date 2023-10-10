using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFleet : MonoBehaviour 
{
    [HideInInspector] public EnemyFleetFormation eff;
    [HideInInspector] public StageMainPlanet smp;
    [HideInInspector] public bool isNormal, isMission, disable = false;
    [HideInInspector] public string battle, level, spawnPosition;
    [HideInInspector] public int destroyer, auxiliary, cruiser, carrier, battleship;
    [HideInInspector] public List<GameObject> enemyShips = new List<GameObject>();

    [Header("Object")]
    public GameObject[] ship_N, ship_A, ship_B;
    public GameObject freighter;
    public GameObject boss;
    public GameObject[] mineral;
    GameObject ship;

    [Header("Spawn")]
    public Transform[] spawnEnemyPosition;

    int shipNumber;

    [HideInInspector] public float shipDelay = 0f;
    [HideInInspector] public bool warp = false;

    void Start()
	{
        eff    = GetComponent<EnemyFleetFormation>();
        eff.ef = this;

        if (isNormal)
            EnemyFleetGenerator();

        if (isMission)
            MissionEnemyFleetGenerator();
    }

    public void MissionClearCheck(int check)
    {
        shipNumber -= check;

        if (isNormal && shipNumber <= 0)
        {
            isNormal = false;
            smp.MissionClear();
        }
    }

    void EnemyFleetGenerator()
    {
        switch (level)
        {
            case "Beginner":
                eff.VerticalFormation(false, -18, "One", 2);
                eff.VerticalFormation(false, 18, "Two", 2);
                break;
            case "Normal":
                eff.VerticalFormation(false, -18, "One", 3);
                eff.VerticalFormation(false, 18, "Two", 3);
                break;
            case "Expert":
                eff.VerticalFormation(true, -36, "One", 2);
                eff.VerticalFormation(true, 0, "Two", 3);
                eff.VerticalFormation(true, 36, "Three", 2);
                break;
            case "Challenger":
                eff.VerticalFormation(true, -36, "One", 3);
                eff.VerticalFormation(true, 0, "Two", 2);
                eff.VerticalFormation(true, 36, "Three", 3);
                break;
            case "Master":
                eff.VerticalFormation(true, -36, "One", 3);
                eff.VerticalFormation(true, 0, "Two", 3);
                eff.VerticalFormation(true, 36, "Three", 3);
                break;
        }
    }

    void MissionEnemyFleetGenerator()
    {
        switch (smp.missionID)
        {
            case 2:
                GameObject mineralClone = Instantiate(mineral[Random.Range(0, 2)], transform.position, Quaternion.identity) as GameObject;
                mineralClone.transform.parent = transform;
                mineralClone.transform.position = transform.position;
                mineralClone.transform.rotation = transform.rotation;
                shipNumber += 1;
                break;
            case 3:
                GameObject fort = Instantiate(boss, transform.position, Quaternion.identity) as GameObject;
                fort.transform.parent = transform;
                fort.transform.position = transform.position;
                fort.transform.rotation = spawnEnemyPosition[0].rotation;

                fort.GetComponent<EnemyShipManager>().ef = gameObject.GetComponent<EnemyFleet>();
                fort.GetComponent<EnemyShipManager>().shipLevel = level;
                fort.GetComponent<EnemyShipManager>().isMission = isMission;
                fort.GetComponent<Mission_4_Fortress>().smp = smp;

                EnemyMovingType(fort);
                enemyShips.Add(fort);
                shipNumber += 1;
                break;
            case 4:
                GameObject enemy = Instantiate(boss, transform.position, Quaternion.identity) as GameObject;
                enemy.transform.parent = transform;
                enemy.transform.position = transform.position;
                enemy.transform.rotation = spawnEnemyPosition[0].rotation;

                enemy.GetComponent<EnemyBossManager>().ef   = GetComponent<EnemyFleet>();
                enemy.GetComponent<EnemyBossManager>().gage = smp.missionUI[4];

                EnemyMovingType(enemy);
                enemyShips.Add(enemy);
                shipNumber += 1;
                break;
        }
    }

    public void ShipCreate(bool isPlus, bool isMission, int shipID, Vector3 shipPos)
    {
        if (!isPlus)
            ship = ship_N[shipID];
        else
        {
            int ran = Random.Range(0, 2);

            switch (ran)
            {
                case 0:
                    ship = ship_A[shipID];
                    break;
                case 1:
                    ship = ship_B[shipID];
                    break;
            }
        }

        GameObject shipSpawn = Instantiate(ship, transform.position, Quaternion.identity) as GameObject;
        shipSpawn.transform.parent   = transform;
        shipSpawn.transform.position = shipPos;
        shipSpawn.transform.rotation = spawnEnemyPosition[0].rotation;

        shipSpawn.GetComponent<EnemyShipManager>().ef = GetComponent<EnemyFleet>();
        shipSpawn.GetComponent<EnemyShipManager>().shipLevel = level;
        shipSpawn.GetComponent<EnemyShipManager>().isMission = isMission;

        EnemyMovingType(shipSpawn);
        enemyShips.Add(shipSpawn);
        shipNumber += 1;

        shipSpawn.SetActive(false);
    }

    void EnemyMovingType(GameObject enemy)
    {
        switch (smp.missionID)
        {
            case 3:
                enemy.GetComponent<EnemyShipMoving>().movingType = EnemyShipMoving.MovingType.Defense;
                break;
        }
    }

    public void StartShip()
    {
        StartCoroutine("ActiveShip");
    }

    IEnumerator ActiveShip()
    {
        for (int i = 0; i < shipNumber; i++)
        {
            enemyShips[i].SetActive(true);
            enemyShips[i].GetComponent<EnemyShipManager>().ShipOnline();
            yield return new WaitForSeconds(0.1f);
        }
    }
}