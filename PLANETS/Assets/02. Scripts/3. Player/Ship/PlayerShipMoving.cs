using UnityEngine;

public class PlayerShipMoving : MonoBehaviour 
{
    public enum MovingType { Normal, Defense, Booster, None }
    public MovingType movingType;
	public Transform moveTarget;
    GameObject closestEnemy, closestDamaged, closestShieldDamaged;
    [HideInInspector] public GameObject targeted;
    [HideInInspector] public bool isEnable = true;
    [HideInInspector] public bool isTarget = false;
    [HideInInspector] public bool isRandom = false;

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

    [HideInInspector] public PlayerShipManager psm;

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

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("EnemyShip");
        float distance = Mathf.Infinity;

        for (int i = 0; i < enemys.Length; i++)
        {
            Vector3 diff = enemys[i].transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;

            if (/*!enemys[i].GetComponentInParent<EnemyShipManager>().isRetire &&*/ curDistance < distance)
            {
                closestEnemy = enemys[i];
                distance = curDistance;
            }
        }

        return closestEnemy;
    }

    public GameObject FindDamagedPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float value = 1.0f;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerShipManager>().isRepair && !players[i].GetComponent<PlayerShipManager>().isRetire)
            {
                float curValue = players[i].GetComponent<PlayerShipManager>().hpBarSlider.fillAmount;

                if (curValue != 1 && curValue < value)
                {
                    closestDamaged = players[i];
                    value = curValue;
                }
            }
        }

        return closestDamaged;
    }

    public GameObject FindShieldDamagedPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float value = 1.0f;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerShipManager>().isShield && players[i].GetComponent<PlayerShipManager>().apBarSlider.fillAmount < 1.0f)
            {
                float curValue = players[i].GetComponent<PlayerShipManager>().apBarSlider.fillAmount;

                if (curValue != 1 && curValue < value)
                {
                    closestShieldDamaged = players[i];
                    value = curValue;
                }
            }
        }

        return closestShieldDamaged;
    }

    public GameObject FindRandomEnemy()
    {
        GameObject randomEnemy;
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("EnemyShip");

        if (enemys.Length != 0)
            randomEnemy = enemys[Random.Range(0, enemys.Length)];
        else
            randomEnemy = null;
        
        return randomEnemy;
    }

    public void Init() 
	{
        isEnable = true;

        shipOriginSpeed = shipSpeed;
        turnOriginSpeed = turnSpeed;

        distanceRange = Random.Range(-10, 10);
        battleRadius += distanceRange;
    }
    
    void Update() 
	{
        if (psm != null)
        {
            switch (psm.pt.turretType)
            {
                case PlayerTurret.TurretType.Normal:
                    if (FindClosestEnemy() != null)
                    {
                        if (isRandom && FindRandomEnemy() != null)
                            psm.pt.closest = FindRandomEnemy();
                        else if (isTarget && targeted != null)
                            psm.pt.closest = targeted;
                        else
                            psm.pt.closest = FindClosestEnemy();
                    }
                    break;
                case PlayerTurret.TurretType.Repair:
                    if (!psm.pt.shieldRepair && FindDamagedPlayer() != null)
                        psm.pt.damaged = FindDamagedPlayer();
                    else if (psm.pt.shieldRepair && FindShieldDamagedPlayer() != null)
                        psm.pt.damaged = FindShieldDamagedPlayer();
                    break;
            }
        }

        if (isEnable && FindClosestEnemy() != null)
        {
            switch (movingType)
            {
                case MovingType.Normal:
                    NormalType();
                    break;
                case MovingType.Defense:
                    DefenseType();
                    break;
                case MovingType.Booster:
                    BoosterMoving();
                    break;
            }
        }
        else
            StopMoving();
    }

    void NormalType()
    {
        if (isRandom && FindRandomEnemy() != null)
        {
            distance = Vector2.Distance(FindRandomEnemy().transform.position, transform.position);
            ShipMoving();
            ShipTurning(FindRandomEnemy());
        }
        else if (isTarget && targeted != null)
        {
            distance = Vector2.Distance(targeted.transform.position, transform.position);
            ShipMoving();
            ShipTurning(targeted);
        }
        else
        {
            distance = Vector2.Distance(FindClosestEnemy().transform.position, transform.position);
            ShipMoving();
            ShipTurning(FindClosestEnemy());
        }
    }

    void DefenseType()
    {
        if (isTarget && targeted != null)
            ShipTurning(targeted);
        else
            ShipTurning(FindClosestEnemy());
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