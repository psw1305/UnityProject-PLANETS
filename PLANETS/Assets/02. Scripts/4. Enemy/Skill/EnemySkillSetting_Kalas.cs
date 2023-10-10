using System.Collections;
using UnityEngine;

public class EnemySkillSetting_Kalas : MonoBehaviour
{
    [HideInInspector] public float cooltime, timePlus, dur, atk, ran, num;
    [HideInInspector] public bool isActive = false, isTime = true;
    public EnemyShipManager esm;
    public GameObject skillEffect;
    public Transform[] skillPos;

    public void EnemySkillDataParsing(string race, string type, string plus, string level)
    {
        var skillMasterTable = new MasterTableEnemy.MasterTableEnemy();
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
        timePlus = 0;
        EnemySkillDataParsing(esm.raceType.ToString(), esm.shipType.ToString(), esm.typePlus, esm.shipLevel);

        if (esm.shipType == EnemyShipManager.ShipType.Carrier)
        {
            if (isActive)
            {
                // 항공모함 A타입 => 레드와인 가스
                if (esm.typePlus == "N" || esm.typePlus == "A")
                {
                    for (int i = 0; i < esm.et.fighters.Length; i++)
                    {
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletDur = dur;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletAtk = atk;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletNum = num;
                    }
                }
                else if (esm.typePlus == "B")
                {
                    for (int i = 0; i < esm.et.fighters.Length; i++)
                    {
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletDur = 5;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletAtk = 0.5f;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletNum = 1;
                    }
                }
            }
            else
            {
                for (int i = 0; i < esm.et.fighters.Length; i++)
                {
                    esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletDur = 5;
                    esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletAtk = 0.5f;
                    esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletNum = 1;
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

    // 구축함 A타입 => 셸 쇼크
    IEnumerator Kalas_Destroyer_A_ShellShock()
    {
        for (int i = 0; i < num; i++)
        {
            if (esm.esmv.FindClosestPlayer() != null)
            {
                float ranX = Random.Range(-12, 13); float ranY = Random.Range(-12, 13);

                GameObject torpedo = Instantiate(skillEffect, skillPos[i].position, transform.rotation) as GameObject;
                torpedo.GetComponent<PlayerHitDamage>().destination = esm.esmv.FindClosestPlayer().transform.position + new Vector3(ranX, ranY, 0);
                torpedo.GetComponent<PlayerHitDamage>().bulletDamage = 0;

                torpedo.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * 0.1f;
                torpedo.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().dur = dur;
                torpedo.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().atk = atk;
                torpedo.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().ran = ran;
            }

            yield return new WaitForSeconds(1.2f / num);
        }
    }

    // 구축함 B타입 => 참호전
    IEnumerator Kalas_Destroyer_B_TrenchWarfare()
    {
        for (int i = 0; i < num; i++)
        {
            GameObject canister = Instantiate(skillEffect, skillPos[i].position, skillPos[i].rotation) as GameObject;
            canister.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().dur = dur;
            canister.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().atk = atk;
            canister.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().ran = ran;

            yield return new WaitForSeconds(1.2f / num);
        }
    }

    // 지원함 A타입 => 진지 사수
    void Kalas_Auxiliary_A_HoldTheFort()
    {
        if (esm.esmv.FindDamagedEnemy() != null && !esm.esmv.FindDamagedEnemy().GetComponent<EnemyShipManager>().isDestroy)
            esm.esmv.FindDamagedEnemy().GetComponent<EnemyShipManager>().eseg.EffectGenerator("Protect", dur, atk, ran, num);
    }

    // 지원함 B타입 => 스테이시스 치료
    void Kalas_Auxiliary_B_StasisField()
    {
        if (esm.esmv.FindDamagedEnemy() != null && !esm.esmv.FindDamagedEnemy().GetComponent<EnemyShipManager>().isDestroy)
            esm.esmv.FindDamagedEnemy().GetComponent<EnemyShipManager>().eseg.EffectGenerator("Stasis", dur, atk, ran, num);
    }

    // 순양함 A타입 => 기절 탄환
    void Kalas_Cruiser_A_HighPowerPulseCannon()
    {
        if (esm.esmv.FindClosestPlayer())
        {
            GameObject stunball = Instantiate(skillEffect, esm.et.turrets[0].position, transform.rotation) as GameObject;
            stunball.transform.localScale = new Vector3(1 + ran, 1 + ran, 1 + ran);
            stunball.GetComponent<PlayerHitDamage>().destination = esm.esmv.FindClosestPlayer().transform.position;
            stunball.GetComponent<PlayerHitDamage>().host = transform;
            stunball.GetComponent<PlayerHitDamage>().hostRadius = 300;
            stunball.GetComponent<PlayerHitDamage>().bulletDamage = esm.et.bulletDamage * atk;
            stunball.GetComponent<CrowdControl>().dur = dur;
        }
    }

    // 순양함 A타입 => 공성 모드
    IEnumerator Kalas_Cruiser_B_SiegeMode()
    {
        for (int i = 0; i < num; i++)
        {
            if (esm.esmv.FindRandomPlayer() != null)
            {
                float ranX = Random.Range(-8, 9);
                float ranY = Random.Range(-8, 9);
                Vector3 destination = esm.esmv.FindRandomPlayer().transform.position + new Vector3(ranX, ranY, 0);

                GameObject shell = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
                shell.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * atk;

                yield return new WaitForSeconds(dur / num);
            }
        }
    }

    // 항공모함 B타입 => 베테랑
    void Kalas_Carrier_B_Veteran()
    {
        for (int i = 0; i < esm.et.fighters.Length; i++)
        {
            if (esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().upgrade < num)
            {
                esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().upgrade += 1;
                esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.damagePercent += atk;
                esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().effect.Play();
            }
        }
    }

    // 전함 A타입 => 요새화
    void Kalas_Battleship_A_DefenseLine()
    {
        esm.eseg.EffectGenerator("DefenseUp2", dur, atk, ran, num);
    }

    // 전함 B타입 => 회피장막
    void Kalas_Battleship_B_EvadeField()
    {
        GameObject evade = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        evade.transform.parent = esm.transform;
        evade.GetComponent<EnemyAura>().dur = dur;
        evade.GetComponent<EnemyAura>().atk = atk;
    }

    IEnumerator SkillCheck_Timer()
    {
        isTime = false;
        yield return new WaitForSeconds(cooltime - Random.Range(-3, 4) + timePlus);

        if (isActive && esm.esmv.FindClosestPlayer() != null)
        {
            if (esm.typePlus == "N" || esm.typePlus == "A")
            {
                switch (esm.shipType)
                {
                    case EnemyShipManager.ShipType.Destroyer:
                        StartCoroutine("Kalas_Destroyer_A_ShellShock");
                        break;
                    case EnemyShipManager.ShipType.Auxiliary:
                        Kalas_Auxiliary_A_HoldTheFort();
                        break;
                    case EnemyShipManager.ShipType.Cruiser:
                        Kalas_Cruiser_A_HighPowerPulseCannon();
                        break;
                    case EnemyShipManager.ShipType.Battleship:
                        Kalas_Battleship_A_DefenseLine();
                        break;
                }
            }
            else if (esm.typePlus == "B")
            {
                switch (esm.shipType)
                {
                    case EnemyShipManager.ShipType.Destroyer:
                        StartCoroutine("Kalas_Destroyer_B_TrenchWarfare");
                        break;
                    case EnemyShipManager.ShipType.Auxiliary:
                        Kalas_Auxiliary_B_StasisField();
                        break;
                    case EnemyShipManager.ShipType.Cruiser:
                        StartCoroutine("Kalas_Cruiser_B_SiegeMode");
                        break;
                    case EnemyShipManager.ShipType.Carrier:
                        Kalas_Carrier_B_Veteran();
                        break;
                    case EnemyShipManager.ShipType.Battleship:
                        Kalas_Battleship_B_EvadeField();
                        break;
                }
            }
        }

        isTime = true;
    }
}
