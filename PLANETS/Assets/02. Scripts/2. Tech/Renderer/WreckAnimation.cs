using System.Collections;
using UnityEngine;

public class WreckAnimation : MonoBehaviour 
{
    [Header("Wreck")]
    public GameObject finale;
	public Rigidbody2D[] wrecks;
    public float alphaTime = 5.0f;

	void Start ()
	{
        if (wrecks != null)
        {
            for (int j = 0; j < wrecks.Length; j++)
            {
                float randomX = Random.Range(-75, 75);
                float randomY = Random.Range(-75, 75);
                float randomT = Random.Range(-25, 25);

                wrecks[j].gameObject.SetActive(true);
                wrecks[j].AddForce(new Vector2(randomX, randomY));
                wrecks[j].AddTorque(randomT);
            }

            for (int k = 0; k < wrecks.Length; k++)
            {
                StartCoroutine(FadeTo(0.0f, alphaTime, wrecks[k].transform));
            }
        }
    }

    IEnumerator FadeTo (float aValue, float aTime, Transform image)
	{
		float alpha = image.GetComponent<SpriteRenderer>().color.a;

		for (float i = 0.0f; i < 1.0f; i += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, i));
			image.GetComponent<SpriteRenderer>().color = newColor;
			yield return null;
		}
	}
}
