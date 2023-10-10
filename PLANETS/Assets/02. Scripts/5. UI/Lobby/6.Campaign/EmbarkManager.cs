using UnityEngine;

public class EmbarkManager : MonoBehaviour
{
    [HideInInspector] public int maintain, cmpCap, shipCap;
    public FleetShipBox[] fsb;

    [Header("UI")]
    public GameObject blind;
    public UILabel maintainLabel;

    [Header("UI: Captain")]
    public UIToggle captainToggle;
    public UISprite captainImage;
    public UILabel captainName;
    public TweenAlpha ta;

    [Header("Popup")]
    public GameObject shipNon;
    public GameObject shipMax;
    public GameObject capMax;
    public UILabel capMaxLabel;
    [HideInInspector] public bool isPopup = false;

    [Header("Script")]
    public MainBackButton mbb;

    void Start()
    {
        PlayerPrefs.SetString("InGame_Captain", "장교 선택창");
    }

    public void BoxToggleCancle()
    {
        for (int i = 0; i < fsb.Length; i++)
            fsb[i].toggle.value = false;
    }

    public void FleetCheck()
    {
        if (PlayerPrefs.HasKey("Building_Headquarters"))
        {
            maintain = 200;
            maintainLabel.text = cmpCap + " / " + maintain; ;
            capMaxLabel.text = "최대 수용량을 넘어" + '\n' + "함선을 더 배치할 수 없습니다.";
        }
        else
        {
            maintain = 100;
            maintainLabel.text = cmpCap + " / " + maintain; ;
            capMaxLabel.text = "최대 수용량을 넘어" + '\n' + "함선을 더 배치할 수 없습니다." + '\n' + "(작전사령부 건설 시 출정 포인트 +100)";
        }
    }

    public void FleetDataLoad()
    {
        for (int i = 0; i < fsb.Length; i++)
            fsb[i].DataLoad();
    }

    public void FleetCancle()
    {
        for (int i = 0; i < fsb.Length; i++)
        {
            if (fsb[i].toggle.value && !fsb[i].isEmpty)
            {
                if (fsb[i].ed != null)
                {
                    fsb[i].ed.button.isEnabled = true;
                    fsb[i].ed.select.SetActive(false);
                }

                fsb[i].Empty();
                fsb[i].toggle.value = false;
            }
        }
    }

    public void CapacitySetting()
    {
        cmpCap  = 0;
        shipCap = 0;

        for (int i = 0; i < fsb.Length; i++)
        {
            if (fsb[i].shipName.text != "")
            {
                int plus = PlayerPrefs.GetInt("ShipMaintain_" + fsb[i].shipName.text);
                cmpCap  += plus;
                shipCap += 1;
            }
        }

        maintainLabel.text = cmpCap + " / " + maintain;
    }

    bool CapacityCheck()
    {
        if (cmpCap == 0)
        {
            mbb.currentNum = 3;

            isPopup = true;
            shipNon.SetActive(true);
            return false;
        }
        else if (cmpCap > maintain)
        {
            mbb.currentNum = 3;

            isPopup = true;
            capMax.SetActive(true);    
            return false;
        }
        else
            return true;
    }

    public bool ShipCapacityCheck()
    {
        if (shipCap >= 8)
        {
            mbb.currentNum = 3;

            isPopup = true;
            shipMax.SetActive(true);
            return false;
        }
        else
            return true;
    }

    public void CaptainList()
    {
        if (captainToggle.value)
            ta.PlayForward();
        else
            ta.PlayReverse();
    }

    public void CaptainSelected(string cType, string cName)
    {
        captainImage.spriteName = cType;
        captainName.text = cName;
        
        captainToggle.value = false;
        ta.PlayReverse();
    }

    public void PopupCancle()
    {
        if (shipNon.activeSelf)
        {
            mbb.currentNum -= 1;
            isPopup = false;
            shipNon.SetActive(false);
        }
        else if (shipMax.activeSelf)
        {
            mbb.currentNum -= 1;
            isPopup = false;
            shipMax.SetActive(false);
        }
        else if (capMax.activeSelf)
        {
            mbb.currentNum -= 1;
            isPopup = false;
            capMax.SetActive(false);
        }
    }

    public void StartCampaign()
    {
        if (CapacityCheck())
        {
            PlayerPrefs.SetString("InGame_Captain", captainImage.spriteName);
            PlayerPrefs.SetString("InGame_CaptainName", captainName.text);
            NextChangeScene.Instance.NextGameScene();
        }
    }
}