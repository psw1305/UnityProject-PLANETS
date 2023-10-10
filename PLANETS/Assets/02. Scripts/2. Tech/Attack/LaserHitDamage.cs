using UnityEngine;
using System.Collections;

public class LaserHitDamage : MonoBehaviour 
{
	public LineRenderer laser;
	public int setOrder = 130;

    [Header("Damage")]
    public bool isEnemyBullet = false;
    public bool isHealing = false;
    public bool shieldRepair = false;
    public float radius, damageCount;
    [HideInInspector] public float laserDamage;
    bool isDamage;
    int hitNum;

    [Header("Alpha")]
    public float delay = 2.0f;
	public float fadeTime = 0.2f;
    public Color TintColor, CoreColor;
    float beginTintAlpha, beginCoreAlpha;

    [Header("Size")]
    public float laserSize = 0.1f;
	public float enlargeSpeed = 1.0f;
    float originSize;

    [Header("Animation")]
    public bool AnimateUV = false;
    public float UVTime;
    float initialBeamOffset;

    [Header("EffectManager")]
    public EffectManager particleStart;
    public EffectManager particleEnd;

    [HideInInspector] public Transform start;
    [HideInInspector] public Vector3 end;

    void Awake()
    {
        laser.sortingOrder = setOrder;
        originSize = laser.startWidth;
    }

    void OnEnable()
    {
        isDamage = false;
        hitNum = 0;

        laser.startWidth = laserSize;
        laser.endWidth   = laserSize;

        beginTintAlpha = TintColor.a;
        beginCoreAlpha = CoreColor.a;
        initialBeamOffset = Random.Range(0.0f, 5.0f);

        StartCoroutine("FadeTo");
    }

    void Update()
    {
        if (start != null)
        {
            laser.SetPosition(0, start.position);
            particleStart.transform.position = start.position;

            laser.SetPosition(1, end);
            particleEnd.transform.position = end;
        }

        if (AnimateUV)
            laser.material.SetTextureOffset("_MainTex", new Vector2(Time.time * UVTime + initialBeamOffset, 0));

        if (!isDamage && hitNum < damageCount)
            StartCoroutine("LineDamage");
    }

    IEnumerator LineDamage()
    {
        isDamage = true;

        hitNum += 1;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (isEnemyBullet)
            {
                if (!isHealing)
                {
                    PlayerHitBox phb = hit.GetComponent<PlayerHitBox>();

                    if (phb != null)
                    {
                        if (phb.psm.isShield)
                        {
                            phb.isHit = true;
                            phb.psm.ShieldDamage(laserDamage / damageCount);
                        }
                        else
                            phb.psm.Damage(laserDamage / damageCount);
                    }

                    PlayerFighterShipManager pfsm = hit.GetComponent<PlayerFighterShipManager>();

                    if (pfsm != null)
                        pfsm.Damage(laserDamage / damageCount);
                }
                else
                {
                    EnemyHitBox ehb = hit.GetComponent<EnemyHitBox>();

                    if (ehb != null)
                    {
                        if (!shieldRepair)
                            ehb.esm.Damage(-laserDamage / damageCount);
                        else
                            ehb.esm.ShieldDamage(-laserDamage / damageCount);
                    }
                }
            }
            else
            {
                if (!isHealing)
                {
                    EnemyHitBox ehb = hit.GetComponent<EnemyHitBox>();

                    if (ehb != null)
                    {
                        if (ehb.esm.isShield)
                        {
                            ehb.isHit = true;
                            ehb.esm.ShieldDamage(laserDamage / damageCount);
                        }
                        else
                            ehb.esm.Damage(laserDamage / damageCount);

                    }

                    EnemyFighterShipManager efsm = hit.GetComponent<EnemyFighterShipManager>();

                    if (efsm != null)
                        efsm.Damage(laserDamage / damageCount);
                }
                else
                {
                    PlayerHitBox phb = hit.GetComponent<PlayerHitBox>();

                    if (phb != null)
                    {
                        if (!shieldRepair)
                            phb.psm.Damage(-laserDamage / damageCount);
                        else
                            phb.psm.ShieldDamage(-laserDamage / damageCount);
                    }
                }
            }
        }

        yield return new WaitForSeconds(delay / damageCount);
        isDamage = false;
    }

    IEnumerator FadeTo() 
	{
        particleStart.EffectCheck(true);
        particleEnd.EffectCheck(true);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color tintColor = new Color(TintColor.r, TintColor.g, TintColor.b, Mathf.Lerp(0, beginTintAlpha, t));
            Color coreColor = new Color(CoreColor.r, CoreColor.g, CoreColor.b, Mathf.Lerp(0, beginCoreAlpha, t));

            laser.material.SetColor("_TintColor", tintColor);
            laser.material.SetColor("_CoreColor", coreColor);

            //if (laserSize <= originSize)
            //    laserSize = (enlargeSpeed * t) + laserSize;

            laser.startWidth = laserSize;
            laser.endWidth   = laserSize;

            yield return null;
        }

        yield return new WaitForSeconds (delay);

        particleStart.EffectCheck(false);
        particleEnd.EffectCheck(false);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color tintColor = new Color(TintColor.r, TintColor.g, TintColor.b, Mathf.Lerp(beginTintAlpha, 0, t));
            Color coreColor = new Color(CoreColor.r, CoreColor.g, CoreColor.b, Mathf.Lerp(beginCoreAlpha, 0, t));

            laser.material.SetColor("_TintColor", tintColor);
            laser.material.SetColor("_CoreColor", coreColor);

            if (laserSize >= 0)
                laserSize = (enlargeSpeed * -t) + laserSize;

            laser.startWidth = laserSize;
            laser.endWidth   = laserSize;

            yield return null;
        }

        yield return new WaitForSeconds(fadeTime);
        gameObject.SetActive(false);
    }

	void OnDisable()
	{
        hitNum = 0;

		laserSize = originSize;
		laser.material.SetColor("_TintColor", TintColor);
		laser.material.SetColor("_CoreColor", CoreColor);

        TintColor.a = beginTintAlpha;
        CoreColor.a = beginCoreAlpha;
    }
}
