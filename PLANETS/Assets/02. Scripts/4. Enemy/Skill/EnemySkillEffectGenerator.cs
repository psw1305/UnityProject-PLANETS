using UnityEngine;
using System.Collections;

public class EnemySkillEffectGenerator : MonoBehaviour
{
    float effectSize;
    int effectSort;
    EnemyShipManager esm;

    bool retire = false, defense   = false, charge    = false;
    bool poison = false, rapid     = false, protect   = false;
    bool slow   = false, stun      = false, overwhelm = false;
    bool reduce = false, dimension = false, weaken    = false;

    bool confuse = false, shield = false, mist  = false;
    bool evade   = false, cool   = false, chaos = false;

    void Awake()
    {
        esm = transform.parent.GetComponent<EnemyShipManager>();

        if (esm != null)
        {
            switch (esm.shipType)
            {
                case EnemyShipManager.ShipType.Destroyer:
                    effectSize = 1.0f;
                    effectSort = 93;
                    break;
                case EnemyShipManager.ShipType.Auxiliary:
                    effectSize = 1.0f;
                    effectSort = 73;
                    break;
                case EnemyShipManager.ShipType.Cruiser:
                    effectSize = 1.4f;
                    effectSort = 53;
                    break;
                case EnemyShipManager.ShipType.Carrier:
                    effectSize = 1.7f;
                    effectSort = 33;
                    break;
                case EnemyShipManager.ShipType.Battleship:
                    effectSize = 1.7f;
                    effectSort = 13;
                    break;
            }
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
            case "Dimension":       /*순양함B*/
                if (!dimension) StartCoroutine(Dimension(dur, atk));
                break;
            case "CoolDown":        /*전함B*/
                if (!cool) StartCoroutine(CoolDown(dur, atk));
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
        }
    }

    IEnumerator Confuse(float time) /*항공모함 기본 스킬*/
    {
        EffectTimeInstance("Confuse", time);

        confuse = true;
        esm.esmv.movePercent -= 0.25f;
        esm.esmv.turnPercent -= 0.25f;
        esm.et.accuracyPoint += 25;

        yield return new WaitForSeconds(time);

        confuse = false;
        esm.esmv.movePercent += 0.25f;
        esm.esmv.turnPercent += 0.25f;
        esm.et.accuracyPoint -= 25;
    }

    IEnumerator Dimension(float time, float stat) /*테란 순양함B*/
    {
        EffectTimeInstance("Dimension", time);

        dimension = true;
        esm.et.isEnable = false;
        esm.obstacle = true;
        esm.obsNum = stat;

        yield return new WaitForSeconds(time);

        dimension = false;
        esm.et.isEnable = true;
        esm.obstacle = false;
        esm.obsNum = 0;
    }

    IEnumerator CoolDown(float time, float plus) /*테란 전함B*/
    {
        cool = true;
        esm.RaceSkillCoolTime(plus);

        yield return new WaitForSeconds(time);

        cool = false;
        esm.RaceSkillCoolTime(0);
    }

    IEnumerator DefenseDown(float time, float stat) /*칼라스 구축함A*/
    {
        EffectTimeInstance("DefenseDown", time);

        reduce = true;
        esm.damagedPercent += stat;

        yield return new WaitForSeconds(time);

        reduce = false;
        esm.damagedPercent -= stat;
    }

    IEnumerator DefenseUp(float time, float stat) /*칼라스 구축함B*/
    {
        EffectTimeInstance("DefenseUp", time);

        defense = true;
        esm.damagedPercent -= stat;

        yield return new WaitForSeconds(time);

        defense = false;
        esm.damagedPercent += stat;
    }

    IEnumerator Protect(float time, float cnt) /*칼라스 지원함A*/
    {
        EffectTimeInstance("Protect", 3);

        protect = true;
        esm.isOverHp = true;
        esm.shipOp = cnt;
        esm.shipOriginOp = cnt;

        if (!esm.isDestroy)
        {
            esm.gage.GetComponent<UIGageManager>().opBar.gameObject.SetActive(true);
            esm.gage.GetComponent<UIGageManager>().opBar.fillAmount = 1.0f;
        }

        yield return new WaitForSeconds(time);

        protect = false;
        esm.isOverHp = false;
        esm.shipOp = 0;
        esm.shipOriginOp = 0;

        if (!esm.isDestroy)
            esm.gage.GetComponent<UIGageManager>().opBar.gameObject.SetActive(false);
    }

    IEnumerator Stasis(float time, float stat, float cnt) /*칼라스 지원함B*/
    {
        EffectTimeInstance("Stasis", time);

        retire = true;
        esm.core.SetActive(false);
        esm.esmv.isEnable = false;
        esm.et.isEnable   = false;

        for (int i = 0; i < stat; i++)
        {
            esm.Damage(-esm.shipOriginHp * 0.01f);
            yield return new WaitForSeconds(time / stat);
        }

        if (esm.isRetire)
            esm.EnemyShipRebirth();

        esm.AddCrewCheck(cnt);
        esm.CrewValue();

        retire = false;
        esm.core.SetActive(true);
        esm.esmv.isEnable = true;
        esm.et.isEnable   = true;
    }

    IEnumerator Stun(float time) /*칼라스 순양함A*/
    {
        EffectTimeInstance("Stun", time);

        stun = true;
        esm.esmv.isEnable = false;
        esm.et.isEnable   = false;

        yield return new WaitForSeconds(time);

        stun = false;
        esm.esmv.isEnable = true;
        esm.et.isEnable   = true;
    }

