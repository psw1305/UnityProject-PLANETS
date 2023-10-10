using UnityEngine;
using System.Collections;

public class PlayerHitBox : MonoBehaviour 
{
	public PlayerShipManager psm;
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
            if (psm.isShield && isHit)
                StartCoroutine("Hitting");
            else if (!psm.isShield)
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
        // 기본 아군 피격
        PlayerHitDamage phd = damage.GetComponent<PlayerHitDamage>();

		if (phd != null && !phd.ignore)
        {
            if (Random.value >= psm.dodge)
            {
                if (psm.isShield)
                {
                    isHit = true;
                    psm.ShieldDamage(phd.bulletDamage);

                    if (!phd.phase)
                        phd.ExplosionShield();
                }
                else if (!psm.isShield)
                {
                    psm.Damage(phd.bulletDamage);

                    if (!phd.phase)
                        phd.Explosion();
                }

                if (isRevenge)
                    revengeDamage += phd.bulletDamage;

                if (isCount && phd.skillCheck)
                {
                    cnt += 1;

                    if (cnt >= 30)
                    {
                        psm.seg.EffectGenerator(phd.skillName, phd.skillDur, phd.skillAtk, 0, phd.skillNum);
                        StartCoroutine(Counting(phd.skillDur));
                    }
                }

                CrowdControl cc = phd.GetComponent<CrowdControl>();

                if (cc != null)
                    psm.seg.EffectGenerator(cc.ccName, cc.dur, cc.atk, cc.ran, cc.num);
            }
        }

        // 쉐도우팽 티쓰 함선 충돌시 자폭 데미지 적용
        if (damage.transform.parent != null)
        {
            EnemyShipManager esm = damage.GetComponentInParent<EnemyShipManager>();

            if (esm != null && esm.ramming)
                esm.ExplosionDamage();
        }
    }
}
