using UnityEngine;

public class Mission_3_Secure : MonoBehaviour
{
    public StageMainPlanet smp;

    [Header("UI")]
    public GameObject startButton;
    public UILabel tLabel;

    int check = 0;
    float updatePoint = 180;
    bool isUpdate = false;

    void Update()
    {
        if (isUpdate)
        {
            updatePoint -= Time.deltaTime;
            tLabel.text = updatePoint.ToString("N1");

            if (updatePoint <= 180 && check == 0)
            {
                check += 1;
                smp.SpawnSetting("EnemyFleet", 3);
            }
            else if (updatePoint <= 150 && check == 1)
            {
                check += 1;
                smp.SpawnSetting("EnemyFleet", 5);
            }
            else if (updatePoint <= 120 && check == 2)
            {
                check += 1;
                smp.SpawnSetting("EnemyFleet", 6);
            }
            else if (updatePoint <= 90 && check == 3)
            {
                check += 1;
                smp.SpawnSetting("EnemyFleet", 6);
            }
            else if (updatePoint <= 60 && check == 4)
            {
                check += 1;
                smp.SpawnSetting("EnemyFleet", 9);
            }

            if (updatePoint <= 0)
            {
                isUpdate = false;
                tLabel.text = "Clear";
                smp.MissionClear();
            }
        }
    }

    public void OnUpdateButton()
    {
        isUpdate = true;
        startButton.SetActive(false);
    }
}
