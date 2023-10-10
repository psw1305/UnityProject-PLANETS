using UnityEngine;

public class ObjectDestroy : MonoBehaviour
{
	public float time;

	void Start()
	{
		Invoke("ObjectDestiny", time);
	}

	void ObjectDestiny()
	{
		Destroy(gameObject);
	}
}
