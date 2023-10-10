using System.Collections;
using UnityEngine;

public class PlayerTurret : MonoBehaviour 
{
    public enum TurretType { Normal, Repair, None }
    public TurretType turretType;
    public enum AttackType { Missile, Laser, Fighter, Explose, None }
    public AttackType attackType;

    public UpgradeRepairSystem urs;
    public FighterSquad[] fs;
    public Transform[] fighters;
    [HideInInspector] public GameObject closest, damaged, random, ship;

    [Header("System")]
    public bool atOnce = false;
    public bool turretFixed = false;
    public bool shieldRepair = false;
    [HideInInspector] public bool isEnable = true;
    [HideInInspector] public bool isShooting = false;

    [Header("Bullet Manager")]
    public Transform bullet;
    public int bulletLength;
    public float bulletAmmos, bulletFireTime;
    public bool bulletSkillCheck = false;
    public string bulletSkillName = "None";
    [HideInInspector] public float bulletDamage, bulletDivideDamage;
    [HideInInspector] public float bulletDur, bulletAtk, bulletNum;
    [HideInInspector] public float timePercent = 1.0f, damagePercent = 1.0f;

    [Header("Turret Manager")]
    public Transform[] turrets;
    public GameObject[] turretMuzzles;
    public float accuracyPoint;
    public int turretsMod;
    [HideInInspector] public float turretFireTime, turretSensor;
    [HideInInspector] public PlayerShipManager psm;

    void Awake()
    {
        if (bullet != null && bulletLength != 0)
        {
            ObjectPool op = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<ObjectPool>();
            op.AddItem(bullet, bulletLength);
        }
    }

    void Update()
    {
        switch (turretType)
        {
            case TurretType.Normal:
                Turret(closest);
                break;
            case TurretType.Repair:
                Turret(damaged);
                break;
            case TurretType.None:
                gameObject.SetActive(false);
                break;
        }
	}

