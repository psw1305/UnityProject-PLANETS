using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public GameObject popup;
    public string current;
    public string next;
    public TweenAlpha ta;

    void Start()
    {
        if (!PlayerPrefs.HasKey("Check_" + current))
            ta.PlayForward();
    }

    void Update()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (popup.activeSelf)
                PopupCancle();
        }
        #endif
    }

    public void PopupActive()
    {
        popup.SetActive(true);

        if (!PlayerPrefs.HasKey("Check_" + current))
        {
            PlayerPrefs.SetInt("Check_" + current, 1);
            ta.Finish();
        }
    }

    public void PopupCancle()
    {
        popup.SetActive(false);
    }

    public void NextPopup()
    {
        if (popup.GetComponent<UISprite>().spriteName != next)
            popup.GetComponent<UISprite>().spriteName = next;
        else
        {
            popup.SetActive(false);
            popup.GetComponent<UISprite>().spriteName = current;
        }

        if (!PlayerPrefs.HasKey("Check_" + current))
        {
            PlayerPrefs.SetInt("Check_" + current, 1);
            ta.Finish();
        }
    }
}
