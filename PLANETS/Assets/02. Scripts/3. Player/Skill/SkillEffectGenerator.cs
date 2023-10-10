using UnityEngine;
using System.Collections;

public class SkillEffectGenerator : MonoBehaviour
{
    float effectSize;
    int effectSort;
    PlayerShipManager psm;

    bool repair = false, strong = false, defense   = false;
    bool poison = false, rapid  = false, protect   = false;
    bool weaken = false, stun   = false, overwhelm = false;
    bool reduce = false, leader = false, retire    = false;
    bool evade  = false, charge = false, chaos     = false;

    bool confuse = false, shield = false, mist = false;
    bool slow    = false;

    void Awake()
    {
        psm = transform.parent.GetComponent<PlayerShipManager>();

        switch (psm.shipType)
        {
            case PlayerShipManager.ShipType.Destroyer:
                effectSize = 1.0f;
                effectSort = 103;
                break;
            case PlayerShipManager.ShipType.Auxiliary:
                effectSize = 1.0f;
                effectSort = 83;
                break;
            case PlayerShipManager.ShipType.Cruiser:
                effectSize = 1.4f;
                effectSort = 63;
                break;
            case PlayerShipManager.ShipType.Carrier:
                effectSize = 1.7f;
                effectSort = 43;
                break;
            case PlayerShipManager.ShipType.Battleship:
                effectSize = 1.7f;
                effectSort = 23;
                break;
        }
    }

    void EffectInstance(string effectType)
    {
        GameObject effect = Resources.Load("Effect/" + effectType) as GameObject;

        if (effect != null)
        {
            GameObject stateEffect = Instantiate(effect, transform.position, Quaternion.identity) as GameObject;
            stateEffect.transform.parent = transform;
            stateEffect.transform.localScale = new Vector3(effectSize, effectSize, effectSize);
            stateEffect.GetComponent<EffectManager>().EffectSortingOrder(effectSort);
            stateEffect.GetComponent<EffectManager>().EffectCheck(true);
        }
    }

    public void EffectTimeInstance(string effectType, float effectTime)
    {
        GameObject effect = Resources.Load("Effect/" + effectType) as GameObject;

        if (effect != null)
        {
            GameObject effectClone = Instantiate(effect, transform.position, Quaternion.identity) as GameObject;
            effectClone.transform.parent = transform;
            effectClone.transform.localScale = new Vector3(effectSize, effectSize, effectSize);
            effectClone.GetComponent<EffectManager>().EffectSortingOrder(effectSort);
            effectClone.GetComponent<EffectManager>().effectTime = effectTime;
            effectClone.GetComponent<EffectManager>().Effect();
        }
    }

