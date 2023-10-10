using System.Collections;
using UnityEngine;

public class PlayerFighterHitDamage : MonoBehaviour
{
    delegate void Move();
    event Move Action;
    bool isDestroy = false;

    public bool notPool = false;
    public Transform moveTarget;
    [HideInInspector] public Transform host;
    GameObject closest;

    [Header("Effect")]
    public ParticleSystem[] effects;
    public int[] effectSortingOrders;

    [Header("Bullet")]
    public SpriteRenderer bulletImage;
    public float bulletDamage;
    public float bulletSpeed;
    public float speedLimit;
    public float bulletTurnSpeed;
    public float bulletTimer;

    Vector2 bulletDir;
    float bulletOriginDamage;
    float bulletOriginSpeed;
    float bulletOriginTurnSpeed;

    [Header("Explosion")]
    public ParticleSystem explosion;
    public float explosionTime;

    public GameObject FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerFighter");
        float distance = Mathf.Infinity;

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 diff = players[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;
            float radius = Vector2.Distance(transform.position, host.position);

            if (radius < 45 && curDistance < distance)
            {
                closest = players[i];
                distance = curDistance;
            }
        }

        return closest;
    }

    void EffectCheck(bool check)
    {
        if (effects != null)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                ParticleSystem ps = effects[i];
                var em = ps.emission;
                em.enabled = check;
            }
        }
    }

    void Start()
    {
        if (effects != null)
        {
            for (int i = 0; i < effects.Length; i++)
            {
                ParticleSystem ps = effects[i];
                ps.GetComponent<Renderer>().sortingOrder = effectSortingOrders[i];
            }
        }

        Action += BulletAiming;
        Action += NormalMoving;
        Action += AccelMoving;

        //StartCoroutine("BulletExplosionTimer");
    }

    void FixedUpdate()
    {
        if (host != null && FindClosestPlayer() != null && !isDestroy)
        {
            Action();
            gameObject.GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;
            BulletDisable();
        }
        else
            Explosion();
    }

    void BulletAiming()
    {
        Vector3 direction = FindClosestPlayer().transform.position - transform.position;
        direction.Normalize();
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, rotZ), bulletTurnSpeed * Time.deltaTime);
    }

    void NormalMoving()
    {
        bulletDir = moveTarget.position - transform.position;
        bulletDir.Normalize();
    }

    void AccelMoving()
    {
        bulletSpeed = Mathf.Lerp(bulletSpeed, speedLimit, Time.deltaTime);
    }

    void BulletDisable()
    {
        if (host != null)
        {
            float distance = Vector2.Distance(transform.position, host.position);

            if (distance >= 45)
                StartCoroutine("BulletExplosion");
        }
    }

    IEnumerator BulletExplosionTimer()
    {
        yield return new WaitForSeconds(bulletTimer);
        Explosion();
    }

    public void Explosion()
    {
        StartCoroutine("BulletExplosion");
    }

    IEnumerator BulletExplosion()
    {
        isDestroy = true;

        bulletSpeed = 0;
        bulletTurnSpeed = 0;

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        EffectCheck(false);
        bulletImage.gameObject.SetActive(false);

        explosion.gameObject.SetActive(true);
        yield return new WaitForSeconds(explosionTime);
        Destroy(gameObject);
    }

    void BulletDestroyCheck()
    {
        if (!notPool)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        EffectCheck(true);
        GetComponent<Collider2D>().enabled = true;

        bulletOriginDamage    = bulletDamage;
        bulletOriginSpeed     = bulletSpeed;
        bulletOriginTurnSpeed = bulletTurnSpeed;
    }

    void OnDisable()
    {
        CancelInvoke();
        isDestroy = false;

        if (bulletImage != null)
            bulletImage.gameObject.SetActive(true);

        bulletDamage    = bulletOriginDamage;
        bulletSpeed     = bulletOriginSpeed;
        bulletTurnSpeed = bulletOriginTurnSpeed;
    }
}
