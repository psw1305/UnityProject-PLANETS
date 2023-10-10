using UnityEngine;
using System.Collections;

public class ActiveSkillSystem : MonoBehaviour
{
    public enum TouchType { Player, Enemy, Wherever, Instant }
    public TouchType touchType;

    public StageMainPlanet smp;
    public string skillName;
    public float coolTime;
    [HideInInspector] public float originCoolTime;
    [HideInInspector] public bool isActive = false, isCool = false;
    [HideInInspector] public PlayerFleet fleet;
    UIToggle toggle;

    [Header("Skill UI")]
    public UISprite foreground;
    public UISprite skillicon;
    public UILabel notice;
    public UILabel countDown;
    public string noticeText;

    [Header("Skill")]
    public GameObject skillObject;
    [HideInInspector] public float value1, value2;

    // 토글 스킬 활성화
    public void ShipTouch()
    {
        if (toggle.value)
        {
            isActive = true;
            notice.text = noticeText;
            notice.gameObject.SetActive(true);
        }
        else
        {
            isActive = false;
            notice.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        originCoolTime = coolTime;
        fleet = smp.pf;
        toggle = GetComponent<UIToggle>();

        StageDataBase.Instance.CaptainSkillDataParsing(smp.cType, smp.cLv, this);
    }

    void Update()
    {
        if (isActive && Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);


            switch (touchType)
            {
                case TouchType.Player:
                    if (hit.collider != null && hit.collider.CompareTag("PlayerShip"))
                        ShipTouchSkill(hit.collider.gameObject);
                    break;
                case TouchType.Enemy:
                    if (hit.collider != null && hit.collider.CompareTag("EnemyShip"))
                        ShipTouchSkill(hit.collider.gameObject);
                    break;
                case TouchType.Wherever:
                    PositionTouchSkill(mouseWorldPos);
                    break;
            }
        }

        if (coolTime > 0 && isCool)
        {
            coolTime -= Time.deltaTime;
            countDown.text = coolTime.ToString("N1");
            foreground.fillAmount = coolTime / originCoolTime;

            if (coolTime <= 0)
            {
                isCool = false;
                coolTime = originCoolTime;
                countDown.text = "READY";
                GetComponent<BoxCollider>().enabled = true;
            }
        }
        else if (fleet == null)
        {
            countDown.text = "DISABLE";
            GetComponent<BoxCollider>().enabled = false;
            toggle.value = false;
        }
    }

    void SkillDisable()
    {
        isCool = true;
        toggle.value = false;
        GetComponent<BoxCollider>().enabled = false;
    }

    void ShipTouchSkill(GameObject target)
    {
        SkillDisable();

        switch (skillName)
        {
            case "Targeting":
                StartCoroutine(Targeting(target));
                break;
            case "AddCrew":
                AddCrew(target);
                break;
        }
    }

    void PositionTouchSkill(Vector3 pos)
    {
        SkillDisable();

        switch (skillName)
        {
            case "NuclearMissile":
                NuclearMissile(pos);
                break;
        }
    }

    IEnumerator Targeting(GameObject target)
    {
        target.GetComponentInParent<EnemyShipManager>().eseg.EffectTimeInstance("Targeting", 15);

        for (int i = 0; i < fleet.playerShips.Count; i++)
        {
            if (fleet.playerShips[i] != null)
            {
                fleet.playerShips[i].GetComponent<PlayerShipManager>().psmv.isTarget = true;
                fleet.playerShips[i].GetComponent<PlayerShipManager>().psmv.targeted = target;
            }
        }

        yield return new WaitForSeconds(15);

        for (int i = 0; i < fleet.playerShips.Count; i++)
        {
            if (fleet.playerShips[i] != null)
            {
                fleet.playerShips[i].GetComponent<PlayerShipManager>().psmv.isTarget = false;
                fleet.playerShips[i].GetComponent<PlayerShipManager>().psmv.targeted = null;
            }
        }
    }

    void NuclearMissile(Vector3 dest)
    {
        GameObject ballista = Instantiate(skillObject, new Vector3(-400, Random.Range(-400, 400), 0), transform.rotation) as GameObject;
        ballista.GetComponent<EnemyHitDamage>().destination = dest;
        ballista.GetComponent<EnemyHitDamage>().explosion.GetComponent<PlayerAura>().damage = value1;
    }

    void AddCrew(GameObject target)
    {
        target.GetComponentInParent<PlayerShipManager>().seg.EffectGenerator("AddCrew", 0, 0, 0, 30);
    }

    public void FocusedDefense()
    {
        if (toggle.value)
        {
            SkillDisable();

            for (int i = 0; i < fleet.playerShips.Count; i++)
            {
                if (fleet.playerShips[i] != null)
                    fleet.playerShips[i].GetComponent<PlayerShipManager>().seg.EffectGenerator("DefenseUp2", value2, value1, 0, 1);
            }
        }
    }
}
