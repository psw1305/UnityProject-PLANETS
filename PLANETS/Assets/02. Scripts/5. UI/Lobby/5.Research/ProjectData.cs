using UnityEngine;

public class ProjectData : MonoBehaviour
{
    public int level;
    public string plus;
    [HideInInspector] public bool isResearch = false, isNext = false;

    [Header("UI")]
    public UIToggle toggle;
    public UISprite frame;
    public UISprite check;
    public UISprite icon;

    [Header("Script")]
    public ProjectData[] nextPd;
    public ProjectManager pm;

    public void ProjectDataParsing()
    {
        PlayerDataBase.Instance.ProjectDataParsing(this, pm);

        if (PlayerPrefs.HasKey("Building_ParticleAccelerator1") && PlayerPrefs.HasKey("Building_ParticleAccelerator2"))
        {
            pm.credit  = (int)(pm.credit * 0.7f);
            pm.dmatter = (int)(pm.dmatter * 0.7f);
        }
        else if (PlayerPrefs.HasKey("Building_ParticleAccelerator1") || PlayerPrefs.HasKey("Building_ParticleAccelerator2"))
        {
            pm.credit = (int)(pm.credit * 0.85f);
            pm.dmatter = (int)(pm.dmatter * 0.85f);
        }
    }

    void OnEnable()
    {
        if (level == 5)
        {
            if (PlayerPrefs.HasKey("Building_SuperComputer"))
            {
                frame.spriteName = "Lab_Skill_Frame";
                check.gameObject.SetActive(false);
            }
        }
    }

    public void OnValueChange()
    {
        if (toggle.value)
        {
            ProjectDataParsing();

            if (!isNext)
                pm.startBtn.gameObject.SetActive(false);
            else
                pm.startBtn.gameObject.SetActive(true);
        }
    }

    public void OnRelease()
    {
        isNext = true;
        check.gameObject.SetActive(false);
        frame.spriteName = "Lab_Skill_Frame";
    }

    public void OnComplete()
    {
        if (isResearch)
        {
            isNext = false;
            check.gameObject.SetActive(true);
            frame.spriteName = "Lab_Skill_Frame Complete";
            check.spriteName = "Lab_Skill_Complete";
        }
    }

    public void OnUncomplete()
    {
        if (!isResearch)
        {
            isNext = false;
            check.gameObject.SetActive(true);
            frame.spriteName = "Lab_Skill_Frame";
            check.spriteName = "Captain_Trait_Lock";
        }
    }
}
