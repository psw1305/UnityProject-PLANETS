using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public GameObject building;
    public UIButton buildBtn;
    public UILabel buildBtnLabel;

    [Header("Data")]
    public string buildName;
    [HideInInspector] public int credit, core, acore;
    [HideInInspector] public BuildingManager bm;

    public void DataCheck()
    {
        if (PlayerPrefs.HasKey("Building_" + buildName))
        {
            buildBtn.isEnabled = false;
            buildBtnLabel.text = "건설완료";
            building.SetActive(true);
        }
    }

    public void PopupActive()
    {
        bm.building  = building;
        bm.buildName = buildName;
        bm.check     = this;

        bm.credit = credit;
        bm.core   = core;
        bm.acore  = acore;

        bm.popupName.text = buildName;

        bm.creditLabel.text = credit.ToString();
        bm.coreLabel.text   = core.ToString();
        bm.acoreLabel.text  = acore.ToString();

        bm.isPopup = true;
        bm.mbb.currentNum = 1;
        bm.popup.SetActive(true);
    }
}
