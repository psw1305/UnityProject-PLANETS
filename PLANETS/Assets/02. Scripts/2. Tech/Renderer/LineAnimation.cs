using UnityEngine;

public class LineAnimation : MonoBehaviour
{
    private LineRenderer playerLine;
    private float initOffset;
    private float distance;

    void Start()
    {
        playerLine = GetComponent<LineRenderer>();
        initOffset = Random.Range(0.0f, 5.0f);
    }

    void Update()
    {
        distance = Vector2.Distance(playerLine.GetPosition(0), playerLine.GetPosition(1));
        playerLine.material.SetTextureScale("_MainTex", new Vector2(distance * 0.05f, 1));
        playerLine.material.SetTextureOffset("_MainTex", new Vector2(Time.time * -1 + initOffset, 0f));
    }
}