using UnityEngine;

public class CaptainManager : MonoBehaviour
{
    public CaptainUIManager um;
    public EmbarkManager em;

    [Header("Captain")]
    public UISprite captainImage;
    public UILabel captainName;
    public UILabel captainLevel;
    public GameObject captainLock;

    [Header("Embark")]
    public UIToggle embark;
    public UILabel embarkLevel;
    public GameObject embarkLock;

    [Header("Skill")]
    public UISprite skillIcon;
    public UILabel skillName;
    public UILabel skillInfo;

    [HideInInspector] public string cType, cName, cRace;
    [HideInInspector] public string sType, sName, sInfo;

    public void Init()
    {
        if (PlayerPrefs.HasKey("Captain_Lock_" + cName))
        {
            captainLock.SetActive(false);
            embarkLock.SetActive(false);
            embark.enabled = true;

            captainLevel.text = "Lv." + PlayerPrefs.GetInt("Captain_Level_" + cName);
            embarkLevel.text  = "Lv." + PlayerPrefs.GetInt("Captain_Level_" + cName);
        }
    }

    public void CaptainEmbark()
    {
        if (embark.value)
            em.CaptainSelected(cType, cName);
        else
            em.CaptainSelected("Captain_Empty", "장교 선택창");
    }

    public void Unlock()
    {
        PlayerPrefs.SetInt("Captain_Lock_" + cName, 1);
        PlayerPrefs.SetInt("Captain_Level_" + cName, 1);
        PlayerPrefs.SetFloat("Captain_Exp_" + cName, 0);

        captainLock.SetActive(false);
        embarkLock.SetActive(false);
        embark.enabled = true;

        captainLevel.text = "Lv." + PlayerPrefs.GetInt("Captain_Level_" + cName);
        embarkLevel.text  = "Lv." + PlayerPrefs.GetInt("Captain_Level_" + cName);
    }

    public void PopupActive()
    {
        int capacity = PlayerPrefs.GetInt("CaptainCapacity", 0);
        int max;

        um.captImage.spriteName = cType;
        um.captName.text = cName;
        
        if (PlayerPrefs.HasKey("Building_SpaceHabitat"))
        {
            max = 6;
            um.captMaxLabel.text = "장교 수가 6명에 도달하여" + '\n' + "장교를 더 고용할 수 없습니다.";
        }
        else
        {
            max = 3;
            um.captMaxLabel.text = "장교 수가 3명에 도달하여" + '\n' + "장교를 더 고용할 수 없습니다." + '\n' + "(장교 거주지 건설시 +3)";
        }

        if (capacity < max)
        {
            um.select = this;

            um.isPopup = true;
            um.mbb.currentNum = 1;
            um.captBuy.SetActive(true);
        }
        else
        {
            um.isPopup = true;
            um.mbb.currentNum = 1;
            um.captMax.SetActive(true);
        }
    }
}
