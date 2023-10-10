using UnityEngine;
using System.Collections;

public class PlayerShipManager : MonoBehaviour 
{
    public enum RaceType { Terran, Aridrian, Kalas, ShadowFang, Harbinger, None }
    public RaceType raceType;
    public enum ShipType { Destroyer, Auxiliary, Cruiser, Carrier, Battleship, None }
    public ShipType shipType;
    public string typePlus;
    public int shipLevel;
    [HideInInspector] public Vector3 startPosition;

    [Header("Ship Data")]
    public string shipName;
    public int shipPosID;

    [Header("Ship State")]
    [HideInInspector] public bool isDestroy = false;
    [HideInInspector] public bool isRetire  = false;
    [HideInInspector] public bool isRepair  = false;
    [HideInInspector] public bool isMission = false;

    [HideInInspector] public bool isOverHp = false;
    [HideInInspector] public bool isShield = true;

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

    [HideInInspector] public bool isSteal = false;
    [HideInInspector] public float stealAp;
    [HideInInspector] public float stealTime;

    [HideInInspector] public float shieldTime, shieldOriginTime, damagedSum, damagedPercent;
    [HideInInspector] public float hpPercent, apPercent, dodge;
    [HideInInspector] public int alertLevel = 1;

    [Header("Prefabs")]
    public GameObject shipExplosion;
    public GameObject shipWreck;

    [Header("UI")]
    public GameObject indicator;
    public GameObject uiGage;
    [HideInInspector] public GameObject skillBtn;
    [HideInInspector] public GameObject indicatorClone, gage;
    [HideInInspector] public UISprite hpBarSlider, apBarSlider, opBarSlider;
    UILabel hpText, apText, mpText;

