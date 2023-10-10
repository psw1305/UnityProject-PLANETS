using UnityEngine;

public class OffScreenTarget : MonoBehaviour 
{
	public Transform goToTrack;
	Vector2 indRange;

	void Update () 
	{
		if (goToTrack != null) 
		{
			Vector3 screenPos = Camera.main.WorldToViewportPoint (goToTrack.position);

			if (screenPos.x > -0.01f && screenPos.x < 1.01f && screenPos.y > -0.01f && screenPos.y < 1.01f) 
			{
				GetComponent <UISprite> ().enabled = false;
			} 
			else 
			{
				GetComponent <UISprite> ().enabled = true;

				Vector3 diff = goToTrack.position - Camera.main.transform.position;
				float rotZ = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
				Quaternion qAxis = Quaternion.AngleAxis (rotZ, Vector3.forward);

				screenPos.x = (Mathf.Cos (Mathf.Deg2Rad * rotZ) / (16 * 0.3f)) + 0.5f;
				screenPos.y = (Mathf.Sin (Mathf.Deg2Rad * rotZ) / (9 * 0.3f)) + 0.5f;

				Vector3 UIPos = UICamera.mainCamera.ViewportToWorldPoint (screenPos);
				UIPos.z = 0;

				transform.position = UIPos;
				transform.rotation = qAxis;
			}
		} 
		else 
		{
			Destroy (gameObject);
		}
	}
}
