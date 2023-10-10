using UnityEngine;

public class EnemyShipMoving : MonoBehaviour 
{
    public enum MovingType { Normal, Defense, Booster, None }
    public MovingType movingType;
	public Transform moveTarget;
    GameObject closestPlayer, closestDamaged, closestShieldDamaged;
    [HideInInspector] public GameObject targeted;
    [HideInInspector] public bool isEnable = true;
    [HideInInspector] public bool isTarget = false;
    [HideInInspector] public bool isRandom = false;
    [HideInInspector] public bool isWarp = false;

    [Header("Ship Action")]
	public float battleRadius;
    float distance, distanceRange;
    [HideInInspector] public float shipSpeed, turnSpeed;
    [HideInInspector] public float shipOriginSpeed, turnOriginSpeed;
    [HideInInspector] public float movePercent = 1.0f, turnPercent = 1.0f;

    [Header("Thruster")]
	public ParticleSystem[] particles;
	public int[] particleSortingOrders;
    public GameObject booster;

    [HideInInspector] public EnemyShipManager esm;
    [HideInInspector] public EnemyBossManager ebm;

    public void EngineCheck (bool check)
	{
		for (int i = 0; i < particles.Length; i++)
		{	
			ParticleSystem ps = particles[i];
			ps.GetComponent<Renderer>().sortingOrder = particleSortingOrders[i];
			var em = ps.emission;
			em.enabled = check;
		}
	}

    public GameObject FindClosestPlayer()
    {
        float distance = Mathf.Infinity;
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerShip");

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 diff = players[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (!players[i].GetComponentInParent<PlayerShipManager>().isRetire && curDistance < distance)
            {
                closestPlayer = players[i];
                distance = curDistance;
            }
        }

        return closestPlayer;
    }

    public GameObject FindDamagedEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        float value = 1.0f;

        for (int i = 0; i < enemys.Length; i++)
        {
            if (enemys[i].GetComponent<EnemyShipManager>().isRepair && !enemys[i].GetComponent<EnemyShipManager>().isRetire)
            {
                float curValue = enemys[i].GetComponent<EnemyShipManager>().hpBarSlider.fillAmount;

                if (curValue != 1 && curValue < value)
                {
                    closestDamaged = enemys[i];
                    value = curValue;
                }
            }
        }

        return closestDamaged;
    }

    public GameObject FindShieldDamagedEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        float value = 1.0f;

        for (int i = 0; i < enemys.Length; i++)
        {
            if (enemys[i].GetComponent<EnemyShipManager>().isShield && enemys[i].GetComponent<EnemyShipManager>().apBarSlider.fillAmount < 1.0f)
            {
                float curValue = enemys[i].GetComponent<EnemyShipManager>().apBarSlider.fillAmount;

                if (curValue != 1 && curValue < value)
                {
                    closestShieldDamaged = enemys[i];
                    value = curValue;
                }
            }
        }

        return closestShieldDamaged;
    }

    public GameObject FindRandomPlayer()
    {
        GameObject randomPlayer;
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerShip");

        if (players.Length != 0)
            randomPlayer = players[Random.Range(0, players.Length)];
        else
            randomPlayer = null;

        return randomPlayer;
    }

    public void Init()
    {
        shipOriginSpeed = shipSpeed;
        turnOriginSpeed = turnSpeed;

        distanceRange = Random.Range(-10, 10);
        battleRadius += distanceRange;
    }

    void Update() 
	{
        if (esm != null) // 일반 함선 터렛 설정
        {
            switch (esm.et.turretType)
            {
                case EnemyTurret.TurretType.Normal:
                    if (isRandom && FindRandomPlayer() != null)
                        esm.et.closest = FindRandomPlayer();
                    else if (isTarget && targeted != null)
                        esm.et.closest = targeted;
                    else
                        esm.et.closest = FindClosestPlayer();
                    break;
                case EnemyTurret.TurretType.Repair:
                    if (!esm.et.shieldRepair && FindDamagedEnemy() != null)
                        esm.et.damaged = FindDamagedEnemy();
                    else if (esm.et.shieldRepair && FindShieldDamagedEnemy() != null)
                        esm.et.damaged = FindShieldDamagedEnemy();
                    break;
            }
        }
        else if (ebm != null) // 보스 함선 터렛 설정
        {
            for (int i = 0; i < ebm.et.Length; i++)
            {
                switch (ebm.et[i].turretType)
                {
                    case EnemyTurret.TurretType.Normal:
                        if (isRandom && FindRandomPlayer() != null)
                            ebm.et[i].closest = FindRandomPlayer();
                        else if (isTarget && targeted != null)
                            ebm.et[i].closest = targeted;
                        else
                            ebm.et[i].closest = FindClosestPlayer();
                        break;
                    case EnemyTurret.TurretType.Repair:
                        if (FindDamagedEnemy() != null)
                            ebm.et[i].damaged = FindDamagedEnemy();
                        break;
                }
            }
        }

        if (isEnable && !isWarp && FindClosestPlayer() != null)
        {
            switch (movingType)
            {
                case MovingType.Normal:
                    distance = Vector2.Distance(FindClosestPlayer().transform.position, transform.position);
                    ShipMoving();
                    ShipTurning(FindClosestPlayer());
                    break;
                case MovingType.Defense:
                    ShipTurning(FindClosestPlayer());
                    break;
                case MovingType.Booster:
                    BoosterMoving();
                    break;
            }
        }
        else
            StopMoving();
    }

    void ShipTurning(GameObject target) 
	{
		Vector2 diff = target.transform.position - transform.position;	
		diff.Normalize();
		float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, rotZ), turnSpeed * turnPercent * Time.deltaTime);
	}

    void ShipMoving()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveTarget.position, shipSpeed * Time.deltaTime);

        if (distance < battleRadius && battleRadius != 0)
        {
            shipSpeed = Mathf.MoveTowards(shipSpeed, 0, shipSpeed * Time.deltaTime);
            EngineCheck(false);
        }
        else
        {
            shipSpeed = Mathf.MoveTowards(shipSpeed, shipOriginSpeed * movePercent, Time.deltaTime);
            EngineCheck(true);
        }
    }

    void BoosterMoving()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveTarget.position, shipSpeed * Time.deltaTime);
        shipSpeed = Mathf.MoveTowards(shipSpeed, shipOriginSpeed * movePercent * 3, Time.deltaTime * 6);
        EngineCheck(true);
    }

    void StopMoving()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveTarget.position, shipSpeed * Time.deltaTime);
        shipSpeed = Mathf.MoveTowards(shipSpeed, 0, shipSpeed * Time.deltaTime);
        EngineCheck(false);
    }
}