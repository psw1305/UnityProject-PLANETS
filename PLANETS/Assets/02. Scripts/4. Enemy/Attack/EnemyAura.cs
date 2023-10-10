using System.Collections;
using UnityEngine;

public class EnemyAura : MonoBehaviour
{
    public bool dot = false, heal = false;
    public float radius, damageCount, dur;
    public string ccName;

    [HideInInspector] public float damage, atk, ran, num;
    [HideInInspector] public float perRadius = 1.0f;

    [HideInInspector] public bool isBox = false;
    [HideInInspector] public Vector2 boxSize;

    bool isDamage = false;
    int hitNum;

    void OnDisable()
    {
        isDamage = false;
        hitNum = 0;
    }

    void Update()
    {
        if (!isDamage && hitNum < damageCount)
            StartCoroutine("AuraDamage");
    }

    IEnumerator AuraDamage()
    {
        isDamage = true;

        hitNum += 1;
        Collider2D[] hits;

        if (!isBox)
            hits = Physics2D.OverlapCircleAll(transform.position, radius * perRadius);
        else
            hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);

        foreach (Collider2D hit in hits)
        {
            if (!heal)
            {
                PlayerHitBox phb = hit.GetComponent<PlayerHitBox>();

                if (phb != null)
                {
                    if (phb.psm.isShield)
                    {
                        phb.isHit = true;
                        phb.psm.ShieldDamage(damage);
                    }
                    else
                        phb.psm.Damage(damage);

                    if (phb.isRevenge)
                        phb.revengeDamage += damage;

                    if (ccName != "None")
                        phb.psm.seg.EffectGenerator(ccName, dur, atk, ran, num);
                }

                PlayerFighterShipManager pfsm = hit.GetComponent<PlayerFighterShipManager>();

                if (pfsm != null)
                    pfsm.Damage(damage);
            }
            else
            {
                EnemyHitBox ehb = hit.GetComponent<EnemyHitBox>();

                if (ehb != null)
                {
                    ehb.esm.Damage(-damage);

                    if (ccName != "None")
                        ehb.esm.eseg.EffectGenerator(ccName, dur, atk, ran, num);
                }
            }
        }

        yield return new WaitForSeconds(dur / damageCount);

        if (dot)
            isDamage = false;
    }
}
