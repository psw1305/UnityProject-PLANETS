using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [HideInInspector] public string difficulty;
    [HideInInspector] public int creditMin, coreMin, dmatterMin;
    [HideInInspector] public int creditMax, coreMax, dmatterMax;
    [HideInInspector] public float addRewardMin, addRewardMax;

    [Header("UI")]
    public GameObject panel;
    public UILabel resultText;
    public UILabel creditText, coreText, dmatterText;

    int credit, core, dmatter;
    float add;

    void Start ()
    {
        credit  = PlayerPrefs.GetInt("Credit");
        core    = PlayerPrefs.GetInt("Core");
        dmatter = PlayerPrefs.GetInt("DMatter");
    }

    public void ResultReward(bool addReward)
    {
        if (addReward)
        {
            add = Random.Range(addRewardMin, addRewardMax) + 1.0f;
        }
        else
            add = 1.0f;

        credit  = PlayerPrefs.GetInt("Credit");
        core    = PlayerPrefs.GetInt("Core");
        dmatter = PlayerPrefs.GetInt("DMatter");

        float creditGain = Random.Range(creditMin, creditMax);
        creditGain *= add; 
        creditGain = Mathf.Round(creditGain * 0.1f) * 10f;
        creditText.text = creditGain.ToString();
        credit += (int)creditGain;

        float coreGain = Random.Range(coreMin, coreMax);
        coreGain *= add;
        coreText.text = Mathf.Round(coreGain).ToString();
        core += (int)coreGain;

        float dmatterGain = Random.Range(dmatterMin, dmatterMax);
        dmatterGain *= add;
        dmatterText.text = Mathf.Round(dmatterGain).ToString();
        dmatter += (int)dmatterGain;

        PlayerPrefs.SetInt("Credit", credit);
        PlayerPrefs.SetInt("Core", core);
        PlayerPrefs.SetInt("DMatter", dmatter);
    }

    public void MissionClearReward()
    {
        credit  = PlayerPrefs.GetInt("Credit");
        core    = PlayerPrefs.GetInt("Core");
        dmatter = PlayerPrefs.GetInt("DMatter");

        creditText.text  = PlayerPrefs.GetInt("MissionClear_Credit").ToString();
        coreText.text    = PlayerPrefs.GetInt("MissionClear_Core").ToString();
        dmatterText.text = PlayerPrefs.GetInt("MissionClear_DMatter").ToString();

        credit  += PlayerPrefs.GetInt("MissionClear_Credit");
        core    += PlayerPrefs.GetInt("MissionClear_Core");
        dmatter += PlayerPrefs.GetInt("MissionClear_DMatter");

        PlayerPrefs.SetInt("Credit", credit);
        PlayerPrefs.SetInt("Core", core);
        PlayerPrefs.SetInt("DMatter", dmatter);

        resultText.text = "미션 성공";
        panel.SetActive(true);
    }

    public void MissionFailedReward()
    {
        creditText.text  = "0";
        coreText.text    = "0";
        dmatterText.text = "0";

        resultText.text = "미션 실패";
        panel.SetActive(true);
    }

    public void CanclePanel()
    {
        panel.SetActive(false);
    }
}
