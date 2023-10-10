using UnityEngine;

public class ObjectManager : MonoBehaviour 
{
	public Transform[] bullets;
	public int[] lengths;

	void Awake ()
	{
        ObjectPool op = GameObject.FindGameObjectWithTag("ObjectPool").GetComponent<ObjectPool>();

        for (int i = 0; i < bullets.Length; i++)
            op.AddItem(bullets[i], lengths[i]);
    }
}