    void Turret(GameObject target)
    {
        if (target != null && isEnable)
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance < turretSensor && !isShooting)
            {
                StartCoroutine(TurretFire(target));
                if (urs != null) urs.PlusRepair();
            }
        }
    }

    IEnumerator TurretFire(GameObject target)
    {
        isShooting = true;

        float timePercentCheck;

        if (timePercent <= 0.05f)
            timePercentCheck = 0.05f;
        else
            timePercentCheck = timePercent;

        WaitForSeconds bulletDelay = new WaitForSeconds(bulletFireTime * timePercentCheck);

        for (int i = 0; i < bulletAmmos; i++)
        {
            if (target != null && isShooting)
            {
                int pos = i % turretsMod;
                ObjectPoolFire(target, pos, i);
            }

            if (!atOnce)
                yield return bulletDelay;
        }

        yield return new WaitForSeconds(turretFireTime * timePercentCheck);

        if (isShooting)
            isShooting = false;
    }

    IEnumerator BulletMuzzleflash(int num)
    {
        if (turretMuzzles != null)
        {
            turretMuzzles[num].SetActive(true);
            yield return new WaitForSeconds(0.1f);
            turretMuzzles[num].SetActive(false);
        }
    }

    public void ObjectPoolFire(GameObject target, int pos, int number)
    {
        float ranX = Random.Range(-accuracyPoint, accuracyPoint);
        float ranY = Random.Range(-accuracyPoint, accuracyPoint);
        Vector3 targetPos = new Vector3(target.transform.position.x + ranX, target.transform.position.y + ranY, 0);

        if (!turretFixed)
        {
            Vector3 targetDir = targetPos - turrets[pos].position;
            targetDir.Normalize();
            float rotZ = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            turrets[pos].rotation = Quaternion.Euler(0f, 0f, rotZ);
        }

        //StartCoroutine(BulletMuzzleflash(pos));

        switch (attackType)
        {
            case AttackType.Missile:
                Bullet_Missile(targetPos, turrets[pos]);
                break;
            case AttackType.Laser:
                Bullet_Laser(targetPos, turrets[pos]);
                break;
            case AttackType.Fighter:
                Bullet_Fighter(turrets[pos], number);
                break;
            case AttackType.Explose:
                Bullet_Explose(targetPos, turrets[pos]);
                break;
        }
    }

    void Bullet_Missile(Vector3 targetPosition, Transform turret)
	{
		Transform obj = ObjectPool.instance.Spawn(bullet);
        if (obj == null) { return; }

        obj.GetComponent<EnemyHitDamage>().host        = transform;
        obj.GetComponent<EnemyHitDamage>().hostRadius  = turretSensor;
        obj.GetComponent<EnemyHitDamage>().destination = targetPosition;

        if (damagePercent <= 0.05f)
            obj.GetComponent<EnemyHitDamage>().bulletDamage = bulletDivideDamage * 0.05f;
        else
            obj.GetComponent<EnemyHitDamage>().bulletDamage = bulletDivideDamage * damagePercent;

        if (bulletSkillCheck)
        {
            obj.GetComponent<EnemyHitDamage>().skillCheck = true;
            obj.GetComponent<EnemyHitDamage>().skillName  = bulletSkillName;
            obj.GetComponent<EnemyHitDamage>().skillDur = bulletDur;
            obj.GetComponent<EnemyHitDamage>().skillAtk = bulletAtk;
            obj.GetComponent<EnemyHitDamage>().skillNum = bulletNum;
        }
        
        obj.position = turret.position;
		obj.rotation = turret.rotation;	
		obj.gameObject.SetActive(true);
	}

	void Bullet_Laser(Vector3 targetPosition, Transform turret)
	{
        Transform obj = ObjectPool.instance.Spawn(bullet);
        if (obj == null) { return; }

        if (damagePercent <= 0.05f)
            obj.GetComponent<LaserHitDamage>().laserDamage = bulletDivideDamage * 0.05f;
        else
            obj.GetComponent<LaserHitDamage>().laserDamage = bulletDivideDamage * damagePercent;

        obj.GetComponent<LaserHitDamage>().start = turret;
        obj.GetComponent<LaserHitDamage>().end   = targetPosition;

        if (obj.GetComponent<LaserHitDamage>().start != null)
        {
            obj.GetComponent<LaserHitDamage>().laser.SetPosition(0, turret.position);
            obj.GetComponent<LaserHitDamage>().laser.SetPosition(1, targetPosition);
        }

        obj.position = targetPosition;
        obj.rotation = turret.rotation;
        obj.gameObject.SetActive(true);
    }

    void Bullet_Fighter(Transform turret, int number)
    {
        PlayerFighterShipManager pfsm = fighters[number].GetComponent<PlayerFighterShipManager>();

        if (pfsm.isDestroy)
        {
            pfsm.isDestroy = false;
            pfsm.flagship = gameObject;

            if (damagePercent <= 0.05f)
                pfsm.damage = bulletDivideDamage * 0.05f;
            else
                pfsm.damage = bulletDivideDamage * damagePercent;

            fighters[number].position = turret.position;
            fighters[number].rotation = turret.rotation;
            fighters[number].gameObject.SetActive(true);
        }
    }

    void Bullet_Explose(Vector3 targetPosition, Transform turret)
    {
        Transform obj = ObjectPool.instance.Spawn(bullet);
        if (obj == null) { return; }

        obj.GetComponent<EnemyHitDamage>().destination = targetPosition;

        if (damagePercent <= 0.05f)
            obj.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().damage = bulletDivideDamage * 0.05f;
        else
            obj.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().damage = bulletDivideDamage * damagePercent;

        obj.position = turret.position;
        obj.rotation = turret.rotation;
        obj.gameObject.SetActive(true);
    }

    void OnEnable ()  { isShooting = false; }
	void OnDisable () { isShooting = false; }
}
