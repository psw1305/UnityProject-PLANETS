using UnityEngine;

public class MainCategory : MonoBehaviour
{
    [Header("Lobby")]
    public GameObject title;
    public UILabel lobbyLabel;
    public GameObject lobby, lobbyBtn;

    [Header("List")]
    public GameObject menu;
    public GameObject credits;
    public GameObject[] mains;
    public SideCategory[] sides;

    [Header("Popup")]
    public GameObject popup;
    public GameObject exit;
    public GameObject reset;

    [HideInInspector] public int buildCheck = 0;
    [HideInInspector] public bool isSetting = false;
    [HideInInspector] public bool isUpgrade = false;
    bool isBuilding = false, isManage   = false, isExit     = false, isMenu     = false, isCredit = false;
    bool isMilitary = false, isResearch = false, isIndustry = false, isCampaign = false, isReset  = false, isExit2 = false;

    void Update()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isUpgrade && !isBuilding && !isManage && !isMilitary && !isResearch && !isIndustry && !isCampaign && !isExit && !isMenu && !isCredit && !isExit2 && !isReset)
            {
                isExit = true;
                exit.SetActive(true);
            }
            else if (!isUpgrade && !isBuilding && !isManage && !isMilitary && !isResearch && !isIndustry && !isCampaign && isExit && !isMenu && !isCredit && !isExit2 && !isReset)
                ExitCancle();
            else if (isMenu)
                OnMenu();
            else if (isCredit)
                OnCredits();
            else if (isExit2)
                OnExit();
            else if (isReset)
                OnReset();

        }
        #endif
    }

    public void OnMenu()
    {
        if (!isMenu)
        {
            isMenu = true;
            menu.SetActive(true);
        }
        else if (isMenu)
        {
            isMenu = false;
            menu.SetActive(false);
        }
    }

    public void OnCredits()
    {
        if (!isCredit)
        {
            isCredit = true;
            isMenu = false;

            title.SetActive(false);
            lobby.SetActive(false);
            lobbyBtn.SetActive(false);
            popup.SetActive(false);

            credits.SetActive(true);
        }
        else if (isCredit)
        {
            isCredit = false;
            isMenu = true;

            title.SetActive(true);
            lobby.SetActive(true);
            lobbyBtn.SetActive(true);
            popup.SetActive(true);

            credits.SetActive(false);
        }
    }

    public void OnExit()
    {
        if (!isExit2)
        {
            isExit2 = true;
            exit.SetActive(true);
        }
        else if (isExit2)
        {
            isExit2 = false;
            exit.SetActive(false);
        }
    }

    public void OnReset()
    {
        if (!isReset)
        {
            isReset = true;
            reset.SetActive(true);
        }
        else if (isReset)
        {
            isReset = false;
            reset.SetActive(false);
        }
    }

    public void OnBuilding()
    {
        if (!isSetting)
        {
            isSetting  = true;
            isBuilding = true;

            lobbyLabel.text = "건설";
            mains[0].SetActive(true);
        }
        else if (isSetting)
        {
            sides[0].InitValue();

            isSetting  = false;
            isBuilding = false;

            lobbyLabel.text = "로비";
            mains[0].SetActive(false);
        }
    }

    public void OnManage()
    {
        if (!isSetting)
        {
            isSetting = true;
            isManage  = true;

            lobbyLabel.text = "관리";
            lobby.SetActive(false);
            lobbyBtn.SetActive(false);
            mains[1].SetActive(true);
        }
        else if (isSetting)
        {
            isSetting = false;
            isManage  = false;

            lobbyLabel.text = "로비";
            lobby.SetActive(true);
            lobbyBtn.SetActive(true);
            mains[1].SetActive(false);
        }
    }

    public void OnIndustry()
    {
        if (!isSetting)
        {
            isSetting = true;
            isIndustry = true;

            lobbyLabel.text = "생산";
            lobby.SetActive(false);
            lobbyBtn.SetActive(false);
            mains[2].SetActive(true);
        }
        else if (isSetting)
        {
            isSetting = false;
            isIndustry = false;

            lobbyLabel.text = "로비";
            lobby.SetActive(true);
            lobbyBtn.SetActive(true);
            mains[2].SetActive(false);
        }
    }

    public void OnMilitary()
    {
        if (!isSetting)
        {
            isSetting  = true;
            isMilitary = true;

            lobbyLabel.text = "군사";
            lobby.SetActive(false);
            lobbyBtn.SetActive(false);
            mains[3].SetActive(true);
        }
        else if (isSetting)
        {
            isSetting  = false;
            isMilitary = false;

            lobbyLabel.text = "로비";
            lobby.SetActive(true);
            lobbyBtn.SetActive(true);
            mains[3].SetActive(false);
        }
    }

    public void OnResearch()
    {
        if (!isSetting)
        {
            isSetting  = true;
            isResearch = true;

            lobbyLabel.text = "연구";
            lobby.SetActive(false);
            lobbyBtn.SetActive(false);
            mains[4].SetActive(true);
        }
        else if (isSetting)
        {
            sides[1].InitValue();

            isSetting  = false;
            isResearch = false;

            lobbyLabel.text = "로비";
            lobby.SetActive(true);
            lobbyBtn.SetActive(true);
            mains[4].SetActive(false);
        }
    }

    public void OnCampaign()
    {
        if (!isSetting)
        {
            isSetting  = true;
            isCampaign = true;

            lobbyLabel.text = "출정";
            lobby.SetActive(false);
            lobbyBtn.SetActive(false);
            mains[5].SetActive(true);
        }
        else if (isSetting)
        {
            isSetting  = false;
            isCampaign = false;

            lobbyLabel.text = "로비";
            lobby.SetActive(true);
            lobbyBtn.SetActive(true);
            mains[5].SetActive(false);
        }
    }

    public void TurnSkip()
    {
        NextChangeScene.Instance.NextLobbyScene();
    }

    public void DataDelete()
    {
        NextChangeScene.Instance.DataDelete();
    }

    public void ExitCancle()
    {
        isExit = false;
        exit.SetActive(false);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
