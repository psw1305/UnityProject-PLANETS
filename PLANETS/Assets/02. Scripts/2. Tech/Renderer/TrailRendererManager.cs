using UnityEngine;

public class TrailRendererManager : MonoBehaviour
{
    public int sortingOrder;

    void Start()
    {
        GetComponent<TrailRenderer>().sortingOrder = sortingOrder;
    }
}
