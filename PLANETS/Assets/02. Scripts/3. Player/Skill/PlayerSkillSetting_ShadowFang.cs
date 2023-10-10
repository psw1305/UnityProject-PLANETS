using System.Collections;
using UnityEngine;

public class PlayerSkillSetting_ShadowFang : MonoBehaviour
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
                // 항공모함 A타입 => 공격력 약화
                if (psm.typePlus == "N" || psm.typePlus == "A")
                {
                    for (int i = 0; i < psm.pt.fighters.Length; i++)
                    {
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletDur = dur;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletAtk = atk;
                        psm.pt.fighters[i].GetComponent<PlayerFighterShipManager>().pt.bulletNum = num;
                    }
                }
                // 항공모함 B타입 => 자폭모드
                else if (psm.typePlus == "B")
                {
                    psm.pt.damagePercent = atk;
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
        else if (psm.shipType == PlayerShipManager.ShipType.Auxiliary)
        {
            if (isActive)
            {
                // 지원함 A타입 => 자폭모드
                if (psm.typePlus == "N" || psm.typePlus == "A")
                {
                    psm.selfDestruct = true;
                    psm.atk = atk;
                    psm.ran = ran;
                    psm.num = num;
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
        psm.seg.EffectGenerator("DefenseUP2", dur, atk, ran, num);

        GameObject flame = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
        flame.transform.parent = transform;
        flame.transform.localScale = new Vector3(1 + ran, 1 + ran, 1 + ran);

        flame.GetComponent<PlayerAura>().damage = psm.pt.bulletDamage * 0.1f;
        flame.GetComponent<PlayerAura>().dur    = dur;
        flame.GetComponent<PlayerAura>().num    = 1;
        flame.GetComponent<PlayerAura>().perRadius = ran;
    }

    // 구축함 B타입 => 우주 기뢰
    IEnumerator ShadowFang_Destroyer_B_SpaceMine()
    {
        for (int i = 0; i < num; i++)
        {
            if (!psm.psmv.isTarget)
                target = psm.psmv.FindClosestEnemy();
            else
                target = psm.psmv.targeted;

            if (target != null)
            {
                float ranNum = Random.Range(16, 20);
                Vector3 destination = target.transform.position + new Vector3(Mathf.Cos(Random.Range(-75, 75)) * ranNum, Mathf.Sin(Random.Range(-75, 75)) * ranNum, 0);

                GameObject mine = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
                mine.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().dur = dur;
                mine.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().atk = psm.pt.bulletDamage * atk;
                mine.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().num = num;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    // 지원함 B타입 => 제압탄환사격
    void ShadowFang_Auxiliary_B_SuppressiveFire()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindClosestEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            GameObject firezone = Instantiate(skillEffect, target.transform.position, transform.rotation) as GameObject;
            firezone.GetComponent<PlayerAura>().atk = atk;
            firezone.GetComponent<PlayerAura>().dur = dur;
            firezone.GetComponent<PlayerAura>().num = num;

            GetComponentInChildren<SubPlayerTurret>().destination = target.transform.position;
            GetComponentInChildren<SubPlayerTurret>().SubTurretActive();
        }
    }

    // 순양함 A타입 => 천공 드릴
    IEnumerator ShadowFang_Cruiser_A_DragonRising()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindClosestEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            psm.seg.EffectTimeInstance("Strong", dur);

            psm.pt.bulletAmmos = 5;
            psm.pt.damagePercent -= 0.5f;
            psm.psmv.isTarget = true;
            psm.psmv.targeted = target;

            yield return new WaitForSeconds(dur / 2);

            if (psm.psmv.targeted != null)
                psm.psmv.targeted.GetComponentInParent<EnemyShipManager>().eseg.EffectGenerator("Overwhelm", dur / 2, atk, ran, num);
            else
            {
                psm.psmv.isTarget = false;
                psm.psmv.targeted = null;
            }

            yield return new WaitForSeconds(dur / 2);

            psm.pt.bulletAmmos = 1;
            psm.pt.damagePercent += 0.5f;
            psm.psmv.isTarget = false;
            psm.psmv.targeted = null;
        }
    }

    // 순양함 B타입 => 부착형 트랩
    void ShadowFang_Cruiser_B_StickyTrap()
    {
        if (!psm.psmv.isTarget)
            target = psm.psmv.FindRandomEnemy();
        else
            target = psm.psmv.targeted;

        if (target != null)
        {
            GameObject trap = Instantiate(skillEffect) as GameObject;
            trap.transform.parent = target.transform.parent;
            trap.transform.localPosition = Vector3.zero;
            trap.GetComponent<PlayerAura>().atk = psm.pt.bulletDamage * atk;
            trap.GetComponent<PlayerAura>().dur = dur;
            trap.GetComponent<PlayerAura>().num = num;
        }
    }

    // 전함 A타입 => 폭죽
    void ShadowFang_Battleship_A_Firework()
    {
        if (psm.psmv.FindRandomEnemy() != null)
        {
            float rangeX = Random.Range(-15, 15); float rangeY = Random.Range(-15, 15);
            Vector3 destination = psm.psmv.FindRandomEnemy().transform.position + new Vector3(rangeX, rangeY, 0);

            GameObject firework = Instantiate(skillEffect, destination, transform.rotation) as GameObject;
            firework.GetComponent<EffectManager>().effectTime = dur;
            firework.GetComponent<EffectManager>().Effect();

            firework.GetComponent<PlayerAura>().dur = dur;
            firework.GetComponent<PlayerAura>().num = num;
        }
    }

    // 전함 B타입 => 화염자취
    IEnumerator ShadowFang_Battleship_B_FireBreathe()
    {
        psm.psmv.battleRadius = 0;
        psm.psmv.shipSpeed = 9;
        psm.psmv.movePercent += 1.0f;
        psm.psmv.turnSpeed = 0;
        psm.psmv.booster.SetActive(true);

        for (int i = 0; i < num; i++)
        {
            GameObject breathe = Instantiate(skillEffect, transform.position, transform.rotation) as GameObject;
            breathe.GetComponent<PlayerAura>().damage = psm.pt.bulletDamage * atk;
            breathe.GetComponent<PlayerAura>().dur = dur;

            yield return new WaitForSeconds(dur / num);
        }

        psm.psmv.battleRadius = 80;
        psm.psmv.movePercent -= 1.0f;
        psm.psmv.turnSpeed = 12;
        psm.psmv.booster.SetActive(false);
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
                        ShadowFang_Destroyer_A_FlameShield();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        StartCoroutine("ShadowFang_Cruiser_A_DragonRising");
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        ShadowFang_Battleship_A_Firework();
                        break;
                }
            }
            else if (psm.typePlus == "B")
            {
                switch (psm.shipType)
                {
                    case PlayerShipManager.ShipType.Destroyer:
                        StartCoroutine("ShadowFang_Destroyer_B_SpaceMine");
                        break;
                    case PlayerShipManager.ShipType.Auxiliary:
                        ShadowFang_Auxiliary_B_SuppressiveFire();
                        break;
                    case PlayerShipManager.ShipType.Cruiser:
                        ShadowFang_Cruiser_B_StickyTrap();
                        break;
                    case PlayerShipManager.ShipType.Battleship:
                        StartCoroutine("ShadowFang_Battleship_B_FireBreathe");
                        break;
                }
            }
        }

        isTime = true;
    }
}
