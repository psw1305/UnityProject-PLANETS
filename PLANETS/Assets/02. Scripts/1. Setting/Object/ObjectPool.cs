using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour 
{
	public static ObjectPool instance; 
	public List<Transform> poolItems; 
	public List<int> poolLength;
	private Dictionary <Transform, Transform[]> pool;

	void Start()
	{
        InitObjectPool();
	}

    void InitObjectPool()
    {
        instance = this;

        if (poolItems.Count > 0)
        {
            pool = new Dictionary<Transform, Transform[]>();

            for (int i = 0; i < poolItems.Count; i++)
            {
                Transform[] itemArray = new Transform[poolLength[i]];

                for (int x = 0; x < poolLength[i]; x++)
                {
                    Transform newItem = Instantiate(poolItems[i]);
                    newItem.gameObject.SetActive(false);
                    newItem.parent = transform;
                    itemArray[x] = newItem;

                    //yield return new WaitForEndOfFrame();
                }

                if (!pool.ContainsKey(poolItems[i]))
                    pool.Add(poolItems[i], itemArray);
            }
        }
    }

	public Transform Spawn(Transform obj)
	{
		for (int i = 0; i < pool[obj].Length; i++)
		{
			if (!pool[obj][i].gameObject.activeSelf)
			{
				Transform spawnItem = pool[obj][i];
				spawnItem.gameObject.SetActive(true);

				return spawnItem;
			}
		}
		
		return null;
	}

	public void AddItem(Transform item, int length)
	{
        bool isContain = false;
        int cnt = 0;
        
        for (int i = 0; i < poolItems.Count; i++)
        {
            if (poolItems[i].Equals(item))
            {
                isContain = true;
                cnt = i;
            }
        }

        if (!isContain)
        {
            poolItems.Add(item);
            poolLength.Add(length);
        }
        else
            poolLength[cnt] += length;
    }
}
