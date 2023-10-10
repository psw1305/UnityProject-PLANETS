using System.Collections;
using UnityEngine;

public class SubEnemyTurret : MonoBehaviour
{
    public enum AttackType { Missile, Laser }
    public AttackType attackType;

    public EnemyTurret et;
    public Transform turretPos;
    [HideInInspector] public Vector3 destination;

    [Header("System")]
    public bool turretFixed = false;
    public bool multiLaser = false;

    [Header("Bullet")]
    public Transform bullet;
    public float bulletAmmos;
    public float bulletFireTime;
    public float accuracyPoint;
    public GameObject effect;

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
        Vector3 targetPos  = new Vector3(destination.x + ranX, destination.y + ranY, 0);

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

        obj.GetComponent<PlayerHitDamage>().host        = transform;
        obj.GetComponent<PlayerHitDamage>().hostRadius  = et.turretSensor;
        obj.GetComponent<PlayerHitDamage>().destination = targetPosition;

        if (et.damagePercent <= 0.05f)
            obj.GetComponent<PlayerHitDamage>().bulletDamage = et.bulletDivideDamage * 0.05f;
        else
            obj.GetComponent<PlayerHitDamage>().bulletDamage = et.bulletDivideDamage * et.damagePercent;

        obj.position = turret.position;
        obj.rotation = turret.rotation;
        obj.gameObject.SetActive(true);
    }

    void Bullet_Laser(Vector3 targetPosition, Transform turret)
    {
        Transform obj = ObjectPool.instance.Spawn(bullet);
        if (obj == null) { return; }

        obj.GetComponent<PlayerHitDamage>().host       = transform;
        obj.GetComponent<PlayerHitDamage>().hostRadius = 500;

        if (et.damagePercent <= 0.05f)
            obj.GetComponent<PlayerHitDamage>().bulletDamage = et.bulletDivideDamage * 0.05f;
        else
            obj.GetComponent<PlayerHitDamage>().bulletDamage = et.bulletDivideDamage * et.damagePercent;

        //GameObject lineEffect = Instantiate(effect, turret.position, effect.transform.rotation) as GameObject;
        //lineEffect.GetComponent<LineRendererManager>().start = turret;
        //lineEffect.GetComponent<LineRendererManager>().end   = targetPosition;

        obj.position = targetPosition;
        obj.rotation = turret.rotation;
        obj.gameObject.SetActive(true);
    }
}
