using UnityEngine;

public class ObjectHitBox : MonoBehaviour
{
    public float objectHp;
    public EnemyHitDamage playerBullet;
    public PlayerHitDamage enemyBullet;

    public void Damage(float damageCount)
    {
        if (objectHp < damageCount)
            objectHp -= objectHp;
        else
            objectHp -= damageCount;

        if (objectHp <= 0)
        {
            objectHp = 0;

            if (playerBullet != null)
                playerBullet.Explosion();
            else if (enemyBullet != null)
                enemyBullet.Explosion();
        }
    }

    void OnTriggerEnter2D(Collider2D damage)
    {
        if (enemyBullet != null)
        {
            EnemyHitDamage ehd = damage.GetComponent<EnemyHitDamage>();

            if (ehd != null && !ehd.ignore)
            {
                Damage(ehd.bulletDamage);

                if (!ehd.phase)
                    ehd.Explosion();
            }
        }

        if (playerBullet != null)
        {
            PlayerHitDamage phd = damage.GetComponent<PlayerHitDamage>();

            if (phd != null && !phd.ignore)
            {
                Damage(phd.bulletDamage);

                if (!phd.phase)
                    phd.Explosion();
            }
        }
    }
}
