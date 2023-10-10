using UnityEngine;
using System.Collections;

public class PlayerAura : MonoBehaviour
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
                EnemyHitBox ehb = hit.GetComponent<EnemyHitBox>();

                if (ehb != null)
                {
                    if (ehb.esm.isShield)
                    {
                        ehb.isHit = true;
                        ehb.esm.ShieldDamage(damage);
                    }
                    else
                        ehb.esm.Damage(damage);

                    if (ehb.isRevenge)
                        ehb.revengeDamage += damage;

                    if (ccName != "None")
                        ehb.esm.eseg.EffectGenerator(ccName, dur, atk, ran, num);
                }

                EnemyFighterShipManager efsm = hit.GetComponent<EnemyFighterShipManager>();

                if (efsm != null)
                    efsm.Damage(damage);
            }
            else
            {
                PlayerHitBox phb = hit.GetComponent<PlayerHitBox>();

                if (phb != null)
                {
                    phb.psm.Damage(-damage);

                    if (ccName != "None")
                        phb.psm.seg.EffectGenerator(ccName, dur, atk, ran, num);
                }
            }
        }

        yield return new WaitForSeconds(dur / damageCount);

        if (dot)
            isDamage = false;
    }
}
