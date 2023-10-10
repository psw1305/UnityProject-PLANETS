using UnityEngine;

public class UIGageManager : MonoBehaviour 
{
    [HideInInspector] public Transform target;
    [HideInInspector] public float caseYPos = -100;
    public Transform barCase;

    [Header("Health")]
    public UISprite hpBar;
    public UILabel hpText;

    [Header("Shield")]
    public UISprite apBar;
    public UILabel apText;

    [Header("OverHealth")]
    public UISprite opBar;
    public UILabel opText;

    [Header("Crew")]
    public UISprite crew;
    public string[] state;
    public UILabel mpText;

    Vector2 screenPos, uiScreenPos;

    void Start()
    {
        barCase.localPosition = new Vector3(0, caseYPos, 0);
    }

    void Update()
	{
        if (target != null)
        {
            screenPos   = Camera.main.WorldToScreenPoint(target.transform.position);
            uiScreenPos = CameraManager.Instance.uiCamera.ScreenToWorldPoint(screenPos);
            transform.position = new Vector3(uiScreenPos.x, uiScreenPos.y, 0);
        }
	}

    public void CrewAlert(int alertLevel)
    {
        if (alertLevel == 1)
            crew.spriteName = state[0];
        else if (alertLevel == 2)
            crew.spriteName = state[1];
        else if (alertLevel == 3)
            crew.spriteName = state[2];
        else if (alertLevel == 4)
            crew.spriteName = state[3];
    }
}