    IEnumerator RedWine(float time, float atk, float cnt) /*칼라스 항공모함A*/
    {
        EffectTimeInstance("RedWine", time);

        poison = true;
        esm.isShield = false;

        for (int i = 0; i < cnt; i++)
        {
            esm.Damage(esm.shipOriginHp * atk);
            yield return new WaitForSeconds(time / cnt);
        }

        poison = false;
        esm.isShield = true;
    }

    IEnumerator DefenseUp2(float time, float stat) /*칼라스 전함A*/
    {
        EffectTimeInstance("DefenseUp", time);

        esm.damagedPercent -= stat;
        yield return new WaitForSeconds(time);
        esm.damagedPercent += stat;
    }

    IEnumerator Evade(float time, float stat) /*칼라스 전함B*/
    {
        float alpha = esm.shipImage.GetComponent<SpriteRenderer>().color.a;

        evade = true;
        esm.dodge = stat;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.5f)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0.4f, t));
            esm.shipImage.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }

        yield return new WaitForSeconds(time);
        alpha = esm.shipImage.GetComponent<SpriteRenderer>().color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.5f)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 1.0f, t));
            esm.shipImage.GetComponent<SpriteRenderer>().color = newColor;
            yield return null;
        }

        evade = false;
        esm.dodge = 0;
    }

    // 쉐도우팽

    void CrewDamage(float stat, float cnt) /*쉐도우팽 기본 스킬 데미지 타입*/
    {
        esm.HullDamage(stat);

        esm.shipMp -= cnt;
        esm.CrewCheck();
        esm.CrewValue();
    }

    IEnumerator Slow(float time, float stat) /*쉐도우팽 지원함B*/
    {
        EffectTimeInstance("Overwhelm", time);

        slow = true;
        esm.esmv.movePercent -= stat;
        esm.esmv.turnPercent -= stat;
        esm.et.timePercent   += stat;

        yield return new WaitForSeconds(time);

        slow = false;
        esm.esmv.movePercent += stat;
        esm.esmv.turnPercent += stat;
        esm.et.timePercent   -= stat;
    }

    IEnumerator Overwhelm(float time, float stat) /*쉐도우팽 순양함A*/
    {
        EffectTimeInstance("Overwhelm", time);

        overwhelm = true;
        esm.damagedPercent += stat;
        esm.et.timePercent += stat;
        esm.et.damagePercent -= stat;
        esm.esmv.movePercent -= stat;
        esm.esmv.turnPercent -= stat;

        yield return new WaitForSeconds(time);

        overwhelm = false;
        esm.damagedPercent -= stat;
        esm.et.timePercent -= stat;
        esm.et.damagePercent += stat;
        esm.esmv.movePercent += stat;
        esm.esmv.turnPercent += stat;
    }

    IEnumerator AttackDown(float time, float stat) /*쉐도우팽 항공모함A*/
    {
        EffectTimeInstance("Slow", time);

        weaken = true;
        esm.et.damagePercent -= stat;

        yield return new WaitForSeconds(time);

        weaken = false;
        esm.et.damagePercent += stat;
    }

    IEnumerator Chaos(float time, float cnt) /*쉐도우팽 전함A*/
    {
        EffectTimeInstance("Overwhelm", time);

        chaos = true;
        esm.esmv.isRandom = true;
        esm.et.accuracyPoint += cnt;

        yield return new WaitForSeconds(time);

        chaos = false;
        esm.esmv.isRandom = false;
        esm.et.accuracyPoint -= cnt;
    }

    // 에이드리언

    void Shock(float stat) /*에이드리언 구축함A*/
    {
        esm.Damage(esm.shipHp * stat);
    }

    IEnumerator Rapid(float time, float atk) /*에이드리언 지원함B*/
    {
        rapid = true;
        esm.et.timePercent -= atk;

        yield return new WaitForSeconds(time);

        rapid = false;
        esm.et.timePercent += atk;
    }

    IEnumerator DefenseDown2(float time, float stat) /*에이드리언 항공모함A*/
    {
        EffectTimeInstance("DefenseDown", time);

        esm.damagedPercent += stat;
        yield return new WaitForSeconds(time);
        esm.damagedPercent -= stat;
    }

    // 하빈저

    IEnumerator ShieldDown(float time) /*하빈저 구축함A*/
    {
        shield = true;

        if (esm.shipMp > 0)
        {
            esm.uncharge = true;
            esm.isShield = false;

            esm.shieldTime = esm.shieldOriginTime;
            esm.shipAp = 0;
            esm.ShieldValue();
        }

        yield return new WaitForSeconds(time);

        if (esm.shipMp > 0)
            esm.uncharge = false;

        shield = false;
    }

    void ShieldCharge(float stat) /*하빈저 지원함A*/
    {
        esm.isShield = true;
        esm.shipAp += stat;
        esm.ShieldValue();
    }

    IEnumerator ThunderCloud(float time) /*하빈저 지원함B*/
    {
        charge = true;
        esm.shieldOriginTime *= 0.5f;

        yield return new WaitForSeconds(time);

        esm.shieldOriginTime *= 2f;
        charge = false;
    }

    IEnumerator Mist(float time, float atk, float cnt) /*하빈저 항공모함A*/
    {
        EffectTimeInstance("Mist", time);

        mist = true;
        esm.et.timePercent += atk;

        for (int i = 0; i < cnt; i++)
        {
            esm.Damage(esm.shipOriginHp * 0.02f);
            yield return new WaitForSeconds(time / cnt);
        }

        mist = false;
        esm.et.timePercent -= atk;
    }
}
