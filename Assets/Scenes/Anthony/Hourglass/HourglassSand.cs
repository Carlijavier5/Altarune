using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HourglassSand : MonoBehaviour
{
	[SerializeField]
	Mesh mesh;
	[SerializeField]
	Renderer renderer;

	[Range(0f, 1f)]
	public float fill;
	[Range(-0.5f, 1)]
	public float shapePadding;
	
	Vector3 pos;
	Vector3 tempPos;

	MaterialPropertyBlock mpb;
	public MaterialPropertyBlock Mpb
	{
		get
		{
			if (mpb == null)
				mpb = new MaterialPropertyBlock();
			return mpb;
		}
	}
	
    void Start()
    {
	    if (mesh == null)
	    {
		    mesh = GetComponent<MeshFilter>().sharedMesh;
	    }
	    if (renderer == null)
	    {
		    renderer = GetComponent<Renderer>();
	    }
    }

    void Update()
    {
	    renderer.GetPropertyBlock(Mpb, 0);
	    
	    Vector3 worldPos = transform.TransformPoint(new Vector3(mesh.bounds.center.x, mesh.bounds.center.y, mesh.bounds.center.z)); 
	    if (shapePadding > 0)
	    {
		    // only lerp if not paused/normal update
		    if (Time.deltaTime != 0)
		    {
			    tempPos = Vector3.Lerp(tempPos, (worldPos - new Vector3(0, GetLowestPoint(), 0)), Time.deltaTime * 10);
		    }
		    else
		    {
			    tempPos = (worldPos - new Vector3(0, GetLowestPoint(), 0));
		    }
 
		    pos = worldPos - transform.position - new Vector3(0, 1 - (fill - (tempPos.y * shapePadding)), 0);
	    }
	    else
	    {
			pos = worldPos - transform.position - new Vector3(0, 1 - fill, 0); 
	    }
	    Mpb.SetVector("_Fill", pos);
	    
	    renderer.SetPropertyBlock(Mpb, 0);
    }
    
    float GetLowestPoint()
    {
	    float lowestY = float.MaxValue;
	    Vector3 lowestVert = Vector3.zero;
	    Vector3[] vertices = mesh.vertices;
 
	    for (int i = 0; i < vertices.Length; i++)
	    {
 
		    Vector3 position = transform.TransformPoint(vertices[i]);
 
		    if (position.y < lowestY)
		    {
			    lowestY = position.y;
			    lowestVert = position;
		    }
	    }
	    return lowestVert.y;
    }
}
