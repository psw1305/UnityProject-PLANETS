using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public bool isDisable = false;
    public ParticleSystem[] effects;
    public int effectSortingOrder;
    public float effectTime;

    public void EffectSortingOrder(int order)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            ParticleSystem ps = effects[i];
            ps.GetComponent<Renderer>().sortingOrder = order;
        }
    }

    void Awake()
    {
        EffectSortingOrder(effectSortingOrder);
    }

    void OnEnable()
    {
        if (isDisable) EffectCheck(false);
    }

    public void Effect()
    {
        EffectCheck(true);
        StartCoroutine("EffectActive");
    }

    public void EffectCheck(bool check)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            ParticleSystem ps = effects[i];
            var em = ps.emission;
            em.enabled = check;
        }
    }

    IEnumerator EffectActive()
    {
        yield return new WaitForSeconds(effectTime);
        EffectCheck(false);
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
