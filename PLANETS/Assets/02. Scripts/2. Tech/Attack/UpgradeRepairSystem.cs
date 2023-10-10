using System.Collections;
using UnityEngine;

public class UpgradeRepairSystem : MonoBehaviour
{
    public PlayerTurret pt;
    GameObject second;

    public GameObject FindDamagedSecondPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float value = 1.0f;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerShipManager>().isRepair && !players[i].GetComponent<PlayerShipManager>().isRetire)
            {
                float curValue = players[i].GetComponent<PlayerShipManager>().hpBarSlider.fillAmount;

                if (curValue != 0 && curValue < value)
                {
                    if (pt.damaged != players[i])
                    {
                        second = players[i];
                        value = curValue;
                    }
                }
            }
        }

        return second;
    }

    public void PlusRepair()
    {
        if (FindDamagedSecondPlayer() != null)
            StartCoroutine(Repair(FindDamagedSecondPlayer()));
        else if (pt.damaged != null)
            StartCoroutine(Repair(pt.damaged));
    }

    IEnumerator Repair(GameObject target)
    {
        float timePercentCheck;

        if (pt.timePercent <= 0.05f)
            timePercentCheck = 0.05f;
        else
            timePercentCheck = pt.timePercent;

        WaitForSeconds bulletDelay = new WaitForSeconds(pt.bulletFireTime * timePercentCheck);

        if (target != null && pt.isShooting)
        {
            pt.ObjectPoolFire(target, 1, 1);
        }

        if (!pt.atOnce)
            yield return bulletDelay;
    }
}
