using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera uiCamera;

	public float distance;
	public float minDistance;
	public float maxDistance;
	public float panSpeed = 70.0f;

	public float minimum;
	public float maximum;
	public float zoomSpeed = 10.0f;

    public bool OnTouch = true;
	public Transform chase;

    void Awake()
    {
        //chase.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -50);
    }

    void Update () 
	{
        if (OnTouch)
        {
            //distance -= Input.GetAxis("Mouse ScrollWheel") * distance;
            //distance = Mathf.Clamp(distance, minimum, maximum);

            Vector3 pos = chase.position;
            pos.x = Mathf.Clamp(pos.x, minDistance, maxDistance);
            pos.y = Mathf.Clamp(pos.y, minDistance, maxDistance);
            chase.position = pos;

            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, distance, Time.deltaTime * zoomSpeed);
            transform.position = Vector3.Lerp(transform.position, chase.position, Time.deltaTime * 7.0f);
        }
	}

    void LateUpdate () 
	{
        #if UNITY_EDITOR
        if (OnTouch)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetAxis("Mouse X") < 0)
                    chase.Translate(panSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetMouseButton(0))
            {
                if (Input.GetAxis("Mouse X") > 0)
                    chase.Translate(-panSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetMouseButton(0))
            {
                if (Input.GetAxis("Mouse Y") < 0)
                    chase.Translate(0, panSpeed * Time.deltaTime, 0);
            }

            if (Input.GetMouseButton(0))
            {
                if (Input.GetAxis("Mouse Y") > 0)
                    chase.Translate(0, -panSpeed * Time.deltaTime, 0);
            }
        }
        
        #elif UNITY_ANDROID
        if (OnTouch)
        {
        	// Camera Zoom In / Out.
        	//if (Input.touchCount == 2)
        	//{
        	//	Touch touch0 = Input.touches[0];
        	//	Touch touch1 = Input.touches[1];
        	//	float touchDistance = (touch1.position - touch0.position).magnitude;
                  //
        	//	float lastTouchDistance = ((touch1.position - touch1.deltaPosition) - (touch0.position - touch0.deltaPosition)).magnitude;	
        	//	float deltaPinch = touchDistance - lastTouchDistance;
                  //
        	//	distance -= deltaPinch * 0.35f;
        	//	distance = Mathf.Clamp(distance, minimum, maximum);
        	//}

        	// Camera drag Moving.
        	if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        	{
        		Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
        		chase.transform.Translate (-touchDeltaPosition.x * distance * 0.1f * Time.deltaTime, -touchDeltaPosition.y * distance * 0.1f * Time.deltaTime, 0);
        	}
        }
        #endif
    }

    void CameraChase (Transform target)
	{
		if (target != null) 
		{
			Vector3 screenPos = target.position;
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(screenPos.x, screenPos.y, -50.0f), Time.deltaTime * 10.0f);
		}
	}
}
