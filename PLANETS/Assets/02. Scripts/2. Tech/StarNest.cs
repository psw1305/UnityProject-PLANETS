// https://www.reddit.com/r/Unity3D/comments/3qvoo7/resource_star_nest_shader_ported_from_shadertoy/

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class StarNest : MonoBehaviour
{
	public int iterations = 17;
	public float formuparam = 0.53f;
	
	public int volsteps = 20;
	public float stepsize = 0.1f;
	
	public float zoom = 0.800f;
	public float tile = 0.850f;
	public float speed = 0.010f;
	public Vector2 speedMultiplier = new Vector2(1,2);
	
	public float brightness = 0.0015f;
	public float darkmatter = 0.300f;
	public float distfading = 0.730f;
	public float saturation = 0.850f;
	
	private Material _material;
	private Material Material
	{
		get
		{
			if (_material == null)
				_material = new Material(Shader.Find("Hidden/ShaderToy/StarNest"));
			
			return _material;
		}
	}
	
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//This part doesn't work so great. You're welcome to play with it. :)
		//Material.SetFloat("_OffsetX", offset.x);
		//Material.SetFloat("_OffsetX", offset.y);
		
		Material.SetFloat("_Iterations", iterations);
		Material.SetFloat("_Formuparam", formuparam);
		
		Material.SetFloat("_Volsteps", volsteps);
		Material.SetFloat("_Stepsize", stepsize);
		
		Material.SetFloat("_Zoom", zoom);
		Material.SetFloat("_Tile", tile);
		Material.SetFloat("_Speed", speed);
		Material.SetFloat("_XSpeedMultiplier", speedMultiplier.x);
		Material.SetFloat("_YSpeedMultiplier", speedMultiplier.y);
		
		Material.SetFloat("_Brightness", brightness);
		Material.SetFloat("_Darkmatter", darkmatter);
		Material.SetFloat("_Distfading", distfading);
		Material.SetFloat("_Saturation", saturation);
		
		Graphics.Blit(source, destination, Material);
	}
}