using UnityEngine;

public class EnemyFleetFormation : MonoBehaviour
{
    [HideInInspector] public EnemyFleet ef;
    public int total, posY;
    int id;

    public void VerticalFormation(bool isPlus, int posX, string line, int scale)
    {
        switch (scale)
        {
            case 2:
                id = Random.Range(0, 14);
                break;
            case 3:
                id = Random.Range(0, 30);
                break;
            case 4:
                id = Random.Range(0, 44);
                break;
        }

        StageDataBase.Instance.BattleDataParsing(line, scale, id, ef);

        if (scale % 2 == 0)
        {
            posY = 18;

            for (int i = 0; i < scale; i++)
            {
                Vector3 shipPos1;

                if (i % 2 == 0)
                {
                    posY += 36 * i;
                    shipPos1 = new Vector3(posX, posY, 0);
                }
                else
                {
                    posY -= 36 * i;
                    shipPos1 = new Vector3(posX, posY, 0);
                }

                ShipSpawn(isPlus, shipPos1);
            }
        }
        else
        {
            posY = 0;

            for (int i = 0; i < scale; i++)
            {
                Vector3 shipPos2;

                if (i % 2 == 0)
                {
                    posY += 36 * i;
                    shipPos2 = new Vector3(posX, posY, 0);
                }
                else
                {
                    posY -= 36 * i;
                    shipPos2 = new Vector3(posX, posY, 0);
                }

                ShipSpawn(isPlus, shipPos2);
            }
        }
    }

    void ShipSpawn(bool plus, Vector3 pos)
    {
        if (ef.destroyer > 0)
        {
            ef.ShipCreate(plus, false, 0, pos);
            ef.destroyer -= 1;
        }
        else if (ef.auxiliary > 0)
        {
            ef.ShipCreate(plus, false, 1, pos);
            ef.auxiliary -= 1;
        }
        else if (ef.cruiser > 0)
        {
            ef.ShipCreate(plus, false, 2, pos);
            ef.cruiser -= 1;
        }
        else if (ef.carrier > 0)
        {
            ef.ShipCreate(plus, false, 3, pos);
            ef.carrier -= 1;
        }
        else if (ef.battleship > 0)
        {
            ef.ShipCreate(plus, false, 4, pos);
            ef.battleship -= 1;
        }
    }
}