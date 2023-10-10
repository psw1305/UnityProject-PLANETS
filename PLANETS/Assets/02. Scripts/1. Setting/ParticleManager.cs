
using UnityEngine;

public class ParticleManager : MonoBehaviour 
{
	private ParticleSystem particle;
	private ParticleSystem[] particles;
	public int order;

	void Start()
	{
		particle = GetComponent<ParticleSystem>() as ParticleSystem;
		particle.GetComponent<Renderer>().sortingOrder = order;
	}
}
