using System.Collections;
using UnityEngine;

public class EnemySkillSetting_Aridrian : MonoBehaviour
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
                // 항공모함 A타입 => 방어력 감소
                if (esm.typePlus == "N" || esm.typePlus == "A")
                {
                    for (int i = 0; i < esm.et.fighters.Length; i++)
                    {
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletDur = dur;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletAtk = atk;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.bulletNum = num;
                    }
                }
                // 항공모함 B타입 => 함재기 포격모드
                else if (esm.typePlus == "B")
                {
                    for (int i = 0; i < esm.et.fighters.Length; i++)
                    {
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.turretFireTime = dur;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.damagePercent  = atk;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().et.turretSensor   = num;
                        esm.et.fighters[i].GetComponent<EnemyFighterShipManager>().bomber = true;
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

    // 구축함 A타입 => 장갑타격 윈더바페
    void Aridrian_Destroyer_A_Wunderwaffe()
    {
        for (int i = 0; i < num; i++)
        {
            if (esm.esmv.FindRandomPlayer() != null)
            {
                GameObject wunderwaffe = Instantiate(skillEffect, new Vector3(250, Random.Range(-400, 400), 0), transform.rotation) as GameObject;
                wunderwaffe.GetComponent<PlayerHitDamage>().destination  = esm.esmv.FindRandomPlayer().transform.position;
                wunderwaffe.GetComponent<PlayerHitDamage>().bulletDamage = 0;
                wunderwaffe.GetComponent<CrowdControl>().atk = atk;
            }
        }
    }

    // 구축함 B타입 => 함재기 요격 레이더
    void Aridrian_Destroyer_B_Radar()
    {
        GameObject radar = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        radar.transform.parent = transform;
        radar.GetComponent<RadarSystem>().delay = atk;
    }

    // 지원함 A타입 => 치유드론 아네모네
    void Aridrian_Auxiliary_A_Anemone()
    {
        GameObject anemone = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        anemone.GetComponent<EffectManager>().Effect();
        anemone.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * atk;
        anemone.GetComponent<EnemyAura>().dur    = dur;
    }

    // 지원함 B타입 => 공격속도 증가 필드
    void Aridrian_Auxiliary_B_RapidField()
    {
        GameObject rapid = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        rapid.transform.parent = esm.transform;
        rapid.GetComponent<EnemyAura>().dur = dur;
        rapid.GetComponent<EnemyAura>().atk = atk;
    }

    // 순양함 A타입 => 미사일 풀 버스트
    IEnumerator Aridrian_Cruiser_A_FullBurst()
    {
        for (int i = 0; i < num; i++)
        {
            int mod = i % 4;
            GameObject hornet = Instantiate(skillEffect, skillPos[mod].position, skillPos[mod].rotation) as GameObject;
            hornet.GetComponent<PlayerHitDamage>().bulletDamage = esm.et.bulletDamage * atk; 
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 순양함 B타입 => 화염필드 그릴피쉬
    IEnumerator Aridrian_Cruiser_B_GrillFish()
    {
        if (esm.esmv.FindRandomPlayer() != null)
        {
            float posY = -15;
            Transform random = esm.esmv.FindClosestPlayer().transform;

            for (int i = 0; i < num; i++)
            {
                if (random != null)
                {
                    GameObject grillfish = Instantiate(skillEffect, skillPos[i].position, skillPos[i].rotation) as GameObject;
                    grillfish.GetComponent<PlayerHitDamage>().destination = random.position + new Vector3(0, posY, 0);
                    grillfish.GetComponent<PlayerHitDamage>().explosionTime = dur;
                    grillfish.GetComponent<PlayerHitDamage>().explosion.transform.localScale = new Vector3(1.0f + ran, 1.0f + ran, 1.0f + ran);
                    grillfish.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().damage = esm.et.bulletDamage * atk;
                    grillfish.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().dur = dur;
                    grillfish.GetComponent<PlayerHitDamage>().explosion.GetComponent<EnemyAura>().perRadius += ran;
                }

                posY += 30;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // 전함 A타입 => 방어드론 젤리피쉬
    void Aridrian_Battleship_A_JellyFish()
    {
        Vector3 buildPos = new Vector3(transform.position.x - 20, transform.position.y, 0);
        GameObject drone = Instantiate(skillEffect, buildPos, transform.rotation);
        drone.GetComponent<DefenseSystem>().damageCount = num;
    }

    // 전함 B타입 => 방어미사일 웨일즈로켓
    void Aridrian_Battleship_B_WhaleRocket()
    {
        if (esm.esmv.FindClosestPlayer() != null)
        {
            GameObject rocket = Instantiate(skillEffect, skillPos[0].position, transform.rotation);
            rocket.GetComponent<PlayerHitDamage>().destination  = esm.esmv.FindClosestPlayer().transform.position;
            rocket.GetComponent<PlayerHitDamage>().bulletDamage = esm.shipOriginHp * 0.2f;
            rocket.GetComponent<ObjectHitBox>().objectHp        = esm.shipOriginHp * atk;
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
                        Aridrian_Destroyer_A_Wunderwaffe();
                        break;
                    case EnemyShipManager.ShipType.Auxiliary:
                        Aridrian_Auxiliary_A_Anemone();
                        break;
                    case EnemyShipManager.ShipType.Cruiser:
                        StartCoroutine("Aridrian_Cruiser_A_FullBurst");
                        break;
                    case EnemyShipManager.ShipType.Battleship:
                        Aridrian_Battleship_A_JellyFish();
                        break;
                }
            }
            else if (esm.typePlus == "B")
            {
                switch (esm.shipType)
                {
                    case EnemyShipManager.ShipType.Destroyer:
                        Aridrian_Destroyer_B_Radar();
                        break;
                    case EnemyShipManager.ShipType.Auxiliary:
                        Aridrian_Auxiliary_B_RapidField();
                        break;
                    case EnemyShipManager.ShipType.Cruiser:
                        StartCoroutine("Aridrian_Cruiser_B_GrillFish");
                        break;
                    case EnemyShipManager.ShipType.Battleship:
                        Aridrian_Battleship_B_WhaleRocket();
                        break;

                }
            }
        }

        isTime = true;
    }
}
