using UnityEngine;

public class BackgroundEventTrigger : MonoBehaviour 
{
    public GameObject[] background;

	void Start()
	{
        int num = Random.Range(0, 4);

        switch (num)
        {
            case 0:
                background[0].SetActive(true);
                break;
            case 1:
                background[1].SetActive(true);
                break;
            case 2:
                background[2].SetActive(true);
                break;
            case 3:
                background[3].SetActive(true);
                break;
        }
	}
}
