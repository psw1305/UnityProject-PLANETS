using System.Collections;
using UnityEngine;

public class PlayerSkillSetting_Terran : MonoBehaviour
{
    [HideInInspector] public float cooltime, dur, atk, ran, num;
    [HideInInspector] public bool isActive = false, isTime = true;
    public PlayerShipManager psm;
    public GameObject skillEffect;
    GameObject target;

    public void PlayerSkillDataParsing(string race, string type, string plus, int level)
    {
        var skillMasterTable = new MasterTablePlayer.MasterTablePlayer();
        skillMasterTable.Load();

        foreach (var skillMaster in skillMasterTable.All)
        {
            if (skillMaster.Race == race && skillMaster.Type == type && skillMaster.Plus == plus && skillMaster.Level == level)
            {
                cooltime = skillMaster.Cooltime;
                dur = skillMaster.DUR;
                atk = skillMaster.ATK;
                ran = skillMaster.RAN;
                num = skillMaster.NUM;
            }
        }
    }

    public void Init()
    {
        PlayerSkillDataParsing(psm.raceType.ToString(), psm.shipType.ToString(), psm.typePlus, psm.shipLevel);

        if (psm.shipType == PlayerShipManager.ShipType.Carrier)
        {
            if (isActive)
            {
                if (psm.typePlus == "N" || psm.typePlus == "A")
                {
                    for (int i = 0; i < psm.pt.fighters.Length; i++)
                    {
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletDur = dur;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletAtk = atk;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletNum = num;
                    }
                }
                else if (psm.typePlus == "B")
                {
                    for (int i = 0; i < psm.pt.fighters.Length; i++)
                    {
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().warp = true;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().shipSpeed = 20 * atk;

                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletDur = dur;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.damagePercent = atk;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletNum = num;
                    }
                }
            }
            else
            {
                for (int i = 0; i < psm.pt.fighters.Length; i++)
                {
                    psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletDur = 5;
                    psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletAtk = 0.5f;
                    psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletNum = 1;
                }
            }
        }
    }

    void Update()
    {
        if (isActive)
        {
            if (isTime)
                StartCoroutine("SkillCheck_Timer");
        }
    }

    // 구축함 A타입 => 항성간 탄도 미사일
    void Terran_Destroyer_A_ISBMBallista()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindRandomEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            float ranX = Random.Range(-15, 15); float ranY = Random.Range(-15, 15);

            GameObject ballista = Instantiate(skillEffect, new Vector3(-400, Random.Range(-400, 400), 0), transform.rotation) as GameObject;
            ballista.GetComponent<EnemyHitDamage>().destination = target.transform.position + new Vector3(ranX, ranY, 0);
            ballista.GetComponent<EnemyHitDamage>().explosion.transform.localScale = new Vector3(1.0f + ran, 1.0f + ran, 1.0f + ran);
            ballista.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().damage = psm.pt.bulletDamage * atk;
            ballista.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().perRadius += ran;
        }
    }

    // 구축함 B타입 => 특수 탄환
    void Terran_Destroyer_B_TransformWeapon()
    {
        psm.seg.EffectGenerator("Strong", dur, atk, ran, num);
    }

    // 지원함 A타입 => 긴급 수리
    void Terran_Auxiliary_A_EmergencyRepair()
    {
        if (psm.psmv.FindDamagedPlayer() != null)
            psm.psmv.FindDamagedPlayer().GetComponent<PlayerShipManager>().seg.EffectGenerator("Repair", dur, psm.pt.bulletDamage * atk * num, ran, num);
    }

    // 순양함 A타입 => 위상 탄환
    IEnumerator Terran_Cruiser_A_PhaseShell()
    {
        for (int i= 0; i < num; i++)
        {
            if (!psm.psmv.isTarget)
                target = psm.psmv.FindClosestEnemy();
            else
                target = psm.psmv.targeted;

            if (target != null)
            {
                GameObject phaseShell = Instantiate(skillEffect, psm.pt.turrets[0].position, transform.rotation) as GameObject;
                phaseShell.GetComponent<EnemyHitDamage>().destination = target.transform.position;
                phaseShell.GetComponent<EnemyHitDamage>().host = transform;
                phaseShell.GetComponent<EnemyHitDamage>().hostRadius = 300;
                phaseShell.GetComponent<EnemyHitDamage>().bulletDamage = psm.pt.bulletDamage * atk;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    // 순양함 B타입 => 차원 장애물
    void Terran_Cruiser_B_DimensionObstacle()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindRandomEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            EnemyShipManager esm = target.transform.parent.GetComponent<EnemyShipManager>();
            esm.eseg.EffectGenerator("Dimension", dur, atk, ran, num);
        }
    }

    // 전함 A타입 => 강화필드
    void Terran_Battleship_A_UpgradeField()
    {
        GameObject upgrade = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        upgrade.transform.parent = psm.transform;
        upgrade.GetComponent<PlayerAura>().dur = dur;
        upgrade.GetComponent<PlayerAura>().atk = atk;
    }

    // 전함 B타입 => 쿨다운필드
    void Terran_Battleship_B_CoolDownField()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindRandomEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            GameObject cooldown = Instantiate(skillEffect, target.transform.position, transform.rotation) as GameObject;
            cooldown.GetComponent<PlayerAura>().dur = dur;
            cooldown.GetComponent<PlayerAura>().atk = atk;
        }
    }

    IEnumerator SkillCheck_Timer()
    {
        isTime = false;
        yield return new WaitForSeconds(cooltime - Random.Range(-3, 4));

        if (isActive && psm.psmv.FindClosestEnemy() != null)
        {
            if (psm.typePlus == "N" || psm.typePlus == "A")
            {
                switch (psm.shipType)
                {
                    case PlayerShipManager.ShipType.Destroyer:
                        Terran_Destroyer_A_ISBMBallista();
                        break;
                    case PlayerShipManager.ShipType.Auxiliary:
                        Terran_Auxiliary_A_EmergencyRepair();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        StartCoroutine("Terran_Cruiser_A_PhaseShell");
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        Terran_Battleship_A_UpgradeField();
                        break;
                }
            }
            else if (psm.typePlus == "B")
            {
                switch (psm.shipType)
                {
                    case PlayerShipManager.ShipType.Destroyer:
                        StartCoroutine("Terran_Destroyer_B_TransformWeapon");
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        Terran_Cruiser_B_DimensionObstacle();
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        Terran_Battleship_B_CoolDownField();
                        break;
                }
            }
        }

        isTime = true;
    }
}
