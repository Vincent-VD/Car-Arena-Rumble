using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
public static class FadeMixerGroup
{
	public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
	{
		float currentTime = 0;
		float currentVol;
		audioMixer.GetFloat(exposedParam, out currentVol);
		currentVol = Mathf.Pow(10, currentVol / 20);
		float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
			audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
			yield return null;
		}
		yield break;
	}
	public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
	{
		float startVolume = audioSource.volume;

		while (audioSource.volume > 0)
		{
			audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

			yield return null;
		}

		audioSource.Stop();
		audioSource.volume = 0;
	}

	public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
	{
		float startVolume = 0.4f;

		audioSource.volume = 0;
		audioSource.Play();

		while (audioSource.volume < 1.0f)
		{
			audioSource.volume += startVolume * Time.deltaTime / fadeTime;

			yield return null;
		}

		audioSource.volume = 1f;
	}
}