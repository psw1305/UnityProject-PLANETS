using System.Collections;
using UnityEngine;

public class PlayerSkillSetting_Harbinger : MonoBehaviour
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
                // 항공모함 A타입 => 안개
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

    // 구축함 A타입 => 정전기 필드
    void Harbinger_Destroyer_A_ElectricShock()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindClosestEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            float ranX = Random.Range(-8, 8); float ranY = Random.Range(-8, 8);
            Vector3 destination = target.transform.position + new Vector3(ranX, ranY, 0);

            GameObject shock = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
            shock.GetComponent<EffectManager>().effectTime = dur;
            shock.GetComponent<EffectManager>().Effect();

            shock.GetComponent<PlayerAura>().damage = psm.pt.bulletDamage * atk;
            shock.GetComponent<PlayerAura>().dur    = dur;
            shock.GetComponent<PlayerAura>().atk    = atk;
            shock.GetComponent<PlayerAura>().ran    = ran;
        }
    }

    // 구축함 B타입 => 쉴드 변환
    void Harbinger_Destroyer_B_ShieldConversion()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindClosestEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            EnemyShipManager esm = target.GetComponentInParent<EnemyShipManager>();
            esm.eseg.EffectGenerator("ShieldDown", 0, 0, 0, 1);

            int cmp = (int)(psm.shipOriginAp / 1000 * atk);
            psm.seg.EffectGenerator("Protect", dur, 0, 0, cmp);
        }
    }

    // 지원함 A타입 => 세례
    void Harbinger_Auxiliary_A_Baptism()
    {
        GameObject baptism = Instantiate(skillEffect, transform.position, transform.rotation);
        baptism.GetComponent<PlayerAura>().atk = psm.pt.bulletDamage * atk;
    }

    // 지원함 B타입 => 전기 구름
    void Harbinger_Auxiliary_B_ThunderCloud()
    {
        GameObject cloud = Instantiate(skillEffect, transform.position, transform.rotation);
        cloud.GetComponent<PlayerAura>().dur = dur;
        cloud.GetComponent<PlayerAura>().atk = atk;
    }

    // 순양함 A타입 => 전도
    void Harbinger_Cruiser_A_Evangelize()
    {
        GameObject chain = Instantiate(skillEffect, psm.pt.turrets[0].position, transform.rotation);
        chain.GetComponent<ChainAttack>().damage = psm.pt.bulletDamage * atk;
    }

    // 순양함 B타입 => 전기 창
    void Harbinger_Cruiser_B_LightningSpear()
    {
        GameObject spear = Instantiate(skillEffect, skillPos[0].position, transform.rotation) as GameObject;
        spear.GetComponent<PlayerAura>().isBox   = true;
        spear.GetComponent<PlayerAura>().boxSize = new Vector2(100, 10);
        spear.GetComponent<PlayerAura>().damage  = psm.pt.bulletDamage * atk;
    }

    // 항공모함 B타입 => 쉴드 드론
    IEnumerator Harbinger_Carrier_B_ShieldDrone()
    {
        for (int i = 0; i < psm.pt.fighters.Length; i++)
        {
            psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().shield = true;
            psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().effect.Play();
        }

        yield return new WaitForSeconds(dur);

        for (int i = 0; i < psm.pt.fighters.Length; i++)
        {
            psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().shield = false;
            psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().effect.Stop();
        }
    }

    // 전함 A타입 => 신벌
    void Harbinger_Battleship_A_DivinePunishment()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindClosestEnemy();
        else
            target = psm.psmv.targeted;

        float ranX = Random.Range(-10, 11); float ranY = Random.Range(-10, 11);

        if (target != null)
        {
            Vector3 destination = new Vector3(target.transform.position.x + ranX, target.transform.position.y + ranY, 0);

            GameObject blackhole = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
            blackhole.transform.localScale = new Vector3(1.0f + ran, 1.0f + ran, 1.0f + ran);

            blackhole.GetComponent<PlayerAura>().dur = dur;
            blackhole.GetComponent<PlayerAura>().damage = psm.pt.bulletDamage * atk;
            blackhole.GetComponent<PlayerAura>().perRadius += ran;

            blackhole.GetComponent<PlayerShipMoving>().battleRadius = 0;
            blackhole.GetComponent<PlayerShipMoving>().shipSpeed = 3;
            blackhole.GetComponent<PlayerShipMoving>().shipOriginSpeed = 3;
            blackhole.GetComponent<PlayerShipMoving>().turnSpeed = 15;
        }
    }

    // 항공모함 B타입 => 복수
    IEnumerator Harbinger_Battleship_B_Revenge()
    {
        psm.phb.isRevenge = true;
        psm.phb.revengeDamage = 0;
        psm.seg.EffectTimeInstance("RevengeCharge", dur);

        yield return new WaitForSeconds(dur);

        psm.phb.isRevenge = false;

        if (psm.psmv.FindClosestEnemy() != null)
        {
            Vector3 destination = psm.psmv.FindClosestEnemy().transform.position;

            GameObject revenge = Instantiate(skillEffect, skillPos[0].position, transform.rotation) as GameObject;
            revenge.GetComponent<EnemyHitDamage>().destination = destination;
            revenge.GetComponent<EnemyHitDamage>().bulletDamage = psm.phb.revengeDamage * atk;
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
                        Harbinger_Destroyer_A_ElectricShock();
                        break;
                    case PlayerShipManager.ShipType.Auxiliary:
                        Harbinger_Auxiliary_A_Baptism();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        Harbinger_Cruiser_A_Evangelize();
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        Harbinger_Battleship_A_DivinePunishment();
                        break;
                }
            }
            else if (psm.typePlus == "B")
            {
                switch (psm.shipType)
                {
                    case PlayerShipManager.ShipType.Destroyer:
                        Harbinger_Destroyer_B_ShieldConversion();
                        break;
                    case PlayerShipManager.ShipType.Auxiliary:
                        Harbinger_Auxiliary_B_ThunderCloud();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        Harbinger_Cruiser_B_LightningSpear();
                        break;
                    case PlayerShipManager.ShipType.Carrier:
                        StartCoroutine("Harbinger_Carrier_B_ShieldDrone");
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        StartCoroutine("Harbinger_Battleship_B_Revenge");
                        break;
                }
            }
        }

        isTime = true;
    }
}
