using UnityEngine;

public class Mission_1_Message : MonoBehaviour
{
    public StageMainPlanet smp;
    public GameObject[] buttons;
    public UILabel[] countDowns;
    public int[] number;

    bool isCheck = false;
    float coolTime = 30;

    void Start()
    {
        int ranNum = Random.Range(1, 4);

        switch (ranNum)
        {
            case 1:
                number[0] = 3;
                number[1] = 3;
                number[2] = 3;
                break;
            case 2:
                number[0] = 3;
                number[1] = 3;
                number[2] = 3;
                break;
            case 3:
                number[0] = 3;
                number[1] = 3;
                number[2] = 3;
                break;
        }

        for (int i = 0; i < 3; i++)
        {
            if (number[i] == 2)
                smp.SpawnSetting("EnemyFleet", 6);
            else if (number[i] == 3)
                smp.SpawnSetting("EnemyFleet", 9);
        }
    }

    void Update()
    {
        if (coolTime > 0 && isCheck)
        {
            coolTime -= Time.deltaTime;
            countDowns[0].text = coolTime.ToString("N1");
            countDowns[1].text = coolTime.ToString("N1");
            countDowns[2].text = coolTime.ToString("N1");

            if (coolTime <= 0)
            {
                isCheck = false;
                coolTime = 30;

                countDowns[0].text = "A편대 교신";
                countDowns[1].text = "B편대 교신";
                countDowns[2].text = "C편대 교신";

                buttons[0].GetComponent<BoxCollider>().enabled = true;
                buttons[1].GetComponent<BoxCollider>().enabled = true;
                buttons[2].GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    public void MessageACheck()
    {
        if (!isCheck)
        {
            smp.enemyFleets[0].transform.position = new Vector3(240, 0, 0);
            Delay(0);
        }
    }

    public void MessageBCheck()
    {
        if (!isCheck)
        {
            smp.enemyFleets[1].transform.position = new Vector3(0, 200, 0);
            smp.enemyFleets[1].transform.rotation = Quaternion.Euler(0, 0, 90);
            Delay(1);
        }
    }

    public void MessageCCheck()
    {
        if (!isCheck)
        {
            smp.enemyFleets[2].transform.position = new Vector3(150, -160, 0);
            smp.enemyFleets[2].transform.rotation = Quaternion.Euler(0, 0, -45);
            Delay(2);
        }
    }

    void Delay(int num)
    {
        isCheck = true;
        smp.pf.ShipOnline();
        smp.enemyFleets[num].GetComponent<EnemyFleet>().StartShip();

        buttons[num].SetActive(false);
        buttons[0].GetComponent<BoxCollider>().enabled = false;
        buttons[1].GetComponent<BoxCollider>().enabled = false;
        buttons[2].GetComponent<BoxCollider>().enabled = false;
    }
}