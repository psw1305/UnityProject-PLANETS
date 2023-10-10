using UnityEngine;

public class EnemyFighterShipManager : MonoBehaviour 
{
    [HideInInspector] public GameObject target;
    [HideInInspector] public GameObject flagship;
    public EnemyTurret et;

    [Header("ShipManager")]
    public bool isDestroy = true;
	public Transform moveTarget;

    public float shipHp;
    public float shipSpeed;
    public float shipTurnSpeed;
    public float shipSensorRange;
    [HideInInspector] public float damage, straight, upgrade;
    [HideInInspector] public float shipOriginHp, shipOriginSpeed;
    [HideInInspector] public bool shield = false, bomber = false;

    float timer = 0;
    bool isTurn;

    [Header("Particle")]
    public GameObject shipExplosion;
    public ParticleSystem effect;
    public ParticleSystem particle;

    public void FighterDataParsing(string race, string level)
    {
        var fighterMasterTable = new MasterTableStage.MasterTableFighter();
        fighterMasterTable.Load();

        foreach (var fighterMaster in fighterMasterTable.All)
        {
            if (fighterMaster.Race == race && fighterMaster.Enemy_Level == level)
            {
                shipHp = fighterMaster.Enemy_Stat;
                et.turretFireTime = 1.0f;
            }
        }
    }

    void EngineCheck(bool check)
	{
        if (particle != null)
        {
            ParticleSystem ps = particle;
            var em = ps.emission;
            em.enabled = check;
        }
    }

    void Start()
	{
        if (effect != null)
            effect.GetComponent<Renderer>().sortingOrder = 111;

        if (particle != null)
            particle.GetComponent<Renderer>().sortingOrder = 109;

        shipOriginHp    = shipHp;
        shipOriginSpeed = shipSpeed;
        shipTurnSpeed   = Random.Range(1.0f, 4.0f);
        straight        = Random.Range(0.5f, 1.5f);

        et.bulletDivideDamage = damage / et.bulletAmmos;
    }

	void Update() 
	{
        if (flagship != null && flagship.GetComponent<EnemyTurret>().closest != null)
        {
            target     = flagship.GetComponent<EnemyTurret>().closest;
            et.closest = target;

            if (!flagship.GetComponent<EnemyTurret>().isEnable)
                ShipShutDown();
        }

        if (bomber)
        {
            if (target != null)
                BomberMoving(target);
            else if (target == null && flagship != null)
                ReturnShip();
            else
                EngineCheck(false);
        }
        else
        {
            if (target != null)
                NormalShip();
            else if (target == null && flagship != null)
                ReturnShip();
            else
                EngineCheck(false);
        }
    }

    void NormalShip()
	{
		ShipMoving(target);
		ShipTurning(target, straight);
	}

    void ReturnShip()
    {
        ReturnMoving(flagship);
        ShipTurning(flagship, straight);
    }

	void ShipTurning(GameObject obj, float delay) 
	{
		Vector2 diff = Vector2.zero;

		if (!isTurn) 
		{
			diff = obj.transform.position - transform.position;
			diff.Normalize();
			float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, rotZ), shipTurnSpeed * Time.deltaTime);
		}
		else 
		{
			timer += Time.deltaTime;

			if (timer > delay)
			{
                isTurn = false;
				timer = 0;

				shipTurnSpeed = Random.Range(1.0f, 4.0f);
				straight = Random.Range(0.5f, 1.5f);
			}
		}
	}

	void ShipMoving(GameObject obj) 
	{
		float dist = Vector2.Distance (obj.transform.position, transform.position);
		transform.position = Vector2.MoveTowards (transform.position, moveTarget.position, shipSpeed * Time.deltaTime);

		if (shipSensorRange > dist)
            isTurn = true; 

		EngineCheck(true);
	}

    void ReturnMoving(GameObject obj)
    {
        float dist = Vector2.Distance(obj.transform.position, transform.position);
        transform.position = Vector2.MoveTowards(transform.position, moveTarget.position, shipSpeed * Time.deltaTime);

        if (3 > dist)
        {
            gameObject.SetActive(false);
            shipHp    = shipOriginHp;
            shipSpeed = shipOriginSpeed;
        }

        EngineCheck(true);
    }

    void BomberMoving(GameObject obj)
    {
        float dist = Vector2.Distance(obj.transform.position, transform.position);
        transform.position = Vector2.MoveTowards(transform.position, moveTarget.position, shipSpeed * Time.deltaTime);

        if (shipSensorRange > dist)
        {
            shipSpeed = 12f;
            shipTurnSpeed = 0.3f;
        }
        else
        {
            shipSpeed = 20f;
            shipTurnSpeed = 2;
        }

        Vector2 diff = obj.transform.position - transform.position;
        diff.Normalize();
        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, rotZ), shipTurnSpeed * Time.deltaTime);

        EngineCheck(true);
    }

    void OnTriggerEnter2D (Collider2D damage) 
	{
        EnemyHitDamage ehd = damage.GetComponent<EnemyHitDamage>();

        if (ehd != null && !ehd.ignore)
        {
            if (!shield)
            {
                Damage(ehd.bulletDamage);
                ehd.Explosion();
            }
            else
                ehd.Explosion();
        }

        EnemyFighterHitDamage efhd = damage.GetComponent<EnemyFighterHitDamage>();

        if (efhd != null)
        {
            if (!shield)
            {
                ShipShutDown();
                efhd.Explosion();
            }
            else
                efhd.Explosion();
        }
    }

	public void Damage(float damageCount)
	{
		shipHp -= damageCount;

		if (shipHp <= 0) 
		    ShipShutDown();
	}

    public void ShipShutDown()
	{
        isDestroy = true;
        isTurn    = false;

        Instantiate(shipExplosion, transform.position, transform.rotation);
        gameObject.SetActive(false);
        EngineCheck(false);

        shipHp    = shipOriginHp;
        shipSpeed = shipOriginSpeed;
    }
}
