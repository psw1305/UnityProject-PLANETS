using UnityEngine;
using System.Collections;

public class ExploseAnimation : MonoBehaviour
{
    [Header("Explose")]
    public GameObject[] exploses;
    public float exploseDelay = 0.5f;

    void Start()
    {
        for (int i = 0; i < exploses.Length; i++)
        {
            GameObject temp = exploses[i];
            int randomIndex = Random.Range(i, exploses.Length);
            exploses[i] = exploses[randomIndex];
            exploses[randomIndex] = temp;
        }

        StartCoroutine("ExploseTo");
    }

    IEnumerator ExploseTo()
    {
        for (int i = 0; i < exploses.Length; i++)
        {
            exploses[i].SetActive(true);
            yield return new WaitForSeconds(exploseDelay);
        }
    }
}
