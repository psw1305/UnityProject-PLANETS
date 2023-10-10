using System.Collections;
using UnityEngine;

public class PlayerMiningEffect : MonoBehaviour
{
    public GameObject laserEffect;
    public GameObject[] MiningBox;
    public Transform[] startTransform;
    public Transform endTransform;
    public float delay;
    public bool isMining = false;

    int mining = 0;
    float rotZ = 0;
    bool isRotate = false;

    void Update()
    {
        if (isMining)
            StartCoroutine("MiningEffect");

        if (isRotate)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, rotZ), 4 * Time.deltaTime);
    }

    IEnumerator MiningEffect()
    {
        isMining = false;
        isRotate = true;
        mining += 1;
        rotZ = Random.Range(-10, 10);

        yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
        isRotate = false;

        //GameObject effect1 = Instantiate(laserEffect, startTransform[0].position, Quaternion.identity) as GameObject;
        //effect1.GetComponent<LineRendererManager>().start = startTransform[0];
        //effect1.GetComponent<LineRendererManager>().end   = endTransform.position;

        //GameObject effect2 = Instantiate(laserEffect, startTransform[1].position, Quaternion.identity) as GameObject;
        //effect2.GetComponent<LineRendererManager>().start = startTransform[1];
        //effect2.GetComponent<LineRendererManager>().end   = endTransform.position;

        yield return new WaitForSeconds(delay - 2);
        isMining = true;
    }
}