    public void EffectGenerator(string effectType, float dur, float atk, float ran, float num)
    {
        switch (effectType)
        {
            // 항공모함 기본스킬
            case "Confuse":
                if (!confuse) StartCoroutine(Confuse(dur));
                break;

            // 테란 스킬
            case "Strong":          /*구축함B*/
                if (!strong) StartCoroutine(Strong(dur, atk));
                break;
            case "Repair":          /*지원함A*/
                if (!repair) StartCoroutine(Repair(dur, atk));
                break;
            case "Leadership":      /*전함A*/
                if (!leader) StartCoroutine(Leadership(dur, atk));
                break;

            // 칼라스 스킬
            case "DefenseDown":     /*구축함A*/
                if (!reduce) StartCoroutine(DefenseDown(dur, atk));
                break;
            case "DefenseUp":       /*구축함B*/
                if (!defense) StartCoroutine(DefenseUp(dur, atk));
                break;
            case "Protect":         /*지원함A*/
                if (!protect) StartCoroutine(Protect(dur, num));
                break;
            case "Stasis":          /*지원함B*/
                if (!retire) StartCoroutine(Stasis(dur, atk, num));
                break;
            case "Stun":            /*순양함A*/
                if (!stun) StartCoroutine(Stun(dur));
                break;
            case "RedWine":         /*항공모함A*/
                if (!poison) StartCoroutine(RedWine(dur, atk, num));
                break;
            case "DefenseUp2":      /*전함A*/
                StartCoroutine(DefenseUp2(dur, atk));
                break;
            case "Evade":           /*전함B*/
                if (!evade) StartCoroutine(Evade(dur, atk));
                break;

            // 쉐도우팽 스킬
            case "CrewDamage":      /*쉐도우팽 기본 스킬 데미지 타입*/
                CrewDamage(atk, num);
                break;
            case "Slow":            /*지원함B*/
                if (!slow) StartCoroutine(Slow(dur, atk));
                break;
            case "Overwhelm":       /*순양함A*/
                if (!overwhelm) StartCoroutine(Overwhelm(dur, atk));
                break;
            case "AttackDown":      /*항공모함A*/
                if (!weaken) StartCoroutine(AttackDown(dur, atk));
                break;
            case "Chaos":           /*전함A*/
                if (!chaos) StartCoroutine(Chaos(dur, num));
                break;

            // 에이드리언 스킬
            case "Shock":           /*구축함A*/
                Shock(atk);
                break;
            case "Rapid":           /*지원함B*/
                if (!rapid) StartCoroutine(Rapid(dur, atk));
                break;
            case "DefenseDown2":    /*항공모함A*/
                StartCoroutine(DefenseDown2(dur, atk));
                break;

            // 하빈저 스킬
            case "ShieldDown":      /*구축함A*/
                if (!shield) StartCoroutine(ShieldDown(dur));
                break;
            case "ShieldCharge":    /*지원함A*/
                ShieldCharge(atk);
                break;
            case "ThunderCloud":    /*지원함B*/
                if (!charge) StartCoroutine(ThunderCloud(dur));
                break;
            case "Mist":            /*항공모함A*/
                if (!mist) StartCoroutine(Mist(dur, atk, num));
                break;

            // 장교 스킬
            case "AddCrew":         /*보급장교1*/
                AddCrew(num);
                break;
        }
    }

    IEnumerator Confuse(float time) /*항공모함 기본 스킬*/
    {
        EffectTimeInstance("Confuse", time);

        confuse = true;
        psm.psmv.movePercent -= 0.25f;
        psm.psmv.turnPercent -= 0.25f;
        psm.pt.accuracyPoint += 25;

        yield return new WaitForSeconds(time);

        confuse = false;
        psm.psmv.movePercent += 0.25f;
        psm.psmv.turnPercent += 0.25f;
        psm.pt.accuracyPoint -= 25;
    }

    // 테란

    IEnumerator Strong(float time, float stat) /*테란 구축함B*/
    {
        EffectTimeInstance("Strong", time);

        strong = true;
        psm.pt.damagePercent += stat;

        yield return new WaitForSeconds(time);

        strong = false;
        psm.pt.damagePercent -= stat;
    }

    IEnumerator Repair(float time, float stat) /*테란 지원함A*/
    {
        EffectTimeInstance("Repair", time);
        repair = true;

        psm.shipHp += stat;
        psm.HealthValue();

        yield return new WaitForSeconds(time);
        repair = false;
    }

    IEnumerator Leadership(float time, float stat) /*테란 전함A*/
    {
        EffectTimeInstance("Leadership", time);

        leader = true;
        psm.damagedPercent -= stat;
        psm.pt.timePercent -= stat;
        psm.pt.damagePercent += stat;
        psm.psmv.movePercent += stat;
        psm.psmv.turnPercent += stat;

        yield return new WaitForSeconds(time);

        leader = false;
        psm.damagedPercent += stat;
        psm.pt.timePercent += stat;
        psm.pt.damagePercent -= stat;
        psm.psmv.movePercent -= stat;
        psm.psmv.turnPercent -= stat;
    }

    // 칼라스

    IEnumerator DefenseDown(float time, float stat) /*칼라스 구축함A*/
    {
        EffectTimeInstance("DefenseDown", time);

        reduce = true;
        psm.damagedPercent += stat;

        yield return new WaitForSeconds(time);

        reduce = false;
        psm.damagedPercent -= stat;
    }

