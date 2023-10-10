using UnityEngine;

public class PlayerFighterShipManager : MonoBehaviour 
{
    [HideInInspector] public GameObject target;
    [HideInInspector] public GameObject flagship;
    public PlayerTurret pt;

    [Header("ShipManager")]
    public bool isDestroy = true;
    public Transform moveTarget;

    public float shipHp;
    public float shipSpeed;
    public float shipTurnSpeed;
    public float shipSensorRange;
    [HideInInspector] public float damage, straight, upgrade;
    [HideInInspector] public float shipOriginHp, shipOriginSpeed;
    [HideInInspector] public bool warp = false, shield = false, bomber = false;

    float timer = 0;
    bool isTurn, isWarp, isReturn;

    [Header("Effect")]
    public GameObject shipExplosion;
    public ParticleSystem effect;
    public ParticleSystem particle;

    public void FighterDataParsing(string race, int level)
    {
        var fighterMasterTable = new MasterTableStage.MasterTableFighter();
        fighterMasterTable.Load();

        foreach (var fighterMaster in fighterMasterTable.All)
        {
            if (fighterMaster.Race == race && fighterMaster.Player_Level == level)
            {
                shipHp = fighterMaster.Player_Stat;
                pt.turretFireTime = 1.0f;
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
            effect.GetComponent<Renderer>().sortingOrder = 121;

        if (particle != null)
            particle.GetComponent<Renderer>().sortingOrder = 119;

        shipOriginHp    = shipHp;
        shipOriginSpeed = shipSpeed;
        shipTurnSpeed   = Random.Range(1.0f, 4.0f);
        straight        = Random.Range(0.5f, 1.5f);

        pt.bulletDivideDamage = damage / pt.bulletAmmos;

        isWarp = true; isReturn = true;
    }

    void Update() 
	{
        if (flagship != null && flagship.GetComponent<PlayerTurret>().closest != null)
        {
            target = flagship.GetComponent<PlayerTurret>().closest;
            pt.closest = target;

            if (!flagship.GetComponent<PlayerTurret>().isEnable)
                ShipShutDown();
        }

        if (warp)
        {
            if (target != null)
                WarpMoving(target, straight);
            else if (target == null && flagship != null)
                WarpReturn(flagship);
            else
                EngineCheck(false);
        }
        else if (bomber)
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
                straight      = Random.Range(0.5f, 1.5f);
            }
        }
    }

    void ShipMoving(GameObject obj) 
	{
		float dist = Vector2.Distance(obj.transform.position, transform.position);
		transform.position = Vector2.MoveTowards(transform.position, moveTarget.position, shipSpeed * Time.deltaTime);
		
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

    void WarpMoving(GameObject obj, float delay)
    {
        if (isWarp)
        {
            isWarp = false;
            effect.Play();

            float ranX = Mathf.Cos(Random.Range(0, 360)); float ranY = Mathf.Sin(Random.Range(0, 360)); ;
            float rad = Random.Range(13, 17);
            transform.position = new Vector3(obj.transform.position.x + ranX * rad, obj.transform.position.y + ranY * rad, 0);

            Vector2 diff = obj.transform.position - transform.position;
            diff.Normalize();
            float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
        else if (!isWarp)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveTarget.position, shipSpeed * Time.deltaTime);
            timer += Time.deltaTime;

            if (timer > delay)
            {
                isWarp = true;
                effect.Play();

                timer = 0;
                straight = Random.Range(0.8f, 1f);
            }
        }

        EngineCheck(true);
    }

    void WarpReturn(GameObject obj)
    {
        if (isReturn)
        {
            isReturn = false;
            effect.Play();

            float ranX = Mathf.Cos(Random.Range(0, 360)); float ranY = Mathf.Sin(Random.Range(0, 360)); ;
            float rad = Random.Range(18, 21);
            transform.position = new Vector3(obj.transform.position.x + ranX * rad, obj.transform.position.y + ranY * rad, 0);

            Vector2 diff = obj.transform.position - transform.position;
            diff.Normalize();
            float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
        else if (!isReturn)
        {
            float dist = Vector2.Distance(obj.transform.position, transform.position);
            transform.position = Vector2.MoveTowards(transform.position, moveTarget.position, shipSpeed * Time.deltaTime);

            if (4 > dist)
            {
                gameObject.SetActive(false);
                shipHp    = shipOriginHp;
                shipSpeed = shipOriginSpeed;
                isReturn = true;
            }

            EngineCheck(true);
        }
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

    void OnTriggerEnter2D(Collider2D damage) 
	{
		PlayerHitDamage phd = damage.GetComponent<PlayerHitDamage>();
		
		if (phd != null && !phd.ignore) 
		{
            if (!shield)
            {
                Damage(phd.bulletDamage);
                phd.Explosion();
            }
            else
                phd.Explosion();
		}

        PlayerFighterHitDamage pfhd = damage.GetComponent<PlayerFighterHitDamage>();

        if (pfhd != null)
        {
            if (!shield)
            {
                ShipShutDown();
                pfhd.Explosion();
            }
            else
                pfhd.Explosion();
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
        isTurn = false;

        Instantiate(shipExplosion, transform.position, transform.rotation);
        gameObject.SetActive(false);
        EngineCheck(false);

        shipHp    = shipOriginHp;
        shipSpeed = shipOriginSpeed;
    }
}
