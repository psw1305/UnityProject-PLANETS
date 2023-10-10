using UnityEngine;

public class ParallaxLayer : MonoBehaviour 
{
	public float movement_resistance = 1f;

	void LateUpdate ()
	{
		Vector3 wantedPosition = Camera.main.transform.position * movement_resistance;
		wantedPosition.z = transform.position.z;
		transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * 10.0f);
	}
}