    [Header("Scripts")]
    public PlayerShipMoving psmv;
    public PlayerTurret pt;
    public PlayerHitBox phb;
    public SkillEffectGenerator seg;
    [HideInInspector] public PlayerFleet pf;

    Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);

		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}
    
    void Start() 
	{
        PlayerDataBase.Instance.PlayerBaseDataParsing(raceType.ToString(), shipType.ToString(), this);
        PlayerDataBase.Instance.PlayerStatDataParsing(raceType.ToString(), shipType.ToString(), typePlus, shipLevel, this);

        psmv.psm = this;
        pt.psm   = this;

        psmv.Init();
        pt.bulletDivideDamage = pt.bulletDamage / pt.bulletAmmos;
        pt.turretSensor = psmv.battleRadius + 20;

        if (pt.attackType == PlayerTurret.AttackType.Fighter)
        {
            GameObject squad;

            if (shipLevel == 2)    
                squad = Instantiate(pt.fs[1].gameObject, transform.parent.position, Quaternion.identity) as GameObject;
            else
                squad = Instantiate(pt.fs[0].gameObject, transform.parent.position, Quaternion.identity) as GameObject;

            squad.transform.parent = transform.parent;
            pt.fighters = squad.GetComponent<FighterSquad>().fighters;

            for (int i = 0; i < pt.fighters.Length; i++)
                pt.fighters[i].GetComponent<PlayerFighterShipManager>().FighterDataParsing(raceType.ToString(), shipLevel);
        }

        damagedSum = 0; damagedPercent = 1.0f;
        hpPercent = 1.0f; apPercent = 1.0f; dodge = 0.0f;

        shipOriginHp = shipHp;
        shipOriginAp = shipAp;
        shipOriginMp = shipMp;
        shieldOriginTime = shieldTime;

        //indicatorClone = Instantiate(indicator) as GameObject;
        //indicatorClone.transform.parent     = GameObject.FindWithTag("UIPlayer").transform;
        //indicatorClone.transform.localScale = new Vector3(1, 1, 1);
        //indicatorClone.GetComponent<OffScreenTarget>().goToTrack = transform;

        gage = Instantiate(uiGage.gameObject, transform.position, Quaternion.identity) as GameObject;
        gage.GetComponent<UIGageManager>().target = transform;
        gage.transform.parent     = GameObject.FindWithTag("UIPlayer").transform;
        gage.transform.localScale = new Vector3(1, 1, 1);

        hpBarSlider = gage.GetComponent<UIGageManager>().hpBar;
        apBarSlider = gage.GetComponent<UIGageManager>().apBar;
        opBarSlider = gage.GetComponent<UIGageManager>().opBar;

        hpText = gage.GetComponent<UIGageManager>().hpText;
        apText = gage.GetComponent<UIGageManager>().apText;
        mpText = gage.GetComponent<UIGageManager>().mpText;

        if (raceType == RaceType.ShadowFang)
        {
            uncharge = true;
            isShield = false;
        }

        ShipStateCheck();

        HealthValue();
        ShieldValue();
        CrewValue();

        if (isShield)
            phb.ShieldActive();
        else if (!isShield)
            apBarSlider.fillAmount = 0;
    }

    public void RaceBalanceSystem(bool isCheck)
    {
        switch (raceType)
        {
            case RaceType.Terran:
                GetComponent<PlayerSkillSetting_Terran>().isActive = isCheck;
                GetComponent<PlayerSkillSetting_Terran>().Init();
                break;
            case RaceType.Kalas:
                GetComponent<PlayerSkillSetting_Kalas>().isActive = isCheck;
                GetComponent<PlayerSkillSetting_Kalas>().Init();
                break;
            case RaceType.ShadowFang:
                GetComponent<PlayerSkillSetting_ShadowFang>().isActive = isCheck;
                GetComponent<PlayerSkillSetting_ShadowFang>().Init();
                break;
            case RaceType.Aridrian:
                GetComponent<PlayerSkillSetting_Aridrian>().isActive = isCheck;
                GetComponent<PlayerSkillSetting_Aridrian>().Init();
                break;
            case RaceType.Harbinger:
                GetComponent<PlayerSkillSetting_Harbinger>().isActive = isCheck;
                GetComponent<PlayerSkillSetting_Harbinger>().Init();
                break;
        }
    }

    void ShipStateCheck()
    {
        string state = PlayerPrefs.GetString("ShipState_" + shipName, "Normal");

        switch (state)
        {
            case "Damaged":
                shipMp = shipOriginMp * 0.75f;
                alertLevel = 1;
                CrewCheck();
                break;
            case "Deadly":
                shipMp = shipOriginMp * 0.5f;
                alertLevel = 2;
                CrewCheck();
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
        if (!isShield && !isSteal && shipOriginAp != 0 && gage != null && !uncharge)
        {
            if (shieldTime > 0)
            {
                shieldTime -= Time.deltaTime;
                apBarSlider.fillAmount = (shieldOriginTime - shieldTime) / shieldOriginTime * apPercent;

                if (shieldTime <= 0)
                {
                    shieldTime = shieldOriginTime;
                    shipAp     = shipOriginAp * apPercent;
                    ShieldValue();
                    phb.ShieldActive();
                    isShield   = true;
                }
            }
        }
        else if (isSteal && shipOriginAp != 0 && gage != null)
        {
            if (stealTime > 0)
            {
                stealTime -= Time.deltaTime;
                shipAp = apBarSlider.fillAmount * shipOriginAp * apPercent;
                shipAp -= Time.deltaTime * stealAp;
                apBarSlider.fillAmount = shipAp / shipOriginAp * apPercent;

                if (shipAp <= 0)
                {
                    shipAp = 0;
                    shieldTime = shieldOriginTime;
                }

                if (stealTime <= 0)
                {
                    shipAp = Mathf.Round(apBarSlider.fillAmount * shipOriginAp * apPercent);
                    float apTime = apBarSlider.fillAmount * shieldOriginTime;
                    stealTime = 0;
                    isSteal = false;

                    if (apTime > 0)
                    {
                        if (shipAp > 0)
                            isShield = true;
                        else if (shipAp == 0)
                        {
                            isShield = false;
                            shieldTime = shieldOriginTime - apBarSlider.fillAmount * shieldOriginTime;
                        }
                    }
                    else if (apTime == 0)
                    {
                        if (shipAp > 0)
                            isShield = true;
                        else if (shipAp == 0)
                        {
                            isShield = false;
                            if (shieldTime == 0)
                                shieldTime = shieldOriginTime;
                        }
                    }
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
            PlayerShipRetire();
        }
        else if (shipMp <= shipOriginMp * 0.3f && alertLevel == 2)
        {
            PlayerPrefs.SetString("ShipState_" + shipName, "Deadly");

            alertLevel += 1;
            hpPercent = 0.6f;
            apPercent = 0.6f;
            pt.damagePercent = 0.6f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
        else if (shipMp <= shipOriginMp * 0.6f && alertLevel == 1)
        {
            PlayerPrefs.SetString("ShipState_" + shipName, "Damaged");

            alertLevel += 1;
            hpPercent = 0.8f;
            apPercent = 0.8f;
            pt.damagePercent = 0.8f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
        else if (shipMp >= shipOriginMp * 0.6f && alertLevel == 1)
            PlayerPrefs.SetString("ShipState_" + shipName, "Normal");
    }

    public void AddCrewCheck(float crew)
    {
        if (shipMp > 0)
            shipMp += crew;
        else
            shipMp = crew;

        if (shipMp > shipOriginMp * 0.6f)
        {
            PlayerPrefs.SetString("ShipState_" + shipName, "Normal");

            alertLevel = 1;
            hpPercent  = 1f;
            apPercent  = 1f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
        else if (shipMp > shipOriginMp * 0.3f)
        {
            PlayerPrefs.SetString("ShipState_" + shipName, "Damaged");

            alertLevel = 2;
            hpPercent  = 0.8f;
            apPercent  = 0.8f;
            pt.damagePercent = 0.8f;
            gage.GetComponent<UIGageManager>().CrewAlert(alertLevel);
        }
        else if (shipMp <= shipOriginMp * 0.3f)
        {
            PlayerPrefs.SetString("ShipState_" + shipName, "Deadly");

            alertLevel = 3;
            hpPercent  = 0.6f;
            apPercent  = 0.6f;
            pt.damagePercent = 0.6f;
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
                PlayerShipEnd();
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
                phb.ShieldDeactive();
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
                phb.ShieldDeactive();
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
                PlayerShipEnd();
            }
        }
    }

    public void PlayerShipRetire()
    {
        isRetire = true;
        RaceBalanceSystem(false);

        shipMp = 0;
        CrewValue();

        uncharge = true;
        psmv.isEnable = false;
        pt.isEnable = false;
    }

    public void PlayerShipRebirth()
    {
        isRetire = false;
        RaceBalanceSystem(true);

        uncharge = false;
        psmv.isEnable = true;
        pt.isEnable = true;
    }

    public void PlayerShipEnd()
    {
        if (!selfDestruct)
            StartCoroutine("PlayerShipExplosion");
        else
            StartCoroutine("PlayerShipRamming");
    }

    IEnumerator PlayerShipExplosion()
    {
        RaceBalanceSystem(false);
        PlayerPrefs.SetString("ShipState_" + shipName, "Destroyed");

        isDestroy     = true;
        psmv.isEnable = false;
        pt.isEnable   = false;

        if (isMission)
            pf.missionShipCheck -= 1;

        Destroy(gage);
        Destroy(core);

        if (shipExplosion != null)
            shipExplosion.SetActive(true);

        yield return new WaitForSeconds(shipDeadTime);
        pf.MissionFailedCheck(1);

        if (shipWreck != null)
        {
            GameObject wreckClone = Instantiate(shipWreck, transform.position, transform.rotation) as GameObject;
            wreckClone.transform.parent = transform.parent;
        }

        Destroy(gameObject);
    }

    IEnumerator PlayerShipRamming()
    {
        selfDestruct = false;
        ramming = true;
        shipHp = shipOriginHp * 0.4f;
        HealthValue();
        RaceBalanceSystem(false);

        pt.isEnable = false;
        psmv.movingType = PlayerShipMoving.MovingType.Booster;
        psmv.booster.SetActive(true);

        yield return new WaitForSeconds(6);

        ExplosionDamage();
    }

    public void ExplosionDamage()
    {
        if (!isDestroy)
        {
            isDestroy = true;
            ramming = false;
            psmv.isEnable = false;
            Destroy(gage);
            Destroy(core);

            pf.MissionFailedCheck(1);

            if (shipWreck != null)
            {
                GameObject wreckClone = Instantiate(shipWreck, transform.position, transform.rotation) as GameObject;
                wreckClone.transform.parent = transform.parent;

                if (wreckClone.GetComponent<WreckAnimation>().finale != null)
                {
                    wreckClone.GetComponent<WreckAnimation>().finale.transform.localScale = new Vector3(0.5f + ran, 0.5f + ran, 0.5f + ran);
                    wreckClone.GetComponent<WreckAnimation>().finale.GetComponent<PlayerAura>().atk = atk * shipOriginHp;
                    wreckClone.GetComponent<WreckAnimation>().finale.GetComponent<PlayerAura>().num = num;
                    wreckClone.GetComponent<WreckAnimation>().finale.GetComponent<PlayerAura>().perRadius += ran;
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
            mpText.text = shipMp.ToString();
        }
    }
}