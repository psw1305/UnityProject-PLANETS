using System.Collections;
using UnityEngine;

public class BossHitBox : MonoBehaviour
{
    public EnemyBossManager esm;
    public ParticleSystem hitEffect;
    public float hitTime = 0.3f;
    [HideInInspector] public bool isHit = false, isCount = false;

    [Header("Collider")]
    public Collider2D armorColl;
    public Collider2D shieldColl;

    void Update()
    {
        if (esm != null)
        {
            if (hitEffect != null)
            {
                if (esm.isShield && isHit)
                    StartCoroutine("Hitting");
                else if (!esm.isShield)
                    isHit = true;
            }
        }
    }

    IEnumerator Hitting()
    {
        isHit = false;
        hitEffect.Play();
        yield return new WaitForSeconds(hitTime);
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
        EnemyHitDamage ehd = damage.GetComponent<EnemyHitDamage>();

        if (ehd != null && !ehd.ignore)
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
           
        }

        if (damage.transform.parent != null)
        {
            PlayerShipManager psm = damage.transform.parent.GetComponent<PlayerShipManager>();

            if (psm != null && psm.selfDestruct)
            {
                psm.selfDestruct = false;
                esm.Damage(psm.shipOriginHp * 0.5f);
                psm.PlayerShipEnd();
            }
        }
    }
}
