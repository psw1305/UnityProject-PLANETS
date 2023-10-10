using System.Collections;
using UnityEngine;

public class EnemyBossManager : MonoBehaviour
{
    public string raceType, shipType, typePlus, shipLevel;

    [Header("Ship State")]
    [HideInInspector] public bool isDestroy = false;
    [HideInInspector] public bool isEscape = false;
    [HideInInspector] public bool isRetire = false;
    [HideInInspector] public bool isRepair = false;
    [HideInInspector] public bool isMission = false;
    [HideInInspector] public bool isShield = true;

    [Header("Ship Manager")]
    public GameObject core;
    public GameObject shipImage;
    public float shipDeadTime;
    float shieldOriginTime = 0;

    [HideInInspector] public float shipHp, shipAp, shipOp;
    [HideInInspector] public float shipOriginHp, shipOriginAp, shipOriginOp;

    [Header("Ship Effect")]
    [HideInInspector] public bool isDrain = false;
    [HideInInspector] public float drainTime;
    [HideInInspector] public float drainAp;

    [HideInInspector] public float shieldTime = 30, damagedSum, damagedPercent;
    [HideInInspector] public float hpPercent, apPercent, dodge;
    [HideInInspector] public int alertLevel = 1;

    [Header("Prefabs")]
    public GameObject shipExplosion;
    public GameObject shipWreck;

    [Header("UI")]
    [HideInInspector] public GameObject gage;
    [HideInInspector] public UISlider hpBarSlider, apBarSlider;
    UILabel hpText, apText;

    [Header("Script")]
    public EnemyShipMoving esmv;
    public EnemyTurret[] et;
    public BossHitBox bhb;
    [HideInInspector] public EnemyFleet ef;

    void Start()
    {
        EnemyDataBase.Instance.BossStatDataParsing(raceType, shipType, typePlus, shipLevel, this);

        for (int i = 0; i < et.Length; i++)
        {
            et[i].ebm = GetComponent<EnemyBossManager>();
            et[i].bulletDivideDamage = et[i].bulletDamage / et[i].bulletAmmos;

            if (et[i].attackType == EnemyTurret.AttackType.Fighter)
            {
                GameObject squad = Instantiate(et[i].fs[0].gameObject, transform.parent.position, Quaternion.identity) as GameObject;
                squad.transform.parent = transform.parent;
                et[i].fighters = squad.GetComponent<FighterSquad>().fighters;
            }
        }

        esmv.ebm = this;
        esmv.Init();

        damagedSum = 0; damagedPercent = 1.0f;
        hpPercent = 1.0f; apPercent = 1.0f; dodge = 0.0f;

        shipOriginHp = shipHp;
        shipOriginAp = shipAp;
        shieldOriginTime = shieldTime;

        hpBarSlider = gage.GetComponent<Mission_5_Boss>().hpSlider;
        apBarSlider = gage.GetComponent<Mission_5_Boss>().apSlider;

        hpText = gage.GetComponent<Mission_5_Boss>().hpText;
        apText = gage.GetComponent<Mission_5_Boss>().apText;

        HealthValue();
        ShieldValue();

        if (isShield)
            bhb.ShieldActive();
        else if (!isShield)
            apBarSlider.value = 0;
    }

    void Update()
    {
        if (core != null)
            ShipHealthUpdating();
    }

    void ShipHealthUpdating()
    {
        if (!isShield && shipOriginAp != 0 && gage != null)
        {
            if (shieldTime > 0)
            {
                shieldTime -= Time.deltaTime;
                apBarSlider.value = (shieldOriginTime - shieldTime) / shieldOriginTime;

                if (shieldTime <= 0)
                {
                    shieldTime = shieldOriginTime;
                    shipAp = shipOriginAp;
                    ShieldValue();
                    bhb.ShieldActive();
                    isShield = true;
                }
            }
        }
        else if (isDrain && shipOriginAp != 0 && gage != null)
        {
            if (drainTime > 0)
            {
                drainTime -= Time.deltaTime;
                shipAp += Time.deltaTime * drainAp;

                if (shipAp > shipOriginAp)
                {
                    shipAp = shipOriginAp;
                    apBarSlider.value = 1.0f;
                }

                apBarSlider.value = shipAp / shipOriginAp;

                if (drainTime <= 0)
                {
                    drainTime = 0;
                    shieldTime = shieldOriginTime;
                    shipAp = Mathf.Round(apBarSlider.value * shipOriginAp);
                    isDrain = false;
                    isShield = true;
                }
            }
        }

        if (shipHp > shipOriginHp)
        {
            isRepair = false;
            shipHp = shipOriginHp;
            hpBarSlider.value = 1.0f;
            hpText.text = (int)shipHp + "  /  " + (int)shipOriginHp;
        }

        if (shipAp > shipOriginAp)
        {
            shipAp = shipOriginAp;
            apBarSlider.value = 1.0f;
            apText.text = (int)shipAp + "  /  " + (int)shipOriginAp;
        }
    }

    public void Damage(float damageCount)
    {
        if (Random.value >= dodge)
        {
            if (damagedPercent <= 0.05f)
                damageCount *= 0.05f;
            else
                damageCount *= damagedPercent;

            isRepair = true;

            if (shipHp < damageCount)
                shipHp -= shipHp;
            else
                shipHp -= damageCount;

            HealthValue();

            if (hpBarSlider.value <= 0.6f && isEscape)
                StartCoroutine("ShipRetreat");

            if (shipHp <= 0 && !isDestroy)
            {
                shipHp = 0;
                hpBarSlider.value = 0;
                EnemyShipEnd();
            }
        }
    }

    public void ShieldDamage(float damageCount)
    {
        if (Random.value >= dodge)
        {
            if (damagedPercent <= 0.05f)
                damageCount *= 0.05f;
            else
                damageCount *= damagedPercent;

            if (shipAp < damageCount)
            {
                damageCount = shipAp;
                shipAp -= damageCount;
            }
            else
                shipAp -= damageCount;

            ShieldValue();

            if (shipAp <= 0)
            {
                isShield = false;
                shipAp = 0;
                apBarSlider.value = 0;
                bhb.ShieldDeactive();
            }
        }
    }

    public void EnemyShipEnd()
    {
        StartCoroutine("EnemyShipExplosion");
    }

    IEnumerator EnemyShipExplosion()
    {
        isDestroy = true;
        esmv.isEnable = false;

        for (int i = 0; i < et.Length; i++)
            et[i].isEnable = false;

        Destroy(gage);
        Destroy(core);

        if (shipExplosion != null)
            shipExplosion.SetActive(true);

        yield return new WaitForSeconds(shipDeadTime);
        ef.MissionClearCheck(1);

        if (shipWreck != null)
        {
            GameObject wreckClone = Instantiate(shipWreck, transform.position, Quaternion.identity) as GameObject;
            wreckClone.transform.parent = transform.parent;
        }

        Destroy(gameObject);
    }

    public void HealthValue()
    {
        if (!isDestroy && hpBarSlider != null)
        {
            hpBarSlider.value = shipHp / shipOriginHp;
            hpText.text = (int)shipHp + "  /  " + (int)shipOriginHp;
        }
    }

    public void ShieldValue()
    {
        if (!isDestroy && apBarSlider != null)
        {
            apBarSlider.value = shipAp / shipOriginAp;
            apText.text = (int)shipAp + "  /  " + (int)shipOriginAp;
        }
    }
}
