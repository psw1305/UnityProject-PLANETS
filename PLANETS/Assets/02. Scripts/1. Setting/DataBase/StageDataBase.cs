using UnityEngine;

public class StageDataBase : Singleton<StageDataBase> 
{
    public void BuildingDataParsing(BuildingData bd, BuildingManager bm)
    {
        var buildMasterTable = new MasterTableStage.MasterTableFormula();
        buildMasterTable.Load();

        foreach (var buildMaster in buildMasterTable.All)
        {
            if (buildMaster.Type == "Building_" + bd.buildName)
            {
                bd.bm     = bm;
                bd.credit = (int)buildMaster.Value1;
                bd.core   = (int)buildMaster.Value2;
                bd.acore  = (int)buildMaster.Value4;
            }
        }
    }

    public void ConstructDataParsing(ConstructManager cm, string race, string type, string plus, int level)
    {
        var playerMasterTable = new MasterTablePlayer.MasterTablePlayer();
        playerMasterTable.Load();

        foreach (var playerMaster in playerMasterTable.All)
        {
            if (playerMaster.Race == race && playerMaster.Type == type && playerMaster.Plus == plus && playerMaster.Level == level)
            {
                cm.damage   = playerMaster.Damage;
                cm.health   = playerMaster.Health;
                cm.shield   = playerMaster.Shield;
                cm.maintain = playerMaster.Maintain;

                if (PlayerPrefs.HasKey("Building_DroneFactory"))
                {
                    cm.credit = (int)(playerMaster.Ship_Build_Credit * 0.8);
                    cm.core   = (int)(playerMaster.Ship_Build_Core * 0.8);
                }
                else
                {
                    cm.credit = playerMaster.Ship_Build_Credit;
                    cm.core   = playerMaster.Ship_Build_Core;
                }

                cm.nameLabel.text     = playerMaster.Name;
                cm.damageLabel.text   = cm.damage.ToString();
                cm.healthLabel.text   = cm.health.ToString();
                cm.shieldLabel.text   = cm.shield.ToString();
                cm.maintainLabel.text = cm.maintain.ToString();

                cm.skillIcon.spriteName = "Skill_" + race + "_" + type + "_" + playerMaster.Plus;
                cm.skillLevel.text      = playerMaster.Skill_Lv;
                cm.skillNameLabel.text  = playerMaster.Skill_Name;
                cm.skillInfoLabel.text  = playerMaster.Skill_Info;

                cm.creditLabel.text = cm.credit.ToString();
                cm.coreLabel.text   = cm.core.ToString();

                string dataPlus = PlayerPrefs.GetString("Research_" + cm.raceName + "_" + cm.shipType + "_Plus", "N");
                cm.blueprint.spriteName = "Base " + race + " " + dataPlus + " " + type;
            }
        }
    }

    public void BattleDataParsing(string id, int scale, int num, EnemyFleet ef)
    {
        var battleMasterTable = new MasterTableStage.MasterTableBattle();
        battleMasterTable.Load();

        foreach (var battleMaster in battleMasterTable.All)
        {
            if (battleMaster.UID == id + "_" + scale + "_" + num)
            {
                ef.destroyer  = battleMaster.Destroyer;
                ef.auxiliary  = battleMaster.Auxiliary;
                ef.cruiser    = battleMaster.Cruiser;
                ef.carrier    = battleMaster.Carrier;
                ef.battleship = battleMaster.Battleship;
            }
        }
    }

    public void StageDataParsing(string race, int uid, CampaignData cd)
    {
        var stageMasterTable = new MasterTableStage.MasterTableStageInfo();
        stageMasterTable.Load();

        foreach (var stageMaster in stageMasterTable.All)
        {
            if (stageMaster.UID == race + "_Stage_" + uid)
            {
                cd.stageLevel = stageMaster.Difficulty;
                cd.bonusSprite.spriteName = "Resource_" + stageMaster.Bonus_Name;
                cd.bonusLabel.text = "+" + stageMaster.Bonus_Value;
            }
        }
    }

    public void MissionRewardDataParsing(string type)
    {
        var rewardMasterTable = new MasterTableStage.MasterTableFormula();
        rewardMasterTable.Load();

        foreach (var rewardMaster in rewardMasterTable.All)
        {
            if (rewardMaster.Type == type)
            {
                float credit, core, dmatter;

                if (PlayerPrefs.HasKey("Building_ResourceRecycling"))
                {
                    credit  = rewardMaster.Value1 * 1.2f;
                    core    = rewardMaster.Value2 * 1.2f;
                    dmatter = rewardMaster.Value3 * 1.2f;
                }
                else
                {
                    credit  = rewardMaster.Value1;
                    core    = rewardMaster.Value2;
                    dmatter = rewardMaster.Value3;
                }

                PlayerPrefs.SetInt("MissionClear_Credit", (int)credit);
                PlayerPrefs.SetInt("MissionClear_Core", (int)core);
                PlayerPrefs.SetInt("MissionClear_DMatter", (int)dmatter);
            }
        }
    }

    public void CaptainExpDataParsing(int level, int stageID, StageMainPlanet smp)
    {
        var captainMasterTable = new MasterTableStage.MasterTableFormula();
        captainMasterTable.Load();

        foreach (var captainMaster in captainMasterTable.All)
        {
            if (captainMaster.Type == "Captain_Lv_Exp_" + level)
                smp.cLvExp = captainMaster.Value1;

            if (captainMaster.Type == "Stage_" + stageID)
                smp.cStageExp = captainMaster.Value1;
        }
    }

    public void CaptainNextExpDataParsing(int level, StageMainPlanet smp)
    {
        var captainMasterTable = new MasterTableStage.MasterTableFormula();
        captainMasterTable.Load();

        foreach (var captainMaster in captainMasterTable.All)
        {
            if (captainMaster.Type == "Captain_Lv_Exp_" + level)
                smp.cNextLvExp = captainMaster.Value1;
        }
    }

    public void CaptainSkillDataParsing(string skill, int level, ActiveSkillSystem ass)
    {
        var captainMasterTable = new MasterTableStage.MasterTableFormula();
        captainMasterTable.Load();

        foreach (var captainMaster in captainMasterTable.All)
        {
            if (captainMaster.Type == skill + "_" + level)
            {
                ass.value1 = captainMaster.Value1;
                ass.value2 = captainMaster.Value2;
            }
        }
    }
}