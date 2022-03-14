using System;

using TerrainGeneration.Rendering.Data;

using UnityEngine;

namespace TerrainGeneration.Generation
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class Chunk : MonoBehaviour
	{
		private const string KERNEL_ID = "mesh_generation";
		private const string VERT_ID = "verts";
		private const string TRIS_ID = "tris";
		private const string X_OFFSET_ID = "x_offset";
		private const string Y_OFFSET_ID = "y_offset";
		
		public Vector2Int Coord { get; private set; }
		
		private ComputeBuffer vertBuffer;
		private ComputeBuffer triBuffer;

		private Vertex[] verts;
		private int[] tris;
		private Mesh mesh;

		private void OnDestroy()
		{
			vertBuffer?.Dispose();
			triBuffer?.Dispose();
		}

		public void Prepare(ComputeShader _shader, Vector2Int _chunkSize, Vector2Int _coord)
		{
			Coord = _coord;
			
			int kernel = GetKernel(_shader);
			verts = new Vertex[(_chunkSize.x + 1) * (_chunkSize.y + 1)];
			tris = new int[_chunkSize.x * _chunkSize.y * 6];
			
			SetupShader(_shader, _chunkSize, kernel);
			Dispatch(_shader, kernel);
			
			SetupMesh();
			AssignMesh();
		}

		private int GetKernel(ComputeShader _shader) => _shader.FindKernel(KERNEL_ID);

		private void SetupShader(ComputeShader _shader, Vector2Int _chunkSize, int _kernel)
		{
			vertBuffer?.Dispose();
			vertBuffer = new ComputeBuffer(verts.Length, Vertex.Size);
			vertBuffer.SetData(verts);
			
			triBuffer?.Dispose();
			triBuffer = new ComputeBuffer(tris.Length, sizeof(int));
			triBuffer.SetData(tris);
			
			_shader.SetBuffer(_kernel, VERT_ID, vertBuffer);
			_shader.SetBuffer(_kernel, TRIS_ID, triBuffer);
			
			_shader.SetInt(X_OFFSET_ID, Coord.x * _chunkSize.x);
			_shader.SetInt(Y_OFFSET_ID, Coord.y * _chunkSize.y);
		}

		private void Dispatch(ComputeShader _shader, int _kernel) => _shader.Dispatch(_kernel, 1, 1, 1);

		private void SetupMesh()
		{
			vertBuffer.GetData(verts);
			triBuffer.GetData(tris);
			
			Vector3[] vertices = new Vector3[verts.Length];
			Vector3[] normals = new Vector3[verts.Length];
			Color[] colors = new Color[verts.Length];
			int[] triangles = new int[tris.Length];

			for(int i = 0; i < verts.Length; i++)
			{
				Vertex vert = verts[i];
				vertices[i] = new Vector3(vert.position.x, vert.position.y, vert.position.z);
				normals[i] = new Vector3(vert.normal.x, vert.normal.y, vert.normal.z);
				colors[i] = new Color(vert.color.x, vert.color.y, vert.color.z, vert.color.w);
			}

			for(int i = 0; i < tris.Length; i++)
				triangles[i] = tris[i];

			mesh = new Mesh
			{
				name = $"Chunk {Coord}",
				vertices = vertices,
				normals = normals,
				colors = colors,
				triangles = triangles
			};
		}

		private void AssignMesh()
		{
			// ReSharper disable once LocalVariableHidesMember
			MeshCollider collider = gameObject.GetComponent<MeshCollider>();
			MeshFilter filter = gameObject.GetComponent<MeshFilter>();

			collider.sharedMesh = mesh;
			filter.mesh = mesh;
		}
	}
}