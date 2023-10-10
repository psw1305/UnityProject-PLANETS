using UnityEngine;
using System.Collections;

public class EnemyHitBox : MonoBehaviour
{
    public EnemyShipManager esm;
    public ParticleSystem hitEffect;
    public float hitTime = 0.3f;
    int cnt = 0;

    [HideInInspector] public bool isHit = false;
    bool isCount = true;

    [HideInInspector] public bool isRevenge = false;
    [HideInInspector] public float revengeDamage = 0;

    [Header("Collider")]
    public Collider2D armorColl;
    public Collider2D shieldColl;

    void Update()
    {
        if (hitEffect != null)
        {
            if (esm.isShield && isHit)
                StartCoroutine("Hitting");
            else if (!esm.isShield)
                isHit = true;
        }
    }

    IEnumerator Hitting()
    {
        isHit = false;
        hitEffect.Play();
        yield return new WaitForSeconds(hitTime);
    }

    IEnumerator Counting(float resistTime)
    {
        isCount = false;
        yield return new WaitForSeconds(resistTime + 3);

        cnt = 0;
        isCount = true;
    }

    public void ShieldActive()
    {
        if (armorColl != null && shieldColl != null)
        {
            armorColl.enabled = false;
            shieldColl.enabled = true;
        }
    }

    public void ShieldDeactive()
    {
        if (armorColl != null && shieldColl != null)
        {
            shieldColl.enabled = false;
            armorColl.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D damage)
    {
        // 기본 적군 피격
        EnemyHitDamage ehd = damage.GetComponent<EnemyHitDamage>();

        if (ehd != null && !ehd.ignore)
        {
            if (Random.value >= esm.dodge)
            {
                if (esm.isShield)
                {
                    isHit = true;
                    esm.ShieldDamage(ehd.bulletDamage);

                    if (!ehd.phase)
                        ehd.ExplosionShield();
                }
                else if (!esm.isShield)
                {
                    esm.Damage(ehd.bulletDamage);

                    if (!ehd.phase)
                        ehd.Explosion();
                }

                if (isRevenge)
                    revengeDamage += ehd.bulletDamage;

                if (isCount && ehd.skillCheck)
                {
                    cnt += 1;

                    if (cnt >= 30)
                    {
                        esm.eseg.EffectGenerator(ehd.skillName, ehd.skillDur, ehd.skillAtk, 0, ehd.skillNum);
                        StartCoroutine(Counting(ehd.skillDur));
                    }
                }

                CrowdControl cc = ehd.GetComponent<CrowdControl>();

                if (cc != null)
                    esm.eseg.EffectGenerator(cc.ccName, cc.dur, cc.atk, cc.ran, cc.num);
            }
        }

        // 차원 장애물 발동시 아군 피격 적용
        if (esm.obstacle)
        {
            PlayerHitDamage phd = damage.GetComponent<PlayerHitDamage>();

            if (phd != null && !phd.ignore)
            {
                float pDamage = phd.bulletDamage * esm.obsNum;

                if (esm.isShield && !phd.phase)
                {
                    isHit = true;
                    esm.ShieldDamage(pDamage);
                    phd.ExplosionShield();
                }
                else if (!esm.isShield && !phd.phase)
                {
                    esm.Damage(pDamage);
                    phd.Explosion();
                }
                else if (ehd.phase && phd.percent)
                {
                    esm.Damage(pDamage);
                    phd.Explosion();
                }
                else
                    esm.Damage(pDamage);
            }
        }

        // 쉐도우팽 티쓰 함선 충돌시 자폭 데미지 적용
        if (damage.transform.parent != null)
        {
            PlayerShipManager psm = damage.GetComponentInParent<PlayerShipManager>();

            if (psm != null && psm.ramming)
                psm.ExplosionDamage();
        }
    }
}
