using System.Collections;
using UnityEngine;

public class EnemySkillSetting_ShadowFang : MonoBehaviour
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
                // 항공모함 A타입 => 공격력 약화
                if (esm.typePlus == "N" || esm.typePlus == "A")
                {
                    for (int i = 0; i < esm.et.fighters.Length; i++)
                    {
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletDur = dur;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletAtk = atk;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletNum = num;
                    }
                }
                // 항공모함 B타입 => 자폭모드
                else if (esm.typePlus == "B")
                {
                    esm.et.damagePercent = atk;
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
        else if (esm.shipType == EnemyShipManager.ShipType.Auxiliary)
        {
            if (isActive)
            {
                // 지원함 A타입 => 자폭모드
                if (esm.typePlus == "N" || esm.typePlus == "A")
                {
                    esm.selfDestruct = true;
                    esm.atk = atk;
                    esm.ran = ran;
                    esm.num = num;
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

    // 구축함 A타입 => 플레임 쉴드
    void ShadowFang_Destroyer_A_FlameShield()
    {
        esm.eseg.EffectGenerator("DefenseUP2", dur, atk, ran, num);

        GameObject flame = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        flame.transform.parent = transform;
        flame.transform.localScale = new Vector3(1 + ran, 1 + ran, 1 + ran);

        flame.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * 0.1f;
        flame.GetComponent<EnemyAura>().dur = dur;
        flame.GetComponent<EnemyAura>().num = 1;
        flame.GetComponent<EnemyAura>().perRadius = ran;
    }
    
    // 구축함 B타입 => 우주 기뢰
    IEnumerator ShadowFang_Destroyer_B_SpaceMine()
    {
        for (int i = 0; i < num; i++)
        {
            if (esm.esmv.FindClosestPlayer() != null)
            {
                float ranNum = Random.Range(16, 20);
                Vector3 destination = esm.esmv.FindClosestPlayer().transform.position + new Vector3(Mathf.Cos(Random.Range(-75, 75)) * ranNum, Mathf.Sin(Random.Range(-75, 75)) * ranNum, 0);

                GameObject mine = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
                mine.GetComponent<PlayerHitDamage>().destination = esm.esmv.FindClosestPlayer().transform.position;
                mine.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().dur = dur;
                mine.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().atk = esm.et.bulletDamage * atk;
                mine.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().num = num;
            }         

            yield return new WaitForSeconds(0.3f);
        }
    }

    // 지원함 B타입 => 제압탄환사격
    void ShadowFang_Auxiliary_B_SuppressiveFire()
    {
        GameObject target = esm.esmv.FindClosestPlayer();

        if (target != null)
        {
            GameObject firezone = Instantiate(skillEffect, target.transform.position, transform.rotation) as GameObject;
            firezone.GetComponent<EnemyAura>().atk = atk;
            firezone.GetComponent<EnemyAura>().dur = dur;
            firezone.GetComponent<EnemyAura>().num = num;

            GetComponentInChildren<SubEnemyTurret>().destination = target.transform.position;
            GetComponentInChildren<SubEnemyTurret>().SubTurretActive();
        }
    }

    // 순양함 A타입 => 천공 드릴
    IEnumerator ShadowFang_Cruiser_A_DragonRising()
    {
        if (esm.esmv.FindClosestPlayer() != null)
        {
            esm.eseg.EffectTimeInstance("Strong", dur);

            esm.et.bulletAmmos = 5;
            esm.et.damagePercent -= 0.5f;
            esm.esmv.isTarget = true;
            esm.esmv.targeted = esm.esmv.FindClosestPlayer();

            yield return new WaitForSeconds(dur / 2);

            if (esm.esmv.targeted != null)
                esm.esmv.targeted.GetComponentInParent<PlayerShipManager>().seg.EffectGenerator("Overwhelm", dur / 2, atk, ran, num);
            else
            {
                esm.esmv.isTarget = false;
                esm.esmv.targeted = null;
            }

            yield return new WaitForSeconds(dur / 2);

            esm.et.bulletAmmos = 1;
            esm.et.damagePercent += 0.5f;
            esm.esmv.isTarget = false;
            esm.esmv.targeted = null;
        }
    }

    // 순양함 B타입 => 부착형 트랩
    void ShadowFang_Cruiser_B_StickyTrap()
    {
        if (esm.esmv.FindRandomPlayer() != null)
        {
            GameObject trap = Instantiate(skillEffect) as GameObject;
            trap.transform.parent = esm.esmv.FindRandomPlayer().transform.parent;
            trap.transform.localPosition = Vector3.zero;
            trap.GetComponent<EnemyAura>().atk = esm.et.bulletDamage * atk;
            trap.GetComponent<EnemyAura>().dur = dur;
            trap.GetComponent<EnemyAura>().num = num;
        }
    }

    // 전함 A타입 => 폭죽
    void ShadowFang_Battleship_A_Firework()
    {
        if (esm.esmv.FindRandomPlayer() != null)
        {
            float rangeX = Random.Range(-15, 15); float rangeY = Random.Range(-15, 15);
            Vector3 destination = esm.esmv.FindRandomPlayer().transform.position + new Vector3(rangeX, rangeY, 0);

            GameObject firework = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
            firework.GetComponent<EffectManager>().effectTime = dur;
            firework.GetComponent<EffectManager>().Effect();

            firework.GetComponent<EnemyAura>().dur = dur;
            firework.GetComponent<EnemyAura>().num = num;
        }
    }

    // 전함 B타입 => 화염자취
    IEnumerator ShadowFang_Battleship_B_FireBreathe()
    {
        esm.esmv.battleRadius = 0;
        esm.esmv.shipSpeed    = 9;
        esm.esmv.movePercent += 1.0f;
        esm.esmv.turnSpeed    = 0;
        esm.esmv.booster.SetActive(true);

        for (int i = 0; i < num; i++)
        {
            GameObject breathe = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
            breathe.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * atk;
            breathe.GetComponent<EnemyAura>().dur = dur;

            yield return new WaitForSeconds(dur / num);
        }

        esm.esmv.battleRadius = 80;
        esm.esmv.movePercent -= 1.0f;
        esm.esmv.turnSpeed    = 12;
        esm.esmv.booster.SetActive(false);
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
                        ShadowFang_Destroyer_A_FlameShield();
                        break;
                    case EnemyShipManager.ShipType.Cruiser:
                        StartCoroutine("ShadowFang_Cruiser_A_DragonRising");
                        break;
                    case EnemyShipManager.ShipType.Battleship:
                        ShadowFang_Battleship_A_Firework();
                        break;
                }
            }
            else if (esm.typePlus == "B")
            {
                switch (esm.shipType)
                {
                    case EnemyShipManager.ShipType.Destroyer:
                        StartCoroutine("ShadowFang_Destroyer_B_SpaceMine");
                        break;
                    case EnemyShipManager.ShipType.Auxiliary:
                        ShadowFang_Auxiliary_B_SuppressiveFire();
                        break;
                    case EnemyShipManager.ShipType.Cruiser:
                        ShadowFang_Cruiser_B_StickyTrap();
                        break;
                    case EnemyShipManager.ShipType.Battleship:
                        StartCoroutine("ShadowFang_Battleship_B_FireBreathe");
                        break;
                }
            }
        }

        isTime = true;
    }
}
