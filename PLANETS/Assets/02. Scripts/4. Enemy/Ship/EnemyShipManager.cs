using UnityEngine;
using System.Collections;

public class EnemyShipManager : MonoBehaviour 
{
    public enum RaceType { Kalas, ShadowFang, Aridrian, Harbinger, None }
    public RaceType raceType;
    public enum ShipType { Destroyer, Auxiliary, Cruiser, Carrier, Battleship, None }
    public ShipType shipType;
    public string typePlus, shipLevel;

    [Header("Ship State")]
    [HideInInspector] public bool isDestroy = false;
    [HideInInspector] public bool isRetire  = false;
    [HideInInspector] public bool isRepair  = false;
    
    [HideInInspector] public bool isElite   = false;
    [HideInInspector] public bool isMission = false;

    [HideInInspector] public bool isOverHp = false;
    [HideInInspector] public bool isShield = true;

    [HideInInspector] public bool obstacle = false;
    [HideInInspector] public float obsNum;

    [HideInInspector] public bool uncharge = false;
    [HideInInspector] public bool ramming  = false;

    [HideInInspector] public bool chargeShield = false;
    [HideInInspector] public bool selfDestruct = false;

    [Header("Ship Manager")]
    public GameObject core;
    public GameObject shipImage;
    public float shipDeadTime;

    [HideInInspector] public float shipHp, shipAp, shipMp, shipOp;
    [HideInInspector] public float shipOriginHp, shipOriginAp, shipOriginMp, shipOriginOp;
    [HideInInspector] public float dur, atk, ran, num;

    [Header("Ship Effect")]
    [HideInInspector] public bool isDrain = false;
    [HideInInspector] public float drainTime;
    [HideInInspector] public float drainAp;

    [HideInInspector] public float shieldTime, shieldOriginTime, damagedSum, damagedPercent;
    [HideInInspector] public float hpPercent, apPercent, dodge;
    [HideInInspector] public int alertLevel = 1;

    [Header("Prefabs")]
    public GameObject shipExplosion;
    public GameObject shipWreck;

    [Header("UI")]
	public GameObject indicator;
	public GameObject uiGage;
    [HideInInspector] public GameObject indicatorClone, gage;
    [HideInInspector] public UISprite hpBarSlider, apBarSlider, opBarSlider;
    UILabel hpText, apText, mpText;

    [Header("Script")]
    public EnemyShipMoving esmv;
    public EnemyTurret et;
    public EnemyHitBox ehb;
    public EnemySkillEffectGenerator eseg;
    [HideInInspector] public EnemyFleet ef;

    void Start() 
	{
        EnemyDataBase.Instance.EnemyBaseDataParsing(raceType.ToString(), shipType.ToString(), this);
        EnemyDataBase.Instance.EnemyStatDataParsing(raceType.ToString(), shipType.ToString(), typePlus, shipLevel, this);

        esmv.esm = this;
        et.esm   = this;

        esmv.Init();
        et.bulletDivideDamage = et.bulletDamage / et.bulletAmmos;
        et.turretSensor = esmv.battleRadius + 20;

        if (et.attackType == EnemyTurret.AttackType.Fighter)
        {
            GameObject squad;

            if (shipLevel == "Normal")
                squad = Instantiate(et.fs[1].gameObject, transform.parent.position, Quaternion.identity) as GameObject;
            else
                squad = Instantiate(et.fs[0].gameObject, transform.parent.position, Quaternion.identity) as GameObject;

            squad.transform.parent = transform.parent;
            et.fighters = squad.GetComponent<FighterSquad>().fighters;

            for (int i = 0; i < et.fighters.Length; i++)
                et.fighters[i].GetComponent<EnemyFighterShipManager>().FighterDataParsing(raceType.ToString(), shipLevel);
        }

        damagedSum = 0; damagedPercent = 1.0f;
        hpPercent = 1.0f; apPercent = 1.0f; dodge = 0.0f;

        shipOriginHp = shipHp;
        shipOriginAp = shipAp;
        shipOriginMp = shipMp;
        shieldOriginTime = shieldTime;

        //indicatorClone = Instantiate (indicator) as GameObject;
        //indicatorClone.transform.parent = GameObject.FindWithTag("UIEnemy").transform;
        //indicatorClone.transform.localScale = new Vector3(1, 1, 1);
        //indicatorClone.GetComponent<OffScreenTarget>().goToTrack = transform;

        gage = Instantiate(uiGage.gameObject, transform.position, Quaternion.identity) as GameObject;
        gage.GetComponent<UIGageManager>().target = transform;
        gage.transform.parent     = GameObject.FindWithTag("UIEnemy").transform;
        gage.transform.localScale = new Vector3(1, 1, 1);

        hpBarSlider = gage.GetComponent<UIGageManager>().hpBar;
        apBarSlider = gage.GetComponent<UIGageManager>().apBar;
        opBarSlider = gage.GetComponent<UIGageManager>().opBar;

        hpText = gage.GetComponent<UIGageManager>().hpText;
        apText = gage.GetComponent<UIGageManager>().apText;
        mpText = gage.GetComponent<UIGageManager>().mpText;

        if (shipLevel != "Beginner")
            RaceBalanceSystem(true);
        else
            RaceBalanceSystem(false);

        if (raceType == RaceType.ShadowFang)
        {
            uncharge = true;
            isShield = false;
        }

        HealthValue();
        ShieldValue();
        CrewValue();

        if (isShield)
            ehb.ShieldActive();
        else if (!isShield) 
			apBarSlider.fillAmount = 0;
	}

