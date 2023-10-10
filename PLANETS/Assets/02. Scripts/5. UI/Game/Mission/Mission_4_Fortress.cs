using UnityEngine;

public class Mission_4_Fortress : MonoBehaviour
{
    [HideInInspector] public StageMainPlanet smp;
    public EnemyShipManager esm;
    int check = 0;

    void Start()
    {
        esm.shipOriginHp *= 7;
        esm.shipOriginAp *= 2.5f;

        esm.shipHp = esm.shipOriginHp;
        esm.shipAp = esm.shipOriginAp;

        esm.HealthValue();
        esm.ShieldValue();
    }

    void Update()
    {
        float value = esm.shipHp / esm.shipOriginHp;

        if (value <= 0.9f && check == 0)
        {
            check += 1;
            smp.SpawnSetting("EnemyFleet", 3);
        }
        else if (value <= 0.6f && check == 1)
        {
            check += 1;
            smp.SpawnSetting("EnemyFleet", 6);
        }
        else if (value <= 0.2f && check == 2)
        {
            check += 1;
            smp.SpawnSetting("EnemyFleet", 9);
        }
    }
}
