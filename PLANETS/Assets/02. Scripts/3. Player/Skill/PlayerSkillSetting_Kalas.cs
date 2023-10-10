using System.Collections;
using UnityEngine;

public class PlayerSkillSetting_Kalas : MonoBehaviour
{
    [HideInInspector] public float cooltime, dur, atk, ran, num;
    [HideInInspector] public bool isActive = false, isTime = true;
    public PlayerShipManager psm;
    public GameObject skillEffect;
    public Transform[] skillPos;
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
                // 항공모함 A타입 => 레드와인 가스
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
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletDur = 5;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletAtk = 0.5f;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletNum = 1;
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

    // 구축함 A타입 => 셸 쇼크
    IEnumerator Kalas_Destroyer_A_ShellShock()
    {
        for (int i = 0; i < num; i++)
        {
            if (psm.psmv.FindClosestEnemy() != null)
            {
                float ranX = Random.Range(-12, 13); float ranY = Random.Range(-12, 13);

                GameObject torpedo = Instantiate(skillEffect, skillPos[i].position, transform.rotation) as GameObject;
                torpedo.GetComponent<EnemyHitDamage>().destination = psm.psmv.FindClosestEnemy().transform.position + new Vector3(ranX, ranY, 0);
                torpedo.GetComponent<EnemyHitDamage>().bulletDamage = 0;

                torpedo.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().damage = psm.pt.bulletDamage * 0.1f;
                torpedo.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().dur = dur;
                torpedo.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().atk = atk;
                torpedo.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().ran = ran;
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
            canister.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().dur = dur;
            canister.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().atk = atk;
            canister.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().ran = ran;

            yield return new WaitForSeconds(1.2f / num);
        }
    }

    // 지원함 A타입 => 진지 사수
    void Kalas_Auxiliary_A_HoldTheFort()
    {
        if (psm.psmv.FindDamagedPlayer() != null && !psm.psmv.FindDamagedPlayer().GetComponent<PlayerShipManager>().isDestroy)
            psm.psmv.FindDamagedPlayer().GetComponent<PlayerShipManager>().seg.EffectGenerator("Protect", dur, atk, ran, num);
    }

    // 지원함 B타입 => 스테이시스 치료
    void Kalas_Auxiliary_B_StasisField()
    {
        if (psm.psmv.FindDamagedPlayer() != null && !psm.psmv.FindDamagedPlayer().GetComponent<PlayerShipManager>().isDestroy)
            psm.psmv.FindDamagedPlayer().GetComponent<PlayerShipManager>().seg.EffectGenerator("Stasis", dur, atk, ran, num);
    }

    // 순양함 A타입 => 기절 탄환
    void Kalas_Cruiser_A_HighPowerPulseCannon()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindClosestEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            GameObject stunball = Instantiate(skillEffect, psm.pt.turrets[0].position, transform.rotation) as GameObject;
            stunball.transform.localScale = new Vector3(1 + ran, 1 + ran, 1 + ran);
            stunball.GetComponent<CrowdControl>().dur = dur;

            stunball.GetComponent<EnemyHitDamage>().destination  = target.transform.position;
            stunball.GetComponent<EnemyHitDamage>().host         = transform;
            stunball.GetComponent<EnemyHitDamage>().hostRadius   = 300;
            stunball.GetComponent<EnemyHitDamage>().bulletDamage = psm.pt.bulletDamage * atk;
        }
    }

    // 순양함 A타입 => 공성 모드
    IEnumerator Kalas_Cruiser_B_SiegeMode()
    {
        for (int i = 0; i < num; i++)
        {
            if (psm.psmv.FindRandomEnemy() != null)
            {
                float ranX = Random.Range(-8, 9);
                float ranY = Random.Range(-8, 9);
                Vector3 destination = psm.psmv.FindRandomEnemy().transform.position + new Vector3(ranX, ranY, 0);

                GameObject shell = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
                shell.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().damage = psm.pt.bulletDamage * atk;

                yield return new WaitForSeconds(dur / num);
            }
        }
    }

    // 항공모함 B타입 => 베테랑
    void Kalas_Carrier_B_Veteran()
    {
        for (int i = 0; i < psm.pt.fighters.Length; i++)
        {
            if (psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().upgrade < num)
            {
                psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().upgrade += 1;
                psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.damagePercent += atk;
                psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().effect.Play();
            }
        }
    }

    // 전함 A타입 => 요새화
    void Kalas_Battleship_A_DefenseLine()
    {
        psm.seg.EffectGenerator("DefenseUp2", dur, atk, ran, num);
    }

    // 전함 B타입 => 회피장막
    void Kalas_Battleship_B_EvadeField()
    {
        GameObject evade = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        evade.transform.parent = psm.transform;
        evade.GetComponent<PlayerAura>().dur = dur;
        evade.GetComponent<PlayerAura>().atk = atk;
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
                        StartCoroutine("Kalas_Destroyer_A_ShellShock");
                        break;
                    case PlayerShipManager.ShipType.Auxiliary:
                        Kalas_Auxiliary_A_HoldTheFort();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        Kalas_Cruiser_A_HighPowerPulseCannon();
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        Kalas_Battleship_A_DefenseLine();
                        break;
                }
            }
            else if (psm.typePlus == "B")
            {
                switch (psm.shipType)
                {
                    case PlayerShipManager.ShipType.Destroyer:
                        StartCoroutine("Kalas_Destroyer_B_TrenchWarfare");
                        break;
                    case PlayerShipManager.ShipType.Auxiliary:
                        Kalas_Auxiliary_B_StasisField();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        StartCoroutine("Kalas_Cruiser_B_SiegeMode");
                        break;
                    case PlayerShipManager.ShipType.Carrier:
                        Kalas_Carrier_B_Veteran();
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        Kalas_Battleship_B_EvadeField();
                        break;
                }
            }
        }

        isTime = true;
    }
}