    public void RaceBalanceSystem(bool isCheck)
    {
        switch (raceType)
        {
            case RaceType.Kalas:
                GetComponent<EnemySkillSetting_Kalas>().isActive = isCheck;
                GetComponent<EnemySkillSetting_Kalas>().Init();
                break;
            case RaceType.ShadowFang:
                GetComponent<EnemySkillSetting_ShadowFang>().isActive = isCheck;
                GetComponent<EnemySkillSetting_ShadowFang>().Init();
                break;
            case RaceType.Aridrian:
                GetComponent<EnemySkillSetting_Aridrian>().isActive = isCheck;
                GetComponent<EnemySkillSetting_Aridrian>().Init();
                break;
            case RaceType.Harbinger:
                GetComponent<EnemySkillSetting_Harbinger>().isActive = isCheck;
                GetComponent<EnemySkillSetting_Harbinger>().Init();
                break;
        }
    }

    public void RaceSkillCoolTime(float timePlus)
    {
        switch (raceType)
        {
            case RaceType.Kalas:
                GetComponent<EnemySkillSetting_Kalas>().timePlus = timePlus;
                break;
            case RaceType.ShadowFang:
                GetComponent<EnemySkillSetting_ShadowFang>().timePlus = timePlus;
                break;
            case RaceType.Aridrian:
                GetComponent<EnemySkillSetting_Aridrian>().timePlus = timePlus;
                break;
            case RaceType.Harbinger:
                GetComponent<EnemySkillSetting_Harbinger>().timePlus = timePlus;
                break;
        }
    }

    void Update()
	{
        if (core != null)
            ShipHealthUpdating();
    }

    void ShipHealthUpdating()
    {
        if (!isShield && !chargeShield && shipOriginAp != 0 && gage != null && !uncharge)
        {
            if (shieldTime > 0)
            {
                shieldTime -= Time.deltaTime;
                apBarSlider.fillAmount = (shieldOriginTime - shieldTime) / shieldOriginTime * apPercent; ;

                if (shieldTime <= 0)
                {
                    shieldTime = shieldOriginTime;
                    shipAp     = shipOriginAp * apPercent; ;
                    ShieldValue();
                    ehb.ShieldActive();
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
                    apBarSlider.fillAmount = 1.0f;
                }

                apBarSlider.fillAmount = shipAp / shipOriginAp;

                if (drainTime <= 0)
                {
                    drainTime  = 0;
                    shieldTime = shieldOriginTime;
                    shipAp     = Mathf.Round(apBarSlider.fillAmount * shipOriginAp);
                    isDrain    = false;
                    isShield   = true;
                }
            }
        }

        if (shipHp > shipOriginHp * hpPercent)
        {
            isRepair = false;
            shipHp = shipOriginHp * hpPercent;
            hpBarSlider.fillAmount = 1.0f * hpPercent;
            hpText.text = shipHp.ToString();
        }

        if (shipAp > shipOriginAp * apPercent)
        {
            shipAp = shipOriginAp * apPercent;
            apBarSlider.fillAmount = 1.0f * apPercent;
            apText.text = shipAp.ToString();
        }

        if (shipMp > shipOriginMp)
        {
            shipMp = shipOriginMp;
            mpText.text = shipMp.ToString();
        }

        CrewCheck();
    }

