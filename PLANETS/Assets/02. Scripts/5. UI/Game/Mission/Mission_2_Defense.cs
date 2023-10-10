using UnityEngine;

public class Mission_2_Defense : MonoBehaviour
{
    public StageMainPlanet smp;

    [Header("UI")]
    public GameObject startButton;
    public UISprite pBarSlider;
    public UILabel pLabel;
    public PlayerMiningEffect[] pme;

    int check = 0;
    float capturePoint;
    bool isCapture = false;

    void Update()
    {
        if (isCapture)
        {
            capturePoint += Time.deltaTime;
            pBarSlider.fillAmount = capturePoint / 180;

            float percent = pBarSlider.fillAmount * 100;
            pLabel.text = percent.ToString("N0");

            if (capturePoint >= 0 && check == 0)
            {
                check += 1;
                smp.SpawnSetting("EnemyFleet", 3);
            }
            else if (capturePoint >= 30 && check == 1)
            {
                check += 1;
                MiningBoxCheck(0);
                smp.SpawnSetting("EnemyFleet", 5);
            }
            else if (capturePoint >= 60 && check == 2)
            {
                check += 1;
                MiningBoxCheck(1);
                smp.SpawnSetting("EnemyFleet", 6);
            }
            else if (capturePoint >= 90 && check == 3)
            {
                check += 1;
                MiningBoxCheck(2);
                smp.SpawnSetting("EnemyFleet", 6);
            }
            else if (capturePoint >= 120 && check == 4)
            {
                check += 1;
                MiningBoxCheck(3);
                smp.SpawnSetting("EnemyFleet", 9);
            }

            if (capturePoint >= 180)
            {
                isCapture = false;
                pBarSlider.fillAmount = 1.0f;
                pLabel.text = "100";
                smp.MissionClear();
            }
        }
    }

    void MiningBoxCheck(int number)
    {
        for (int i = 0; i < pme.Length; i++)
        {
            if (pme[i] != null)
                pme[i].MiningBox[number].SetActive(true);
        }
    }

    public void OnCaptureButton()
    {
        isCapture = true;

        for (int i = 0; i < pme.Length; i++)
            pme[i].isMining = true;

        startButton.SetActive(false);
    }
}
