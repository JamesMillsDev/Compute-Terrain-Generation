using System.Threading.Tasks;

namespace TerrainGeneration.Utilities
{
	public static class ExtensionFunctions
	{
		public static async void Process(this Task _task) => await _task;
	}
}