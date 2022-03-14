using System.Collections.Generic;
using System.Threading.Tasks;

using TerrainGeneration.Rendering.UI;
using TerrainGeneration.Utilities;

using UnityEngine;

using Random = System.Random;

namespace TerrainGeneration.Generation
{
	public class World : MonoBehaviour
	{
		private const string X_SIZE_ID = "x_size";
		private const string Y_SIZE_ID = "y_size";
		private const string X_NOISE_OFFSET_ID = "x_noise_offset";
		private const string Y_NOISE_OFFSET_ID = "y_noise_offset";
		private const string VERT_DISTANCE_ID = "vert_distance";
		private const string NOISE_SCALE_ID = "noise_scale";
		private const int MAX_RAND_VALUE = 10000000;

		public Chunk chunkPrefab;
		public ComputeShader shader;
		public Vector2Int worldSize = new Vector2Int(4, 4);
		public Vector2Int chunkSize = new Vector2Int(128, 128);
		public float noiseScale = 50;
		public float vertDistance = 2;
		public string seed = "123456789";
		public LoadingScreen loadingScreen;
		private readonly Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

		private void Start() => SpawnChunks().Process();

		private async Task SpawnChunks()
		{
			loadingScreen.Prepare(worldSize.x * worldSize.y);
			
			if(string.IsNullOrEmpty(seed))
				seed = UnityEngine.Random.Range(-MAX_RAND_VALUE, MAX_RAND_VALUE).ToString();
			
			Random random = new Random(seed.GetHashCode());

			shader.SetInt(X_NOISE_OFFSET_ID, random.Next(-MAX_RAND_VALUE, MAX_RAND_VALUE));
			shader.SetInt(Y_NOISE_OFFSET_ID, random.Next(-MAX_RAND_VALUE, MAX_RAND_VALUE));
			shader.SetInt(X_SIZE_ID, chunkSize.x);
			shader.SetInt(Y_SIZE_ID, chunkSize.y);
			shader.SetFloat(NOISE_SCALE_ID, noiseScale);
			shader.SetFloat(VERT_DISTANCE_ID, vertDistance);

			for(int x = 0; x < worldSize.x; x++)
			{
				for(int z = 0; z < worldSize.y; z++)
				{
					Chunk newChunk = Instantiate(chunkPrefab, transform);
					newChunk.transform.position = new Vector3(x * chunkSize.x * vertDistance, 0, z * chunkSize.y * vertDistance);

					Vector2Int coord = new Vector2Int(x, z);
					newChunk.name = $"Chunk {coord}";
					newChunk.Prepare(shader, chunkSize, coord);
					chunks.Add(coord, newChunk);

					await Task.Delay(5);
					
					loadingScreen.Progress();
				}
			}

			await Task.Delay(5);
			
			loadingScreen.Hide();
		}
	}
}