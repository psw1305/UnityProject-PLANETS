using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageMainPlanet : MonoBehaviour
{
    [HideInInspector] public string level;
    [HideInInspector] public int mainRace, stageID, missionID, playerShipCnt;
    string enemy, mission;
    bool isClear = true, isFailed = false;

    [Header("Stage")]
    public GameObject playerFleet;
    public Transform PlayerPos, EnemyPos;
    public Transform MissionPlayerPos, MissionEnemyPos;

    [Header("Spawn")]
    public GameObject[] mainEnemys;
    public GameObject[] mainEnemyGrounds;
    public GameObject missionEnemy;
    public GameObject missionEnemyGround;

    [HideInInspector] public GameObject spawn, addSpawn;
    [HideInInspector] public List<GameObject> enemyFleets = new List<GameObject>();

    [Header("UI")]
    public GameObject[] missionUI;
    public Transform skillPanel;
    public UILabel skillNotice;

    [Header("UI: Result")]
    public UISprite captainImage;
    public UILabel captainName;
    public UILabel captainLevel;
    public TweenFill captainExp;

    [HideInInspector] public float cLvExp, cNextLvExp, cStageExp;
    [HideInInspector] public string cType, cName;
    [HideInInspector] public int cLv;
    float cExp, cDiv, cSum;

    [Header("Script")]
    public MissionPlayerData[] mpd;
    public RewardManager rm;
    public Mission_2_Defense md;
    [HideInInspector] public PlayerFleet pf;

    void Awake()
    {
        stageID = PlayerPrefs.GetInt("StageID");
        enemy   = PlayerPrefs.GetString("StartRace");
        mission = PlayerPrefs.GetString("Operation");
        level   = PlayerPrefs.GetString("Difficulty");

        cType = PlayerPrefs.GetString("InGame_Captain");
        cName = PlayerPrefs.GetString("InGame_CaptainName");
        cLv   = PlayerPrefs.GetInt("Captain_Level_" + cName);
        cExp  = PlayerPrefs.GetFloat("Captain_Exp_" + cName);

        StageDataBase.Instance.CaptainExpDataParsing(cLv, stageID, this);
        cDiv  = cExp % cLvExp;

        if (cName != "장교 선택창")
        {
            captainImage.spriteName = cType;
            captainName.text  = cName;
            captainLevel.text = "Lv." + cLv;
            captainExp.GetComponent<UISprite>().fillAmount = cDiv / cLvExp;
        }
        else
        {
            captainImage.spriteName = "Captain_Empty";
            captainName.text  = "장교없음";
            captainLevel.text = "";
            captainExp.GetComponent<UISprite>().fillAmount = 0;
        }

        switch (enemy)
        {
            case "Kalas":
                Instantiate(mainEnemyGrounds[0], transform.position, Quaternion.identity);
                mainRace = 0;
                break;
            case "ShadowFang":
                Instantiate(mainEnemyGrounds[1], transform.position, Quaternion.identity);
                mainRace = 1;
                break;
            case "Aridrian":
                Instantiate(mainEnemyGrounds[2], transform.position, Quaternion.identity);
                mainRace = 2;
                break;
            case "Harbinger":
                Instantiate(mainEnemyGrounds[3], transform.position, Quaternion.identity);
                mainRace = 3;
                break;
        }

        switch (mission)
        {
            case "Mission_1_공격":
                missionID = 1;
                missionUI[0].SetActive(true);
                break;
            case "Mission_2_방어":
                missionID = 2;
                missionUI[1].SetActive(true);
                break;
            case "Mission_3_확보":
                missionID = 3;
                missionUI[2].SetActive(true);
                break;
            case "Mission_4_습격":
                missionID = 4;
                missionUI[3].SetActive(true);
                break;
            case "Mission_5_보스":
                missionID = 5;
                missionUI[4].SetActive(true);
                break;
        }

        StageGenerator();
        CaptainSkillCreate();
    }

    void StageGenerator()
    {
        switch (missionID)
        {
            // 미션: 공격
            case 1:
                PlayerPos.position = new Vector3(0, 0, 0);
                SpawnSetting("PlayerFleet", 0);
                break;
            // 미션: 방어
            case 2:
                PlayerPos.position = new Vector3(-50, 0, 0);
                EnemyPos.position = new Vector3(200, 0, 0);
                MissionPlayerPos.position = new Vector3(-120, 0, 0);
                MissionEnemyPos.position = new Vector3(-200, 0, 0);

                SpawnSetting("PlayerFleet", 0);
                SpawnSetting("PlayerFleet_Mission", 3);
                SpawnSetting("EnemyFleet_Mission", 0);
                break;
            // 미션: 확보
            case 3:
                PlayerPos.position = new Vector3(-50, 0, 0);
                EnemyPos.position = new Vector3(200, 0, 0);
                MissionPlayerPos.position = new Vector3(-150, 0, 0);

                SpawnSetting("PlayerFleet", 0);
                SpawnSetting("PlayerFleet_Mission", 0);
                break;
            // 미션: 습격
            case 4:
                PlayerPos.position = new Vector3(-80, 0, 0);
                EnemyPos.position = new Vector3(180, 0, 0);
                MissionEnemyPos.position = new Vector3(160, 0, 0);

                SpawnSetting("PlayerFleet", 0);
                SpawnSetting("EnemyFleet_Mission", 0);
                break;
            // 미션: 보스
            case 5:
                PlayerPos.position = new Vector3(-80, 0, 0);
                EnemyPos.position = new Vector3(180, 0, 0);
                MissionEnemyPos.position = new Vector3(160, 0, 0);

                SpawnSetting("PlayerFleet", 0);
                SpawnSetting("EnemyFleet_Mission", 0);
                break;
        }
    }

    void CaptainSkillCreate()
    {
        GameObject skill = Resources.Load("Skill/" + cType + " (Toggle)") as GameObject;

        if (skill != null)
        {
            GameObject cSkill = Instantiate(skill, transform.position, Quaternion.identity) as GameObject;
            cSkill.transform.parent = skillPanel;
            cSkill.transform.localScale = new Vector3(1, 1, 1);
            cSkill.transform.localPosition = new Vector3(-528, -400, 0);

            cSkill.GetComponent<ActiveSkillSystem>().smp = this;
            cSkill.GetComponent<ActiveSkillSystem>().notice = skillNotice;
        }
    }

    void CaptainLevelExp()
    {
        if (cName != "장교 선택창")
        {
            if (cDiv + cStageExp < cLvExp)
            {
                captainExp.duration = 1.0f;
                captainExp.from = cDiv / cLvExp;
                captainExp.to = (cDiv + cStageExp) / cLvExp;
                captainExp.PlayForward();
            }
            else if (cDiv + cStageExp >= cLvExp)
            {
                cLv += 1;
                captainLevel.text = "Lv." + cLv;
                PlayerPrefs.SetInt("Captain_Level_" + cName, cLv);

                cSum = cDiv + cStageExp - cLvExp;
                StageDataBase.Instance.CaptainNextExpDataParsing(cLv, this);

                captainExp.duration = 0.5f;
                captainExp.from = cDiv / cLvExp;
                captainExp.to = 1.0f;
                captainExp.PlayForward();

                StartCoroutine("DelayTweenFill");

                while (cSum > cNextLvExp)
                {
                    cLv += 1; 
                    captainLevel.text = "Lv." + cLv;
                    PlayerPrefs.SetInt("Captain_Level_" + cName, cLv);

                    cSum -= cNextLvExp;
                    StageDataBase.Instance.CaptainNextExpDataParsing(cLv, this);

                    StartCoroutine("DelayTweenFill");
                }
            }

            PlayerPrefs.SetFloat("Captain_Exp_" + cName, cExp + cStageExp);
        }
    }

    void CaptainLevelUp()
    {

    }

    IEnumerator DelayTweenFill()
    {
        yield return new WaitForSeconds(0.5f);

        captainExp.ResetToBeginning();
        captainExp.from = 0;
        captainExp.to = cSum / cLvExp;
        captainExp.PlayForward();
    }

    public void TimeReset()
    {
        Time.timeScale = 1.0f;
    }

    public void SpawnSetting(string select, int fleetScale)
    {
        switch (select)
        {
            case "PlayerFleet":
                spawn = Instantiate(playerFleet) as GameObject;
                spawn.transform.parent = GameObject.FindGameObjectWithTag("PlayerPosition").transform;
                spawn.transform.position = PlayerPos.position;
                spawn.GetComponent<PlayerFleet>().isNormal = true;
                spawn.GetComponent<PlayerFleet>().isMission = false;
                spawn.GetComponent<PlayerFleet>().smp = this;
                pf = spawn.GetComponent<PlayerFleet>();
                break;
            case "PlayerFleet_Mission":
                spawn = Instantiate(playerFleet) as GameObject;
                spawn.transform.parent = GameObject.FindGameObjectWithTag("PlayerPosition").transform;
                spawn.transform.position = MissionPlayerPos.position;
                spawn.GetComponent<PlayerFleet>().isNormal = false;
                spawn.GetComponent<PlayerFleet>().isMission = true;
                spawn.GetComponent<PlayerFleet>().smp = this;
                break;
            case "EnemyFleet":
                spawn = Instantiate(mainEnemys[mainRace]) as GameObject;
                spawn.transform.parent = GameObject.FindGameObjectWithTag("EnemyPosition").transform;
                spawn.transform.position = EnemyPos.position;
                spawn.GetComponent<EnemyFleet>().isNormal = true;
                spawn.GetComponent<EnemyFleet>().isMission = false;
                spawn.GetComponent<EnemyFleet>().smp = this;
                spawn.GetComponent<EnemyFleet>().level = level;
                spawn.GetComponent<EnemyFleet>().spawnPosition = "EnemyPosition";
                enemyFleets.Add(spawn);
                break;
            case "EnemyFleet_Mission":
                spawn = Instantiate(mainEnemys[mainRace]) as GameObject;
                spawn.transform.parent = GameObject.FindGameObjectWithTag("EnemyPosition").transform;
                spawn.transform.position = MissionEnemyPos.position;
                spawn.GetComponent<EnemyFleet>().isNormal = false;
                spawn.GetComponent<EnemyFleet>().isMission = true;
                spawn.GetComponent<EnemyFleet>().smp = this;
                spawn.GetComponent<EnemyFleet>().level = level;
                spawn.GetComponent<EnemyFleet>().spawnPosition = "EnemyPosition";
                enemyFleets.Add(spawn);
                break;
        }
    }

    public void MissionClear()
    {
        if (isClear)
        {
            Time.timeScale = 0.3f;
            isFailed = true;

            string stageRace = PlayerPrefs.GetString("StartRace");
            int stageID = PlayerPrefs.GetInt("StageID");
            PlayerPrefs.SetInt(stageRace + "_Stage_" + stageID, 1);

            rm.MissionClearReward();
            StartCoroutine("MissionResult");
        }
    }

    public void MissionFailed()
    {
        if (!isFailed)
        {
            Time.timeScale = 0.3f;
            isClear = false;

            rm.MissionFailedReward();
            StartCoroutine("MissionResult");
        }
    }

    IEnumerator MissionResult()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < playerShipCnt; i++)
            mpd[i].ResultData();

        CaptainLevelExp();
    }

    public void ReturnLobby()
    {
        NextChangeScene.Instance.NextLobbyScene();
    }

    public void BattleAgain()
    {
        NextChangeScene.Instance.NextGameScene();
    }
}