using UnityEngine;

public class TitleBackButton : MonoBehaviour
{
    public GameObject popup;
    bool isPopup = false;

    void Update()
    {
        #if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPopup)
            {
                isPopup = true;
                popup.SetActive(true);
            }
            else
            {
                isPopup = false;
                popup.SetActive(false);
            }
        }
        #endif
    }

    public void PopupClose()
    {
        isPopup = false;
        popup.SetActive(false);
    }
}
