using System.Collections;
using UnityEngine;

public class DefenseSystem : MonoBehaviour
{
    public bool FindEnemyBullet;
    bool isDestroy = false;

    public float radius, damageCount;
    [HideInInspector] public float perRadius = 1.0f;

    int hitNum = 0;

    void Update()
    {
        if (!isDestroy && hitNum < damageCount)
            StartCoroutine("DefenseActive");
        else if (!isDestroy && hitNum >= damageCount)
        {
            isDestroy = true;
            GetComponent<EffectManager>().EffectCheck(false);
        }
    }

    IEnumerator DefenseActive()
    {
        isDestroy = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius * perRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit != null)
            {
                if (!FindEnemyBullet)
                {
                    EnemyHitDamage ehd = hit.GetComponent<EnemyHitDamage>();

                    if (ehd != null)
                    {
                        hitNum += 1;
                        ehd.Explosion();
                        yield return new WaitForSeconds(0.01f);
                    }
                }
                else
                {
                    PlayerHitDamage phd = hit.GetComponent<PlayerHitDamage>();

                    if (phd != null)
                    {
                        hitNum += 1;
                        phd.Explosion();
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
        }

        isDestroy = false;
    }
}
