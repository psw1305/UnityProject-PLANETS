using System.Collections;
using UnityEngine;

public class RadarSystem : MonoBehaviour
{
    public bool FindEnemyFighter;
    bool isDestroy = false;

    public float radius, delay;
    public GameObject missile;

    void Update()
    {
        if (!isDestroy)
            StartCoroutine("RadarActive");
    }

    IEnumerator RadarActive()
    {
        isDestroy = true;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit != null)
            {
                if (!FindEnemyFighter)
                {
                    PlayerFighterShipManager pfsm = hit.GetComponent<PlayerFighterShipManager>();

                    if (pfsm != null)
                    {
                        GameObject clone = Instantiate(missile, transform.position, transform.rotation) as GameObject;
                        clone.GetComponent<PlayerFighterHitDamage>().host = transform;
                        yield return new WaitForSeconds(delay);
                    }
                }
                else
                {
                    EnemyFighterShipManager efsm = hit.GetComponent<EnemyFighterShipManager>();

                    if (efsm != null)
                    {
                        GameObject clone = Instantiate(missile, transform.position, transform.rotation) as GameObject;
                        clone.GetComponent<EnemyFighterHitDamage>().host = transform;
                        yield return new WaitForSeconds(delay);
                    }
                }
            }
        }

        isDestroy = false;
    }
}
