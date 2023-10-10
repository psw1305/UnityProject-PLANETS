using System.Collections;
using UnityEngine;

public class SubPlayerTurret : MonoBehaviour
{
    public enum AttackType { Missile, Laser }
    public AttackType attackType;

    public PlayerTurret pt;
    public Transform turretPos;
    [HideInInspector] public Vector3 destination;

    [Header("System")]
    public bool turretFixed = false;
    public bool multiLaser = false;

    [Header("Bullet")]
    public Transform bullet;
    public int bulletLength;
    public float bulletAmmos;
    public float bulletFireTime;
    public float accuracyPoint;
    public GameObject effect;

    void Awake()
    {
        if (bullet != null && bulletLength != 0)
        {
            ObjectPool op = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<ObjectPool>();
            op.AddItem(bullet, bulletLength);
        }
    }

    public void SubTurretActive()
    {
        StartCoroutine("TurretFire");
    }

    IEnumerator TurretFire()
    {
        for (int i = 0; i < bulletAmmos; i++)
        {
            ObjectPoolFire(i);
            yield return new WaitForSeconds(bulletFireTime);
        }
    }

    void ObjectPoolFire(int number)
    {
        float ranX = Random.Range(-accuracyPoint, accuracyPoint);
        float ranY = Random.Range(-accuracyPoint, accuracyPoint);
        Vector3 targetPos = new Vector3(destination.x + ranX, destination.y + ranY, 0);

        if (!turretFixed)
        {
            Vector3 targetDir = targetPos - turretPos.position;
            targetDir.Normalize();
            float rotZ = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            turretPos.rotation = Quaternion.Euler(0f, 0f, rotZ);
        }

        switch (attackType)
        {
            case AttackType.Missile:
                Bullet_Missile(targetPos, turretPos);
                break;
            case AttackType.Laser:
                Bullet_Laser(targetPos, turretPos);
                break;
        }
    }

    void Bullet_Missile(Vector3 targetPosition, Transform turret)
    {
        Transform obj = ObjectPool.instance.Spawn(bullet);
        if (obj == null) { return; }

        obj.GetComponent<EnemyHitDamage>().host = transform;
        obj.GetComponent<EnemyHitDamage>().hostRadius = pt.turretSensor;
        obj.GetComponent<EnemyHitDamage>().destination = targetPosition;

        if (pt.damagePercent <= 0.05f)
            obj.GetComponent<EnemyHitDamage>().bulletDamage = pt.bulletDivideDamage * 0.05f;
        else
            obj.GetComponent<EnemyHitDamage>().bulletDamage = pt.bulletDivideDamage * pt.damagePercent;

        obj.position = turret.position;
        obj.rotation = turret.rotation;
        obj.gameObject.SetActive(true);
    }

    void Bullet_Laser(Vector3 targetPosition, Transform turret)
    {
        Transform obj = ObjectPool.instance.Spawn(bullet);
        if (obj == null) { return; }

        obj.GetComponent<EnemyHitDamage>().host = transform;
        obj.GetComponent<EnemyHitDamage>().hostRadius = 500;

        if (pt.damagePercent <= 0.05f)
            obj.GetComponent<EnemyHitDamage>().bulletDamage = pt.bulletDivideDamage * 0.05f;
        else
            obj.GetComponent<EnemyHitDamage>().bulletDamage = pt.bulletDivideDamage * pt.damagePercent;

        //GameObject lineEffect = Instantiate(effect, turret.position, effect.transform.rotation) as GameObject;
        //lineEffect.GetComponent<LineRendererManager>().start = turret;
        //lineEffect.GetComponent<LineRendererManager>().end = targetPosition;

        obj.position = targetPosition;
        obj.rotation = turret.rotation;
        obj.gameObject.SetActive(true);
    }
}
