using UnityEngine;

public class CaptainUIManager : MonoBehaviour
{
    [Header("Script")]
    public CaptainManager[] cm;
    public ResourceData rd;
    public MainBackButton mbb;
    [HideInInspector] public CaptainManager select;

    [Header("Popup")]
    public GameObject captBuy;
    public GameObject captMax;
    public UISprite captImage;
    public UILabel captName;
    public UILabel captMaxLabel;
    [HideInInspector] public bool isPopup = false;

    void Start()
    {
        PlayerDataBase.Instance.CaptainDataParsing("Captain_Battle", 1, cm[0]);
        PlayerDataBase.Instance.CaptainDataParsing("Captain_Tech", 1, cm[1]);
        PlayerDataBase.Instance.CaptainDataParsing("Captain_Supply", 1, cm[2]);
    }

    void Update()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPopup && mbb.currentNum == 1)
                PopupCancle();
        }
        #endif
    }

    public void Appointment()
    {
        PopupCancle();

        if (select != null)
            select.Unlock();
    }

    public bool CostPay()
    {
        rd.mbb = mbb;
        rd.mbbNum = 1;
        rd.resourceCheck = true;

        int checkCredit = rd.PayResource("Credit", 1200);
        int checkCore = rd.PayResource("Core", 12);

        if (rd.resourceCheck)
        {
            rd.ResourceLabel("Credit", checkCredit);
            rd.ResourceLabel("Core", checkCore);
            return true;
        }
        else
        {
            PopupCancle();
            return false;
        }
    }

    public void PopupCancle()
    {
        if (isPopup && captBuy.activeSelf)
        {
            isPopup = false;
            mbb.currentNum -= 1;
            captBuy.SetActive(false);
        }
        else if (isPopup && captMax.activeSelf)
        {
            isPopup = false;
            mbb.currentNum -= 1;
            captMax.SetActive(false);
        }
    }
}
