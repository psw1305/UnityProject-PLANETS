public class EnemyDataBase : Singleton<EnemyDataBase>
{
    public void EnemyBaseDataParsing(string race, string type, EnemyShipManager esm)
    {
        var shipMasterTable = new MasterTableStage.MasterTableShip();
        shipMasterTable.Load();

        foreach (var shipMaster in shipMasterTable.All)
        {
            if (shipMaster.Race == race && shipMaster.Type == type)
            {
                esm.et.turretFireTime = shipMaster.Firerate;
                esm.shieldTime        = shipMaster.ShieldTime;
                esm.esmv.shipSpeed    = shipMaster.Moving;
                esm.esmv.turnSpeed    = shipMaster.TurnMoving;
                esm.shipMp            = shipMaster.Crew;
            }
        }
    }

    public void EnemyStatDataParsing(string race, string type, string plus, string level, EnemyShipManager esm)
    {
        var enemyMasterTable = new MasterTableEnemy.MasterTableEnemy();
        enemyMasterTable.Load();

        foreach (var enemyMaster in enemyMasterTable.All)
        {
            if (enemyMaster.Race == race && enemyMaster.Type == type && enemyMaster.Plus == plus && enemyMaster.Level == level)
            {
                esm.et.bulletDamage = enemyMaster.Damage;
                esm.shipHp          = enemyMaster.Health;
                esm.shipAp          = enemyMaster.Shield;
            }
        }
    }

    public void BossStatDataParsing(string race, string type, string plus, string level, EnemyBossManager ebm)
    {
        var enemyMasterTable = new MasterTableEnemy.MasterTableEnemy();
        enemyMasterTable.Load();

        foreach (var enemyMaster in enemyMasterTable.All)
        {
            if (enemyMaster.Race == race && enemyMaster.Type == type && enemyMaster.Plus == plus && enemyMaster.Level == level)
            {
                for (int i = 0; i < ebm.et.Length; i++)
                    ebm.et[i].bulletDamage = enemyMaster.Damage;
                ebm.shipAp = enemyMaster.Shield;
                ebm.shipHp = enemyMaster.Health;
            }
        }
    }
}
