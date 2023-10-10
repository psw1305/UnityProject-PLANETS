using UnityEngine;

public class ResourceData : MonoBehaviour
{
    [Header("Resource")]
    public UILabel creditLabel;
    public UILabel coreLabel;
    public UILabel dmatterLabel;
    public UILabel acoreLabel;

    [HideInInspector] public int credit, core, dmatter, acore;
    [HideInInspector] public bool resourceCheck, cancleTrade;

    [Header("Popup")]
    public GameObject popupResource;
    [HideInInspector] public MainBackButton mbb;
    [HideInInspector] public int mbbNum;
    [HideInInspector] public bool isPopup = false;

    void Awake()
    {
        int creditCheck  = PlayerPrefs.GetInt("Credit");
        int coreCheck    = PlayerPrefs.GetInt("Core");
        int dmatterCheck = PlayerPrefs.GetInt("DMatter");
        int acoreCheck   = PlayerPrefs.GetInt("ACore");

        if (creditCheck != 0)
            creditLabel.text = string.Format("{0:#,###}", creditCheck);
        else
            creditLabel.text = "0";

        if (coreCheck != 0)
            coreLabel.text = string.Format("{0:#,###}", coreCheck);
        else
            coreLabel.text = "0";

        if (dmatterCheck != 0)
            dmatterLabel.text = string.Format("{0:#,###}", dmatterCheck);
        else
            dmatterLabel.text = "0";

        if (acoreCheck != 0)
            acoreLabel.text = string.Format("{0:#,###}", acoreCheck);
        else
            acoreLabel.text = "0";
    }

    void LateUpdate()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (mbb != null)
            {
                if (isPopup && mbb.currentNum == mbbNum)
                    PopupClose();
            }
        }
        #endif
    }

    public void PopupClose()
    {
        isPopup = false;

        if (mbb != null)
            mbb.currentNum -= 1;

        if (popupResource.activeSelf == true)
            popupResource.SetActive(false);
    }

    public int PayResource(string resourceName, int resource)
    {
        if (resourceCheck)
        {
            int playerResource = PlayerPrefs.GetInt(resourceName);
            playerResource -= resource;

            if (playerResource < 0)
            {
                playerResource = 0;

                resourceCheck = false;
                isPopup = true;

                if (mbb != null)
                    mbb.currentNum += 1;

                popupResource.SetActive(true);
            }

            return playerResource;
        }
        else
            return 0;

    }
    
    public void RefundResource(string resourceName, int resource)
    {
        int playerResource = PlayerPrefs.GetInt(resourceName);
        playerResource += resource;

        ResourceLabel(resourceName, playerResource);
    }

    public void ResourceLabel(string resourceName, int resource)
    {
        switch (resourceName)
        {
            case "Credit":
                if (resource != 0)
                    creditLabel.text = string.Format("{0:#,###}", resource);
                else
                    creditLabel.text = "0";
                PlayerPrefs.SetInt(resourceName, resource);
                break;
            case "Core":
                if (resource != 0)
                    coreLabel.text = string.Format("{0:#,###}", resource);
                else
                    coreLabel.text = "0";
                PlayerPrefs.SetInt(resourceName, resource);
                break;
            case "DMatter":
                if (resource != 0)
                    dmatterLabel.text = string.Format("{0:#,###}", resource);
                else
                    dmatterLabel.text = "0";
                PlayerPrefs.SetInt(resourceName, resource);
                break;
            case "ACore":
                if (resource != 0)
                    acoreLabel.text = string.Format("{0:#,###}", resource);
                else
                    acoreLabel.text = "0";
                PlayerPrefs.SetInt(resourceName, resource);
                break;
        }
    }
}
