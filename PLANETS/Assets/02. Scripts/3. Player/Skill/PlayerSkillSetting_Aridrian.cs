using System.Collections;
using UnityEngine;

public class PlayerSkillSetting_Aridrian : MonoBehaviour
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
                // 항공모함 A타입 => 방어력 감소
                if (psm.typePlus == "N" || psm.typePlus == "A")
                {
                    for (int i = 0; i < psm.pt.fighters.Length; i++)
                    {
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletDur = dur;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletAtk = atk;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletNum = num;
                    }
                }
                // 항공모함 B타입 => 함재기 포격모드
                else if (psm.typePlus == "B")
                {
                    for (int i = 0; i < psm.pt.fighters.Length; i++)
                    {
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.turretFireTime = dur;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.damagePercent  = atk;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.turretSensor   = num;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().bomber = true;
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

    // 구축함 A타입 => 장갑타격 윈더바페
    void Aridrian_Destroyer_A_Wunderwaffe()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindRandomEnemy();
        else
            target = psm.psmv.targeted;

        for (int i = 0; i < num; i++)
        {
            if (target != null)
            {
                GameObject wunderwaffe = Instantiate(skillEffect, new Vector3(-250, Random.Range(-400, 400), 0), transform.rotation) as GameObject;
                wunderwaffe.GetComponent<EnemyHitDamage>().destination = target.transform.position;
                wunderwaffe.GetComponent<EnemyHitDamage>().bulletDamage = 0;
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
        anemone.GetComponent<PlayerAura>().damage = psm.pt.bulletDamage * atk;
        anemone.GetComponent<PlayerAura>().dur = dur;
    }

    // 지원함 B타입 => 공격속도 증가 필드
    void Aridrian_Auxiliary_B_RapidField()
    {
        GameObject rapid = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        rapid.transform.parent = psm.transform;
        rapid.GetComponent<PlayerAura>().dur = dur;
        rapid.GetComponent<PlayerAura>().atk = atk;
    }

    // 순양함 A타입 => 미사일 풀 버스트
    IEnumerator Aridrian_Cruiser_A_FullBurst()
    {
        for (int i = 0; i < num; i++)
        {
            int mod = i % 4;
            GameObject hornet = Instantiate(skillEffect, skillPos[mod].position, skillPos[mod].rotation) as GameObject;
            hornet.GetComponent<EnemyHitDamage>().bulletDamage = psm.pt.bulletDamage * atk;
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 순양함 B타입 => 화염필드 그릴피쉬
    IEnumerator Aridrian_Cruiser_B_GrillFish()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindClosestEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            float posY = -15;
            Transform random = target.transform;

            for (int i = 0; i < num; i++)
            {
                if (random != null)
                {
                    GameObject grillfish = Instantiate(skillEffect, skillPos[i].position, skillPos[i].rotation) as GameObject;
                    grillfish.GetComponent<EnemyHitDamage>().destination   = random.position + new Vector3(0, posY, 0);
                    grillfish.GetComponent<EnemyHitDamage>().explosionTime = dur;
                    grillfish.GetComponent<EnemyHitDamage>().explosion.transform.localScale = new Vector3(1.0f + ran, 1.0f + ran, 1.0f + ran);
                    grillfish.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().damage     = psm.pt.bulletDamage * atk;
                    grillfish.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().dur        = dur;
                    grillfish.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().perRadius += ran;
                }

                posY += 30;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // 전함 A타입 => 방어드론 젤리피쉬
    void Aridrian_Battleship_A_JellyFish()
    {
        Vector3 buildPos = new Vector3(transform.position.x + 20, transform.position.y, 0);
        GameObject drone = Instantiate(skillEffect, buildPos, transform.rotation);
        drone.GetComponent<DefenseSystem>().damageCount = num;
    }

    // 전함 B타입 => 방어미사일 웨일즈로켓
    void Aridrian_Battleship_B_WhaleRocket()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindClosestEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            GameObject rocket = Instantiate(skillEffect, skillPos[0].position, transform.rotation);
            rocket.GetComponent<EnemyHitDamage>().destination  = target.transform.position;
            rocket.GetComponent<EnemyHitDamage>().bulletDamage = psm.shipOriginHp * 0.2f;
            rocket.GetComponent<ObjectHitBox>().objectHp = psm.shipOriginHp * atk;
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
                        Aridrian_Destroyer_A_Wunderwaffe();
                        break;
                    case PlayerShipManager.ShipType.Auxiliary:
                        Aridrian_Auxiliary_A_Anemone();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        StartCoroutine("Aridrian_Cruiser_A_FullBurst");
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        Aridrian_Battleship_A_JellyFish();
                        break;
                }
            }
            else if (psm.typePlus == "B")
            {
                switch (psm.shipType)
                {
                    case PlayerShipManager.ShipType.Destroyer:
                        Aridrian_Destroyer_B_Radar();
                        break;
                    case PlayerShipManager.ShipType.Auxiliary:
                        Aridrian_Auxiliary_B_RapidField();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        StartCoroutine("Aridrian_Cruiser_B_GrillFish");
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        Aridrian_Battleship_B_WhaleRocket();
                        break;

                }
            }
        }

        isTime = true;
    }
}
