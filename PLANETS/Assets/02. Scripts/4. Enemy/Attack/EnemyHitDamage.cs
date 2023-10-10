using UnityEngine;
using System.Collections;

public class EnemyHitDamage : MonoBehaviour 
{
    delegate void Move();
    event Move Action;
    bool isDestroy = false;

    public bool notPool = false;
    public Transform moveTarget;
    [HideInInspector] public Vector3 destination;
    [HideInInspector] public Transform host;
    [HideInInspector] public float hostRadius = 0;
    GameObject closest;

    [Header("Move Action")]
    public bool accele  = false;
    public bool burst   = false;
    public bool ignore  = false;
    public bool phase   = false;
    public bool chase   = false;
    public bool timer   = false;

    [Header("Effect")]
    public ParticleSystem[] effects;
    public int[] effectSortingOrders;
    public GameObject reinforceEffect;

    [Header("Bullet")]
    public SpriteRenderer bulletImage;
    public float bulletDamage;
    public float bulletSpeed;
    public float bulletSpreadSpeed;
    public float speedLimit;
    public float bulletTurnSpeed;
    public float bulletTimer;

    [HideInInspector] public bool skillCheck = false;
    [HideInInspector] public string skillName = "None";
    [HideInInspector] public float skillDur, skillAtk, skillNum;

    Vector2 bulletDir;
    float bulletOriginDamage;
    float bulletOriginSpeed;
    float bulletOriginTurnSpeed;

    [Header("Explosion")]
    public ParticleSystem explosion;
    public float explosionTime;
    public Vector2 explosionDelay;

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

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("EnemyShip");
        float distance = Mathf.Infinity;

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

        return closest;
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

		if (accele) Action += AccelMoving;

        if (timer)
            StartCoroutine("BulletExplosionTimer");
    }
	
	void FixedUpdate()
	{
		if (!isDestroy) 
		{
			Action();
            gameObject.GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;
            BulletDisable();

            if (!chase && bulletSpeed > 0)
                StartCoroutine("BulletTurnTimer");
        }
	}

    IEnumerator BulletTurnTimer()
    {
        yield return new WaitForSeconds(1);
        bulletTurnSpeed = 0;
    }

    void BulletAiming() 
	{
        if (FindClosestEnemy() != null && chase)
            destination = FindClosestEnemy().transform.position;
        else if (FindClosestEnemy() == null && chase)
            Explosion();

		Vector3 direction = destination - transform.position;
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

    IEnumerator BulletExplosionTimer()
    {
        yield return new WaitForSeconds(bulletTimer);
        Explosion();
    }

    public void Explosion()
	{
		StartCoroutine("Destroy");
	}

    public void ExplosionShield()
    {
        ShieldDestroy();
    }

    IEnumerator Destroy()
	{
		bulletTurnSpeed = 0;
        transform.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(Random.Range(explosionDelay.x, explosionDelay.y));

        StartCoroutine("BulletExplosion");
    }

    void ShieldDestroy()
    {
        bulletTurnSpeed = 0;
        transform.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("BulletExplosion");
    }

    IEnumerator BulletExplosion()
    {
        bulletSpeed = 0;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        isDestroy = true;
        EffectCheck(false);

        if (bulletImage != null)
            bulletImage.gameObject.SetActive(false);

        if (explosion != null)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            explosion.gameObject.SetActive(true);
            explosion.Play();
            yield return new WaitForSeconds(explosionTime);

            explosion.Stop();
            explosion.Clear();
            explosion.gameObject.SetActive(false);
            BulletDestroyCheck();
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            yield return new WaitForSeconds(explosionTime);
            BulletDestroyCheck();
        }
    }

    IEnumerator Disable()
    {
        bulletTurnSpeed = 0;
        EffectCheck(false);
        transform.GetComponent<Collider2D>().enabled = false;

        if (bulletImage != null)
            bulletImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);

        isDestroy = true;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        BulletDestroyCheck();
    }

    void BulletDisable()
    {
        if (host != null)
        {
            float distance1 = Vector2.Distance(transform.position, destination);
            float distance2 = Vector2.Distance(transform.position, host.position);

            if (distance1 <= 0.5f)
                bulletTurnSpeed = 0;

            if (distance2 >= hostRadius)
                StartCoroutine("Disable");
        }
        else if (ignore)
        {
            float distance = Vector2.Distance(transform.position, destination);

            if (distance <= 3)
                StartCoroutine("BulletExplosion");
        }
    }

    void BulletDestroyCheck()
    {
        if (!notPool)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }

    void OnEnable ()
	{
		EffectCheck (true);
        transform.GetComponent<Collider2D>().enabled = true;

        bulletOriginSpeed = bulletSpeed;
        bulletOriginTurnSpeed = bulletTurnSpeed;
        bulletOriginDamage = bulletDamage;
        bulletSpeed += Random.Range(0, bulletSpreadSpeed);
    }
	
	void OnDisable ()
	{
		CancelInvoke ();
		isDestroy = false;

        if (bulletImage != null)
            bulletImage.gameObject.SetActive (true);

        bulletSpeed = bulletOriginSpeed;
		bulletTurnSpeed = bulletOriginTurnSpeed;
		bulletDamage = bulletOriginDamage;

        if (skillCheck)
        {
            skillCheck = false;
            skillName = "None";
        }
	}
}