    public void CrewCheck()
    {
        if (shipMp <= 0 && alertLevel == 3)
        {
            alertLevel += 1;
            mpText.text = "";
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
            EnemyShipRetire();
        }
        else if (shipMp <= shipOriginMp * 0.3f && alertLevel == 2)
        {
            alertLevel += 1;
            hpPercent = 0.6f;
            apPercent = 0.6f;
            et.damagePercent = 0.6f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
        else if (shipMp <= shipOriginMp * 0.6f && alertLevel == 1)
        {
            alertLevel += 1;
            hpPercent = 0.8f;
            apPercent = 0.8f;
            et.damagePercent = 0.8f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
    }

    public void AddCrewCheck(float crew)
    {
        if (shipMp > 0)
            shipMp += crew;
        else
            shipMp = crew;

        if (shipMp > shipOriginMp * 0.6f)
        {
            alertLevel = 1;
            hpPercent = 1f;
            apPercent = 1f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
        else if (shipMp > shipOriginMp * 0.3f)
        {
            alertLevel = 2;
            hpPercent = 0.8f;
            apPercent = 0.8f;
            et.damagePercent = 0.8f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
        else if (shipMp <= shipOriginMp * 0.3f)
        {
            alertLevel = 3;
            hpPercent = 0.6f;
            apPercent = 0.6f;
            et.damagePercent = 0.6f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
    }

    void CrewDamageCheck(float damage)
    {
        if (damage > 0)
            damagedSum += damage;

        if (damagedSum >= shipOriginHp * 0.03f)
        {
            int cnt = (int)(damagedSum / (shipOriginHp * 0.03f));
            damagedSum -= (shipOriginHp * 0.03f) * cnt;
            shipMp -= cnt;
            CrewValue();
        }
    }

    public void Damage (float damageCount)
	{
        if (damagedPercent <= 0.05f)
            damageCount *= 0.05f;
        else
            damageCount *= damagedPercent;

        isRepair = true;

        if (!isOverHp)
        {
            if (shipHp < damageCount)
                shipHp -= shipHp;
            else
                shipHp -= damageCount;

            if (shipHp >= shipOriginHp * hpPercent)
                shipHp = shipOriginHp * hpPercent;

            HealthValue();
            CrewDamageCheck(damageCount);

            if (shipHp <= 0 && !isDestroy)
            {
                shipHp = 0;
                hpBarSlider.fillAmount = 0;
                EnemyShipEnd();
            }
        }
        else
        {
            shipOp -= 1;
            opBarSlider.fillAmount = shipOp / shipOriginOp;

            if (shipOp <= 0)
            {
                isOverHp = false;
                shipOp = 0;
                opBarSlider.gameObject.SetActive(false);
            }
        }
    }

    public void ShieldDamage(float damageCount)
    {
        if (damagedPercent <= 0.05f)
            damageCount *= 0.05f;
        else
            damageCount *= damagedPercent;

        if (!isOverHp)
        {
            if (shipAp < damageCount)
                shipAp -= shipAp;
            else
                shipAp -= damageCount;

            ShieldValue();

            if (shipAp <= 0)
            {
                isShield = false;
                shipAp = 0;
                apBarSlider.fillAmount = 0;
                ehb.ShieldDeactive();
            }
        }
        else
        {
            shipOp -= 1;
            opBarSlider.fillAmount = shipOp / shipOriginOp;

            if (shipOp <= 0)
            {
                isOverHp = false;
                shipOp = 0;
                opBarSlider.gameObject.SetActive(false);
            }
        }
    }

    public void HullDamage(float damageCount)
    {
        if (damagedPercent <= 0.05f)
            damageCount *= 0.05f;
        else
            damageCount *= damagedPercent;

        if (isShield)
        {
            if (shipAp < damageCount)
                shipAp -= shipAp;
            else
                shipAp -= damageCount;

            ShieldValue();

            if (shipAp <= 0)
            {
                isShield = false;
                shipAp = 0;
                apBarSlider.fillAmount = 0;
                ehb.ShieldDeactive();
            }
        }
        else
        {
            if (shipHp < damageCount)
                shipHp -= shipHp;
            else
                shipHp -= damageCount;

            HealthValue();

            if (shipHp <= 0 && !isDestroy)
            {
                shipHp = 0;
                hpBarSlider.fillAmount = 0;
                EnemyShipEnd();
            }
        }
    }

    public void EnemyShipRetire()
    {
        isRetire = true;
        RaceBalanceSystem(false);

        shipMp = 0;
        CrewValue();

        uncharge = true;
        esmv.isEnable = false;
        et.isEnable = false;
    }

    public void EnemyShipRebirth()
    {
        isRetire = false;
        RaceBalanceSystem(true);

        uncharge = false;
        esmv.isEnable = true;
        et.isEnable = true;
    }

    public void EnemyShipEnd ()
	{
        if (!selfDestruct)
            StartCoroutine("EnemyShipExplosion");
        else
            StartCoroutine("EnemyShipRamming");
    }

	IEnumerator EnemyShipExplosion()
    {
        RaceBalanceSystem(false);

        isDestroy     = true;
        esmv.isEnable = false;
        et.isEnable   = false;
        Destroy(gage);
        Destroy(core);
        
        if (shipExplosion != null)
            shipExplosion.SetActive(true);

        yield return new WaitForSeconds(shipDeadTime);
        ef.MissionClearCheck(1);

        if (shipWreck != null)
        {
            GameObject wreckClone = Instantiate(shipWreck, transform.position, transform.rotation) as GameObject;
            wreckClone.transform.parent = transform.parent;
        }

        Destroy(gameObject);
    }

    IEnumerator EnemyShipRamming()
    {
        selfDestruct = false;
        ramming = true;
        shipHp = shipOriginHp * 0.4f;
        HealthValue();
        RaceBalanceSystem(false);

        et.isEnable = false;
        esmv.movingType = EnemyShipMoving.MovingType.Booster;
        esmv.booster.SetActive(true);

        yield return new WaitForSeconds(6);

        ExplosionDamage();
    }

    public void ExplosionDamage()
    {
        if (!isDestroy)
        {
            isDestroy = true;
            ramming = false;
            esmv.isEnable = false;
            Destroy(gage);
            Destroy(core);

            ef.MissionClearCheck(1);

            if (shipWreck != null)
            {
                GameObject wreckClone = Instantiate(shipWreck, transform.position, transform.rotation) as GameObject;
                wreckClone.transform.parent = transform.parent;

                if (wreckClone.GetComponent<WreckAnimation>().finale != null)
                {
                    wreckClone.GetComponent<WreckAnimation>().finale.transform.localScale = new Vector3(0.5f + ran, 0.5f + ran, 0.5f + ran);
                    wreckClone.GetComponent<WreckAnimation>().finale.GetComponent<EnemyAura>().atk = atk * shipOriginHp;
                    wreckClone.GetComponent<WreckAnimation>().finale.GetComponent<EnemyAura>().num = num;
                    wreckClone.GetComponent<WreckAnimation>().finale.GetComponent<EnemyAura>().perRadius += ran;
                }
            }

            Destroy(gameObject);
        }
    }

    public void HealthValue()
    {
        if (!isDestroy && hpBarSlider != null)
        {
            hpBarSlider.fillAmount = shipHp / shipOriginHp;
            hpText.text = ((int)shipHp).ToString();
        }
    }

    public void ShieldValue()
    {
        if (!isDestroy && apBarSlider != null)
        {
            apBarSlider.fillAmount = shipAp / shipOriginAp;
            apText.text = ((int)shipAp).ToString();
        }
    }

    public void CrewValue()
    {
        if (!isDestroy && alertLevel != 4)
        {
            mpText.text = shipMp.ToString("N0");
        }
    }

    public void ShipOnline()
    {
        StartCoroutine("WarpShip");
    }

    IEnumerator WarpShip()
    {
        shipImage.GetComponent<EffectManager>().EffectCheck(true);
        shipImage.GetComponent<TweenScale>().PlayForward();
        shipImage.GetComponent<TweenPosition>().PlayForward();

        yield return new WaitForSeconds(1.5f);

        core.SetActive(true);
        gage.SetActive(true);
        //indicatorClone.SetActive(true);
        et.isEnable = true;
        esmv.isEnable = true;
    }

    public void ShipOffline()
    {
        core.SetActive(false);
        gage.SetActive(false);
        //indicatorClone.SetActive(false);
        et.isEnable = false;
    }
}