    IEnumerator DefenseUp(float time, float stat) /*칼라스 구축함B*/
    {
        EffectTimeInstance("DefenseUp", time);

        defense = true;
        psm.damagedPercent -= stat;

        yield return new WaitForSeconds(time);

        defense = false;
        psm.damagedPercent += stat;
    }

    IEnumerator Protect(float time, float cnt) /*칼라스 지원함A*/
    {
        EffectTimeInstance("Protect", 3);

        protect = true;
        psm.isOverHp = true;
        psm.shipOp = cnt;
        psm.shipOriginOp = cnt;

        if (!psm.isDestroy)
        {
            psm.gage.GetComponent<UIGageManager>().opBar.gameObject.SetActive(true);
            psm.gage.GetComponent<UIGageManager>().opBar.fillAmount = 1.0f;
        }

        yield return new WaitForSeconds(time);

        protect = false;
        psm.isOverHp = false;
        psm.shipOp = 0;
        psm.shipOriginOp = 0;

        if (!psm.isDestroy)
            psm.gage.GetComponent<UIGageManager>().opBar.gameObject.SetActive(false);
    }

    IEnumerator Stasis(float time, float stat, float cnt) /*칼라스 지원함B*/
    {
        EffectTimeInstance("Stasis", time);

        retire = true;
        psm.core.SetActive(false);
        psm.psmv.isEnable = false;
        psm.pt.isEnable = false;

        for (int i = 0; i < stat; i++)
        {
            psm.Damage(-psm.shipOriginHp * 0.01f);
            yield return new WaitForSeconds(time / stat);
        }

        if (psm.isRetire)
            psm.PlayerShipRebirth();

        psm.AddCrewCheck(cnt);
        psm.CrewValue();

        retire = false;
        psm.core.SetActive(true);
        psm.psmv.isEnable = true;
        psm.pt.isEnable = true;
    }

    IEnumerator Stun(float time) /*칼라스 순양함A*/
    {
        EffectTimeInstance("Stun", time);

        stun = true;
        psm.psmv.isEnable = false;
        psm.pt.isEnable = false;

        yield return new WaitForSeconds(time);

        stun = false;
        psm.psmv.isEnable = true;
        psm.pt.isEnable = true;
    }

    IEnumerator RedWine(float time, float atk, float cnt) /*칼라스 항공모함A*/
    {
        EffectTimeInstance("RedWine", time);

        poison = true;
        psm.isShield = false;

        for (int i = 0; i < cnt; i++)
        {
            psm.Damage(psm.shipOriginHp * atk);
            yield return new WaitForSeconds(time / cnt);
        }

        poison = false;
        psm.isShield = true;
    }

    IEnumerator DefenseUp2(float time, float stat) /*칼라스 전함A*/
    {
        EffectTimeInstance("DefenseUp", time);

        psm.damagedPercent -= stat;
        yield return new WaitForSeconds(time);
        psm.damagedPercent += stat;
    }

