using System.Collections;
using UnityEngine;

public class EnemySkillSetting_Harbinger : MonoBehaviour
{
    [HideInInspector] public float cooltime, timePlus, dur, atk, ran, num;
    [HideInInspector] public bool isActive = false, isTime = false;
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
                // 항공모함 A타입 => 안개
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

    // 구축함 A타입 => 정전기 필드
    void Harbinger_Destroyer_A_ElectricShock()
    {
        if (esm.esmv.FindClosestPlayer() != null)
        {
            float ranX = Random.Range(-8, 8); float ranY = Random.Range(-8, 8);
            Vector3 destination = esm.esmv.FindClosestPlayer().transform.position + new Vector3(ranX, ranY, 0);

            GameObject shock = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
            shock.GetComponent<EffectManager>().effectTime = dur;
            shock.GetComponent<EffectManager>().Effect();

            shock.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * atk;
            shock.GetComponent<EnemyAura>().dur = dur;
            shock.GetComponent<EnemyAura>().atk = atk;
            shock.GetComponent<EnemyAura>().ran = ran;
        }
    }

    // 구축함 B타입 => 쉴드 변환
    void Harbinger_Destroyer_B_ShieldConversion()
    {
        if (esm.esmv.FindClosestPlayer() != null)
        {
            PlayerShipManager psm = esm.esmv.FindClosestPlayer().GetComponentInParent<PlayerShipManager>();
            psm.seg.EffectGenerator("ShieldDown", 0, 0, 0, 1);

            int cmp = (int)(psm.shipOriginAp / 1000 * atk);
            esm.eseg.EffectGenerator("Protect", dur, 0, 0, cmp);
        }
    }

    // 지원함 A타입 => 세례
    void Harbinger_Auxiliary_A_Baptism()
    {
        GameObject baptism = Instantiate(skillEffect, transform.position, transform.rotation);
        baptism.GetComponent<EnemyAura>().atk = esm.et.bulletDamage * atk;
    }

    // 지원함 B타입 => 전기 구름
    void Harbinger_Auxiliary_B_ThunderCloud()
    {
        GameObject cloud = Instantiate(skillEffect, transform.position, transform.rotation);
        cloud.GetComponent<EnemyAura>().dur = dur;
        cloud.GetComponent<EnemyAura>().atk = atk;
    }

    // 순양함 A타입 => 전도
    void Harbinger_Cruiser_A_Evangelize()
    {
        GameObject chain = Instantiate(skillEffect, esm.et.turrets[0].position, transform.rotation);
        chain.GetComponent<ChainAttack>().damage = esm.et.bulletDamage * atk;
    }

    // 순양함 B타입 => 전기 창
    void Harbinger_Cruiser_B_LightningSpear()
    {
        GameObject spear = Instantiate(skillEffect, skillPos[0].position, transform.rotation) as GameObject;
        spear.GetComponent<EnemyAura>().isBox = true;
        spear.GetComponent<EnemyAura>().boxSize = new Vector2(100, 10);
        spear.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * atk;
    }

    // 항공모함 B타입 => 쉴드 드론
    IEnumerator Harbinger_Carrier_B_ShieldDrone()
    {
        for (int i = 0; i < esm.et.fighters.Length; i++)
        {
            esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().shield = true;
            esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().effect.Play();
        }

        yield return new WaitForSeconds(dur);

        for (int i = 0; i < esm.et.fighters.Length; i++)
        {
            esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().shield = false;
            esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().effect.Stop();
        }
    }

    // 전함 A타입 => 신벌
    void Harbinger_Battleship_A_DivinePunishment()
    {
        float ranX = Random.Range(-10, 11); float ranY = Random.Range(-10, 11);
        GameObject target = esm.esmv.FindRandomPlayer();

        if (target != null)
        {
            Vector3 destination = new Vector3(target.transform.position.x + ranX, target.transform.position.y + ranY, 0);

            GameObject blackhole = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
            blackhole.transform.localScale = new Vector3(1.0f + ran, 1.0f + ran, 1.0f + ran);

            blackhole.GetComponent<EnemyAura>().dur = dur;
            blackhole.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * atk;
            blackhole.GetComponent<EnemyAura>().perRadius += ran;

            blackhole.GetComponent<EnemyShipMoving>().battleRadius = 0;
            blackhole.GetComponent<EnemyShipMoving>().shipSpeed = 3;
            blackhole.GetComponent<EnemyShipMoving>().shipOriginSpeed = 3;
            blackhole.GetComponent<EnemyShipMoving>().turnSpeed = 15;
        }
    }

    // 항공모함 B타입 => 복수
    IEnumerator Harbinger_Battleship_B_Revenge()
    {
        esm.ehb.isRevenge = true;
        esm.ehb.revengeDamage = 0;
        esm.eseg.EffectTimeInstance("RevengeCharge", dur);

        yield return new WaitForSeconds(dur);

        esm.ehb.isRevenge = false;
        
        if (esm.esmv.FindClosestPlayer() != null)
        {
            Vector3 destination = esm.esmv.FindClosestPlayer().transform.position;

            GameObject revenge = Instantiate(skillEffect, skillPos[0].position, transform.rotation) as GameObject;
            revenge.GetComponent<PlayerHitDamage>().destination = destination;
            revenge.GetComponent<PlayerHitDamage>().bulletDamage = esm.ehb.revengeDamage * atk;
        }
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
                        Harbinger_Destroyer_A_ElectricShock();
                        break;
                    case EnemyShipManager.ShipType.Auxiliary:
                        Harbinger_Auxiliary_A_Baptism();
                        break;
                    case EnemyShipManager.ShipType.Cruiser:
                        Harbinger_Cruiser_A_Evangelize();
                        break;
                    case EnemyShipManager.ShipType.Battleship:
                        Harbinger_Battleship_A_DivinePunishment();
                        break;
                }
            }
            else if (esm.typePlus == "B")
            {
                switch (esm.shipType)
                {
                    case EnemyShipManager.ShipType.Destroyer:
                        Harbinger_Destroyer_B_ShieldConversion();
                        break;
                    case EnemyShipManager.ShipType.Auxiliary:
                        Harbinger_Auxiliary_B_ThunderCloud();
                        break;
                    case EnemyShipManager.ShipType.Cruiser:
                        Harbinger_Cruiser_B_LightningSpear();
                        break;
                    case EnemyShipManager.ShipType.Carrier:
                        StartCoroutine("Harbinger_Carrier_B_ShieldDrone");
                        break;
                    case EnemyShipManager.ShipType.Battleship:
                        StartCoroutine("Harbinger_Battleship_B_Revenge");
                        break;
                }
            }
        }

        isTime = true;
    }
}
