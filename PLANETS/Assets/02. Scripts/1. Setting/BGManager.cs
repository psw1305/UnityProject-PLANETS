using UnityEngine;

public class BGManager : MonoBehaviour 
{
	void FixedUpdate ()
	{
		Vector2 cameraPos = Camera.main.transform.position;
		transform.position = cameraPos;

		ResizeSpriteToScreen ();
	}

	void ResizeSpriteToScreen ()
	{
		SpriteRenderer sr = GetComponent <SpriteRenderer> ();
		if (sr == null) return;

		transform.localScale = new Vector3 (1, 1, 1);
		float width = sr.sprite.bounds.size.x;
		float height = sr.sprite.bounds.size.y;

		float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		Vector3 xWidth = transform.localScale;
		xWidth.x = worldScreenWidth / width;
		transform.localScale = xWidth;

		Vector3 yHeight = transform.localScale;
		yHeight.y = worldScreenHeight / height;
		transform.localScale = yHeight;
	}

}
