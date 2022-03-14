using System;

using UnityEngine;
using UnityEngine.UI;

namespace TerrainGeneration.Rendering.UI
{
	public class LoadingScreen : MonoBehaviour
	{
		[SerializeField] private Slider slider;
		[SerializeField] private Image image;
		[SerializeField] private Image progressBar;

		private float progress;

		private void Update()
		{
			image.color = Color.Lerp(Color.magenta, Color.yellow, slider.value / slider.maxValue);
			progressBar.fillAmount = Mathf.Clamp01(progress / slider.maxValue);
		}

		public void Prepare(int _max)
		{
			slider.minValue = 0;
			slider.maxValue = _max;
			slider.value = _max;
			progress = _max;
			
			Show();
		}
		
		public void Progress()
		{
			slider.value -= 1;
			progress -= 1f;
		}

		public void Show() => gameObject.SetActive(true);

		public void Hide() => gameObject.SetActive(false);
	}
}