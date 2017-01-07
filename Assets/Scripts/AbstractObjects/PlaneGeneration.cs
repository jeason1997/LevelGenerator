﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[RequireComponent (typeof(AbstractBounds))]
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[DisallowMultipleComponent]
public class PlaneGeneration : MeshProperty, ITransformable{

	private Vector3 roomBounds;
	[Range(1f, 5f)]
	public float textureRepeat = 1f;
	public Material material;
	private AbstractBounds abstractBounds;

	// Use this for initialization
	void Awake () {

	}

	public void GenerateMesh(){
		MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
		GenerateCube ();
		meshRenderer.material = this.material;
	}

	private void GenerateCube(){
		MeshFilter filter = gameObject.GetComponent< MeshFilter >();
		Mesh mesh;

		#if UNITY_EDITOR
		Mesh meshCopy = Mesh.Instantiate(filter.sharedMesh);
		mesh = filter.sharedMesh = meshCopy;
		#else
		if (filter.mesh == null) {
		filter.mesh = new Mesh ();
		}

		mesh = filter.mesh;
		#endif

		mesh.name = "Generated Mesh";
		mesh.Clear();

		#region Vertices
		Vector3 p0 = new Vector3( -roomBounds.x * .5f,	0, roomBounds.z * .5f );
		Vector3 p1 = new Vector3( roomBounds.x * .5f, 	0, roomBounds.z * .5f );
		Vector3 p2 = new Vector3( roomBounds.x * .5f, 	0, -roomBounds.z * .5f );
		Vector3 p3 = new Vector3( -roomBounds.x * .5f,	0, -roomBounds.z * .5f );	

		Vector3[] vertices = new Vector3[]
		{
			// Bottom
			p0, p1, p2, p3,
		};
		#endregion

		#region Normales
		Vector3 down 	= Vector3.down;

		Vector3[] normals = new Vector3[]
		{
			// Bottom
			down, down, down, down,
		};
		#endregion	

		float wUV = roomBounds.x / textureRepeat;
		float hUV = roomBounds.z / textureRepeat;

		#region UVs
		Vector2 _00 = new Vector2( 0f, 0f );
		Vector2 _10 = new Vector2( wUV, 0f );
		Vector2 _01 = new Vector2( 0f, hUV );
		Vector2 _11 = new Vector2( wUV, hUV );

		Vector2[] uvs = new Vector2[]
		{
			// Bottom
			_11, _01, _00, _10,
		};
		#endregion

		#region Triangles
		int[] triangles = new int[] {3, 0, 1, 3, 1, 2};
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.subMeshCount = 6;

		mesh.triangles = triangles;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals ();
		mesh.Optimize();
	}

	public override void Preview(){
		if (abstractBounds == null) {
			abstractBounds = GetComponent<AbstractBounds> ();
		}
		this.roomBounds = abstractBounds.Bounds;
		GenerateMesh ();
	}

	public override void Generate(){
		if (abstractBounds == null) {
			abstractBounds = GetComponent<AbstractBounds> ();
		}
		this.roomBounds = abstractBounds.Bounds;
		GenerateMesh ();
	}

	public void NotifyBoundsChanged(AbstractBounds newBounds){
		Preview ();
		SceneUpdater.UpdateScene ();
	}
}