    IEnumerator Evade(float time, float stat) /*칼라스 전함B*/
    {
        float alpha = psm.shipImage.GetComponent<SpriteRenderer>().color.a;

        evade = true;
        psm.dodge = stat;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.5f)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0.4f, t));
            psm.shipImage.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }

        yield return new WaitForSeconds(time);
        alpha = psm.shipImage.GetComponent<SpriteRenderer>().color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.5f)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 1.0f, t));
            psm.shipImage.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }

        evade = false;
        psm.dodge = 0;
    }

    // 쉐도우팽

    void CrewDamage(float stat, float cnt) /*쉐도우팽 기본 스킬 데미지 타입*/
    {
        psm.HullDamage(stat);

        psm.shipMp -= cnt;
        psm.CrewCheck();
        psm.CrewValue();
    }

    IEnumerator Slow(float time, float stat) /*쉐도우팽 지원함B*/
    {
        EffectTimeInstance("Overwhelm", time);

        slow = true;
        psm.psmv.movePercent -= stat;
        psm.psmv.turnPercent -= stat;
        psm.pt.timePercent   += stat;

        yield return new WaitForSeconds(time);

        slow = false;
        psm.psmv.movePercent += stat;
        psm.psmv.turnPercent += stat;
        psm.pt.timePercent   -= stat;
    }

    IEnumerator Overwhelm(float time, float stat) /*쉐도우팽 순양함A*/
    {
        EffectTimeInstance("Overwhelm", time);

        overwhelm = true;
        psm.damagedPercent += stat;
        psm.pt.timePercent += stat;
        psm.pt.damagePercent -= stat;
        psm.psmv.movePercent -= stat;
        psm.psmv.turnPercent -= stat;

        yield return new WaitForSeconds(time);

        overwhelm = false;
        psm.damagedPercent -= stat;
        psm.pt.timePercent -= stat;
        psm.pt.damagePercent += stat;
        psm.psmv.movePercent += stat;
        psm.psmv.turnPercent += stat;
    }

    IEnumerator AttackDown(float time, float stat) /*쉐도우팽 항공모함A*/
    {
        EffectTimeInstance("Slow", time);

        weaken = true;
        psm.pt.damagePercent -= stat;

        yield return new WaitForSeconds(time);

        weaken = false;
        psm.pt.damagePercent += stat;
    }

    IEnumerator Chaos(float time, float cnt) /*쉐도우팽 전함A*/
    {
        EffectTimeInstance("Overwhelm", time);

        chaos = true;
        psm.psmv.isRandom = true;
        psm.pt.accuracyPoint += cnt;

        yield return new WaitForSeconds(time);

        chaos = false;
        psm.psmv.isRandom = false;
        psm.pt.accuracyPoint -= cnt;
    }

    // 에이드리언

    void Shock(float stat) /*에이드리언 구축함A*/
    {
        psm.Damage(psm.shipOriginHp * stat);
    }

    IEnumerator Rapid(float time, float atk) /*에이드리언 지원함B*/
    {
        rapid = true;
        psm.pt.timePercent -= atk;

        yield return new WaitForSeconds(time);

        rapid = false;
        psm.pt.timePercent += atk;
    }

    IEnumerator DefenseDown2(float time, float stat) /*에이드리언 항공모함A*/
    {
        EffectTimeInstance("DefenseDown", time);

        psm.damagedPercent += stat;
        yield return new WaitForSeconds(time);
        psm.damagedPercent -= stat;
    }

    // 하빈저

    IEnumerator ShieldDown(float time) /*하빈저 구축함A*/
    {
        shield = true;

        if (psm.shipMp > 0)
        {
            psm.uncharge = true;
            psm.isShield = false;

            psm.shieldTime = psm.shieldOriginTime;
            psm.shipAp = 0;
            psm.ShieldValue();
        }

        yield return new WaitForSeconds(time);

        if (psm.shipMp > 0)
            psm.uncharge = false;

        shield = false;
    }

    void ShieldCharge(float stat) /*하빈저 지원함A*/
    {
        psm.isShield = true;
        psm.shipAp += stat;
        psm.ShieldValue();
    }

    IEnumerator ThunderCloud(float time) /*하빈저 지원함B*/
    {
        charge = true;
        psm.shieldOriginTime *= 0.5f;

        yield return new WaitForSeconds(time);

        charge = false;
        psm.shieldOriginTime *= 2f;
    }

    IEnumerator Mist(float time, float atk, float cnt) /*하빈저 항공모함A*/
    {
        EffectTimeInstance("Mist", time);

        mist = true;
        psm.pt.timePercent += atk;

        for (int i = 0; i < cnt; i++)
        {
            psm.Damage(psm.shipOriginHp * 0.02f);
            yield return new WaitForSeconds(time / cnt);
        }

        mist = false;
        psm.pt.timePercent -= atk;
    }

    // 장교 스킬
    void AddCrew(float cnt) /*보급장교1*/
    {
        EffectTimeInstance("AddCrew", 2);

        if (psm.isRetire)
            psm.PlayerShipRebirth();
        
        psm.AddCrewCheck(cnt);
        psm.CrewValue();
    }
}
