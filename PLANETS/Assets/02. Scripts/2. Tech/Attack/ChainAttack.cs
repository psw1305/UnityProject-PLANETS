using System.Collections;
using UnityEngine;

public class ChainAttack : MonoBehaviour
{
    public bool targetEnemy;
    public GameObject impact;
    public float damage;
    float minusDamage;

    GameObject closest;
    GameObject first, second;

    void Start()
    {
        minusDamage = damage * 0.3f;

        if (!targetEnemy)
            StartCoroutine("ChainSystem_FindPlayer");
        else
            StartCoroutine("ChainSystem_FindEnemy");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!targetEnemy)
        {
            PlayerHitBox phb = collision.GetComponent<PlayerHitBox>();

            if (phb != null)
            {
                if (phb.psm.isShield)
                {
                    phb.isHit = true;
                    phb.psm.ShieldDamage(damage);
                }
                else
                    phb.psm.Damage(damage);

                Instantiate(impact, transform.position, transform.rotation);
                damage -= minusDamage;
            }
        }
        else
        {
            EnemyHitBox ehb = collision.GetComponent<EnemyHitBox>();

            if (ehb != null)
            {
                if (ehb.esm.isShield)
                {
                    ehb.isHit = true;
                    ehb.esm.ShieldDamage(damage);
                }
                else
                    ehb.esm.Damage(damage);

                Instantiate(impact, transform.position, transform.rotation);
                damage -= minusDamage;
            }
        }
    }

    // 타겟이 아군함선일시
    public GameObject FindClosestFirstPlayer()
    {
        float distance = Mathf.Infinity;
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerShip");

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 diff = players[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                closest = players[i];
                distance = curDistance;
            }
        }

        first = closest;
        return closest;
    }

    public GameObject FindClosestSecondPlayer()
    {
        float distance = Mathf.Infinity;
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerShip");

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 diff = players[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance && first != players[i])
            {
                closest = players[i];
                distance = curDistance;
            }
        }

        second = closest;
        return closest;
    }

    public GameObject FindClosestThirdPlayer()
    {
        float distance = Mathf.Infinity;
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerShip");

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 diff = players[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance && first != players[i] && second != players[i])
            {
                closest = players[i];
                distance = curDistance;
            }
        }

        return closest;
    }

    IEnumerator ChainSystem_FindPlayer()
    {
        yield return new WaitForSeconds(0.3f);

        if (FindClosestFirstPlayer() != null)
            transform.position = FindClosestFirstPlayer().transform.position;
        yield return new WaitForSeconds(0.3f);

        if (FindClosestSecondPlayer() != null)
            transform.position = FindClosestSecondPlayer().transform.position;
        yield return new WaitForSeconds(0.3f);

        if (FindClosestThirdPlayer() != null)
            transform.position = FindClosestThirdPlayer().transform.position;
        yield return new WaitForSeconds(0.4f);

        Destroy(gameObject);
    }

    // 타겟이 적함선일시
    public GameObject FindClosestFirstEnemy()
    {
        float distance = Mathf.Infinity;
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("EnemyShip");

        for (int i = 0; i < enemys.Length; i++)
        {
            Vector3 diff = enemys[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance)
            {
                closest = enemys[i];
                distance = curDistance;
            }
        }

        first = closest;
        return closest;
    }

    public GameObject FindClosestSecondEnemy()
    {
        float distance = Mathf.Infinity;
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("EnemyShip");

        for (int i = 0; i < enemys.Length; i++)
        {
            Vector3 diff = enemys[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance && first != enemys[i])
            {
                closest = enemys[i];
                distance = curDistance;
            }
        }

        second = closest;
        return closest;
    }

    public GameObject FindClosestThirdEnemy()
    {
        float distance = Mathf.Infinity;
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("EnemyShip");

        for (int i = 0; i < enemys.Length; i++)
        {
            Vector3 diff = enemys[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (curDistance < distance && first != enemys[i] && second != enemys[i])
            {
                closest = enemys[i];
                distance = curDistance;
            }
        }

        return closest;
    }

    IEnumerator ChainSystem_FindEnemy()
    {
        yield return new WaitForSeconds(0.3f);

        if (FindClosestFirstEnemy() != null)
            transform.position = FindClosestFirstEnemy().transform.position;
        yield return new WaitForSeconds(0.3f);

        if (FindClosestSecondEnemy() != null)
            transform.position = FindClosestSecondEnemy().transform.position;
        yield return new WaitForSeconds(0.3f);

        if (FindClosestThirdEnemy() != null)
            transform.position = FindClosestThirdEnemy().transform.position;
        yield return new WaitForSeconds(0.4f);

        Destroy(gameObject);
    }
}
