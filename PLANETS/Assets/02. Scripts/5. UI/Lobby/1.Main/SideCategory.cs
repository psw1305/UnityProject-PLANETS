using UnityEngine;

public class SideCategory : MonoBehaviour
{
    public UIToggle[] side;
    public GameObject[] sidePanel;

    public void OnValueChange()
    {
        for (int i = 0; i < side.Length; i++)
        {
            if (side[i].value)
                sidePanel[i].SetActive(true);
            else
                sidePanel[i].SetActive(false);
        }
    }

    public void InitValue()
    {
        for (int i = 1; i < side.Length; i++)
        {
            side[i].value = false;
            sidePanel[i].SetActive(false);
        }

        side[0].value = true;
        sidePanel[0].SetActive(true);
    }
}
