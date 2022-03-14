using System;

using UnityEngine;

namespace ComputeShaders
{
	[Obsolete("Initial test script, use WorldGenerator now.")]
	public class MeshNoise : MonoBehaviour
	{
		public struct Vertex
		{
			public Vector3 position;
			public Vector3 normal;

			public Vertex(Vector3 _position, Vector3 _normal)
			{
				position = _position;
				normal = _normal;
			}
		}

		public ComputeShader shader;
		public int xSize = 128, ySize = 128;
		public float noiseScale = 10f;
		private ComputeBuffer vertexBuffer;
		private ComputeBuffer triangleBuffer;

		private Vertex[] vertices;
		private int[] triangles;

		private void Start()
		{
			vertices = new Vertex[(xSize + 1) * (ySize + 1)];
			triangles = new int[xSize * ySize * 6];

			vertexBuffer?.Dispose();
			triangleBuffer?.Dispose();
			vertexBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 6);
			vertexBuffer.SetData(vertices);
			triangleBuffer = new ComputeBuffer(triangles.Length, sizeof(int));
			triangleBuffer.SetData(triangles);

			int kernel = shader.FindKernel("mesh_generation");
			shader.SetInt("x_size", xSize);
			shader.SetInt("y_size", ySize);
			shader.SetFloat("noise_scale", noiseScale);
			shader.SetBuffer(kernel, "vertices", vertexBuffer);
			shader.SetBuffer(kernel, "triangles", triangleBuffer);

			shader.Dispatch(kernel, 1, 1, 1);

			vertexBuffer.GetData(vertices);
			triangleBuffer.GetData(triangles);
			Vector3[] vertPos = new Vector3[vertices.Length];
			Vector3[] normals = new Vector3[vertices.Length];

			for(int i = 0; i < vertices.Length; i++)
			{
				vertPos[i] = new Vector3(vertices[i].position.x, vertices[i].position.y, vertices[i].position.z);
				normals[i] = new Vector3(vertices[i].normal.x, vertices[i].normal.y, vertices[i].normal.z);
			}

			Mesh mesh = new Mesh
			{
				vertices = vertPos,
				normals = normals,
				triangles = triangles
			};

			gameObject.GetComponent<MeshFilter>().mesh = mesh;

			vertexBuffer.Dispose();
			triangleBuffer.Dispose();
		}
	}
}