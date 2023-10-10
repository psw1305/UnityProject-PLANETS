public class PlayerDataBase : Singleton<PlayerDataBase>
{
    public void PlayerBaseDataParsing(string race, string type, PlayerShipManager psm)
    {
        var shipMasterTable = new MasterTableStage.MasterTableShip();
        shipMasterTable.Load();

        foreach (var shipMaster in shipMasterTable.All)
        {
            if (shipMaster.Race == race && shipMaster.Type == type)
            {
                psm.pt.turretFireTime = shipMaster.Firerate;
                psm.shieldTime        = shipMaster.ShieldTime;
                psm.psmv.shipSpeed    = shipMaster.Moving;
                psm.psmv.turnSpeed    = shipMaster.TurnMoving;
                psm.shipMp            = shipMaster.Crew;
            }
        }
    }

    public void PlayerStatDataParsing(string race, string type, string plus, int level, PlayerShipManager psm)
    {
        var playerMasterTable = new MasterTablePlayer.MasterTablePlayer();
        playerMasterTable.Load();

        foreach (var playerMaster in playerMasterTable.All)
        {
            if (playerMaster.Race == race && playerMaster.Type == type && playerMaster.Plus == plus && playerMaster.Level == level)
            {
                psm.pt.bulletDamage = playerMaster.Damage;
                psm.shipHp          = playerMaster.Health;
                psm.shipAp          = playerMaster.Shield;
            }
        }
    }

    public void CaptainDataParsing(string type, int num, CaptainManager data)
    {
        var captainMasterTable = new MasterTablePlayer.MasterTableCaptain();
        captainMasterTable.Load();

        foreach (var captainMaster in captainMasterTable.All)
        {
            if (captainMaster.Captain_Type == type + num)
            {
                data.cType = captainMaster.Captain_Type;
                data.cName = captainMaster.Captain_Name;
                data.cRace = captainMaster.Captain_Race;

                data.sType = captainMaster.Skill_Type;
                data.sName = captainMaster.Skill_Name;
                data.sInfo = captainMaster.Skill_Info;

                data.captainImage.spriteName = data.cType;
                data.captainName.text = data.cName;

                data.skillIcon.spriteName = data.sType;
                data.skillName.text = data.sName;
                data.skillInfo.text = data.sInfo;

                data.Init();
            }
        }
    }

    public void RepairDataParsing(ManageData md, string state)
    {
        var repairMasterTable = new MasterTableStage.MasterTableFormula();
        repairMasterTable.Load();

        foreach (var repairMaster in repairMasterTable.All)
        {
            if (repairMaster.Type == "Repair_Ship_" + state + "_" + md.dataLv)
            {
                md.rCredit = repairMaster.Value1;
                md.rCore = repairMaster.Value2;
            }
        }
    }

    public void UpgradeDataParsing(ManageData md, int curLv, int nextLv, string curPlus, string nextPlus)
    {
        var upgradeMasterTable = new MasterTablePlayer.MasterTablePlayer();
        upgradeMasterTable.Load();

        foreach (var upgradeMaster in upgradeMasterTable.All)
        {
            if (upgradeMaster.Race == md.dataRace && upgradeMaster.Type == md.dataType)
            {
                if (upgradeMaster.Level == curLv && upgradeMaster.Plus == curPlus)
                {
                    md.cCredit = upgradeMaster.Ship_Build_Credit;
                    md.cCore   = upgradeMaster.Ship_Build_Core;
                }

                if (upgradeMaster.Level == nextLv && upgradeMaster.Plus == nextPlus)
                {
                    md.uCredit = upgradeMaster.Ship_Build_Credit;
                    md.uCore   = upgradeMaster.Ship_Build_Core;
                }
            }
        }
    }

    public void ProjectDataParsing(ProjectData pd, ProjectManager pm)
    {
        var projectMasterTable = new MasterTablePlayer.MasterTablePlayer();
        projectMasterTable.Load();

        foreach (var projectMaster in projectMasterTable.All)
        {
            if (projectMaster.Race == pm.raceName && projectMaster.Type == pm.shipType && projectMaster.Plus == pd.plus && projectMaster.Level == pd.level)
            {
                pm.credit = projectMaster.Ship_Research_Credit;
                pm.dmatter = projectMaster.Ship_Research_DarkMatter;

                pm.shipImage.spriteName = "Base " + pm.raceName + " " + pd.plus + " " + pm.shipType;
                pm.shipInfo.text = projectMaster.Name + pd.plus + " Lv." + pd.level;

                pm.nextShipStats[0].text = projectMaster.Health.ToString();
                pm.nextShipStats[1].text = projectMaster.Shield.ToString();
                pm.nextShipStats[2].text = projectMaster.Damage.ToString();

                pm.skillName.text = projectMaster.Skill_Name;
                pm.skilllnfo.text = projectMaster.Skill_Info;

                pm.creditLabel.text = pm.credit.ToString();
                pm.dmatterLabel.text = pm.dmatter.ToString();

                pm.lvCheck = pd.level;
                pm.shipPlus = pd.plus;

                pm.skillID = projectMaster.Plus;
                pm.skillLv = projectMaster.Skill_Lv;
                pm.curPd = pd;
            }
        }
    }

    public void RaceProjectResourceDataParsing(RaceProjectData rpd)
    {
        var formulaMasterTable = new MasterTableStage.MasterTableFormula();
        formulaMasterTable.Load();

        foreach (var formulaMaster in formulaMasterTable.All)
        {
            if (formulaMaster.Type == "Species_" + rpd.level)
            {
                rpd.credit  = (int)formulaMaster.Value1;
                rpd.dmatter = (int)formulaMaster.Value3;
            }
        }
    }

    public void RaceProjectDataParsing(RaceProjectData rpd, RaceProjectManager rpm)
    {
        var projectMasterTable = new MasterTablePlayer.MasterTablePlayer();
        projectMasterTable.Load();

        foreach (var projectMaster in projectMasterTable.All)
        {
            if (projectMaster.Race == rpm.raceName && projectMaster.Type == rpd.shipType && projectMaster.Plus == rpd.shipPlus)
            {
                rpm.shipImage.spriteName = "Base " + rpm.raceName + " " + rpd.shipPlus + " " + rpd.shipType;
                rpm.shipInfo.text = rpd.shipType + " " + rpd.shipPlus + "타입";

                rpm.skillImage.spriteName = "Skill_" + rpm.raceName + "_" + rpd.shipType + "_" + rpd.shipPlus;
                rpm.skillName.text = projectMaster.Skill_Name;
                rpm.skillInfo.text = projectMaster.Skill_Info;
            }
        }
    }
}
