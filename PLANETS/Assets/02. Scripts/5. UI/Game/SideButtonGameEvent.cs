using UnityEngine;

public class SideButtonGameEvent : MonoBehaviour 
{
	[Header ("Side - Public")]
	public GameObject MainPanel;
	public GameObject NormalPanel;
	public GameObject SettingPanel;

    public GameObject blind, cameraObject;
	private BoxCollider[] arrBoxCollider;
	private bool isSetting;

	TweenAlpha alpha;
	CameraManager cm;

	private float speed;
	public UISprite speedIcon;
	
	void Awake()
	{
		arrBoxCollider = MainPanel.GetComponentsInChildren<BoxCollider>();
		isSetting = false;
		
		alpha = blind.GetComponent<TweenAlpha>();
		cm = cameraObject.GetComponent<CameraManager>();

		Time.timeScale = 1.0f;
		speed = 1.0f;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			OnSettingEvent();
		}
	}

	void NGUIFunction(bool active)
	{
		for (int i = 0; i <= arrBoxCollider.Length - 1; i++) 
		{
			arrBoxCollider[i].enabled = active;
		}
	}
	
	public void OnSettingEvent()
	{
		if (!isSetting) 
		{
			Time.timeScale = 0.0f;
			isSetting = true;

			cm.OnTouch = false;
			NormalPanel.SetActive(false);
			SettingPanel.SetActive(true);
				
			alpha.PlayForward();
		}
		else if (isSetting)
		{
			Time.timeScale = speed;
			isSetting = false;

            cm.OnTouch = true;
            NormalPanel.SetActive(true);
            SettingPanel.SetActive(false);

            alpha.PlayReverse();
        }
	}

    public void OnDisableButton()
	{
		if (isSetting)
		{
			Time.timeScale = speed;
			isSetting = false;

			cm.OnTouch = true;
			NormalPanel.SetActive(true);
			SettingPanel.SetActive(false);
			
			alpha.PlayReverse();
		}
	}

	public void CloseAccept()
	{
		Application.Quit();
	}

	public void BackToHome()
	{
		speed = 1.0f;
		OnDisableButton();
	}

    public void GameSpeed()
    {
        if (speed == 1.0f)
        {
            speed = 1.5f;
            Time.timeScale = speed;
            speedIcon.spriteName = "Icon_Setting_Speed2";
        }
        else if (speed == 1.5f)
        {
            speed = 1.0f;
            Time.timeScale = speed;
            speedIcon.spriteName = "Icon_Setting_Speed1";
        }
    }
}
