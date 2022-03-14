using UnityEngine;

namespace TerrainGeneration.Rendering.Data
{
	public struct Vertex
	{
		public static int Size => sizeof(float) * 10;
		
		public Vector3 position;
		public Vector3 normal;
		public Vector4 color;
	}
}