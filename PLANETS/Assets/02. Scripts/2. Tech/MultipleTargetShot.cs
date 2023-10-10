using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleTargetShot : MonoBehaviour 
{
	public GameObject player;
	public GameObject target;
	private GameObject enemy;	
	List<Transform> enemys = new List<Transform> ();
	
	public GameObject missile;
	public float missileAmmos = 3;	
	private bool shooting = true;
	private bool targeting = true;
	static int choose;

	void Start ()
	{
		choose = 0;
	}

	void Update ()
	{
		enemy = GameObject.FindGameObjectWithTag ("Enemy");
		
		if (enemy != null) 
		{
			if (!targeting)	FindTarget ();
			if (!shooting)	StartCoroutine ("ContinueFire");
		}
	}

	void FindTarget ()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
			
			if (hit.collider != null) 
			{
				if (hit.collider.gameObject.CompareTag ("Enemy")) 
				{
					enemys.Add (hit.collider.transform);
					GameObject obj = Instantiate (target, enemys [choose].position, enemys [choose].rotation) as GameObject;
					obj.transform.parent = enemys [choose];				
					choose += 1;
					
					if (choose >= missileAmmos)
					{
						targeting = true;
						shooting = false;
						choose = 0;
					}
				}
			}
		}
	}

	public void MultipleShot ()
	{
		OnShooting ();
	}

	void Aiming (Transform target) 
	{
        Vector3 targetDir = target.position - player.transform.position;	
		targetDir.Normalize ();
		
		float rotZ = Mathf.Atan2 (targetDir.y, targetDir.x) * Mathf.Rad2Deg;	
		player.transform.rotation = Quaternion.Euler (0f, 0f, rotZ);
	}
	
	IEnumerator ContinueFire () 
	{
		shooting = true;

		for (int i = 0; i < enemys.Count; i++) 
		{
			if (enemys[i] != null)
			{
				Aiming (enemys[i]);
                Instantiate(missile, player.transform.position, player.transform.rotation);
                yield return new WaitForSeconds (1f);
			}
		}
	}
	
	void OnShooting ()
	{
		enemys.Clear ();
		targeting = false;
	}
}
