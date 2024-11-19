using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RotateObject : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
	public float speed = 60f;

	[ExecuteInEditMode]
	void Update() 
	{
		transform.Rotate(axis * (Time.deltaTime * speed));
	}
}
