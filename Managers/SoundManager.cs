using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
	[SerializeField] private AudioClip[] _batteringRamAudioClips = null;
	[SerializeField] private AudioClip[] _carCollisionAudioClips = null;
	[SerializeField] private AudioClip _bouncySpringAudioClip = null;
	[SerializeField] private AudioClip _carKillAudioClip = null; //Scream: "Bloody murder!"
	[SerializeField] private AudioClip _itemPickupAudioClip = null;
	[SerializeField] private AudioClip _mineDropAudioClip = null;
	[SerializeField] private AudioClip _mineExplosionAudioClip = null;
	[SerializeField] private AudioClip _nitroAudioClip = null;

	[SerializeField]
	private AudioSource _mainSource = null;

	[SerializeField]
	private AudioSource _inGameSource = null;

	[SerializeField]
	private AudioSource _secondarySource = null;

	#region SINGLETON
	private static SoundManager _instance;
	public static SoundManager Instance
	{
		get
		{
			if (_instance == null && !_applicationQuiting)
			{
				//find it in case it was placed in the scene
				_instance = FindObjectOfType<SoundManager>();
				if (_instance == null)
				{
					//none was found in the scene, create a new instance
					GameObject newObject = new GameObject("Singleton_CarModManager");
					_instance = newObject.AddComponent<SoundManager>();
				}
			}
			return _instance;
		}
	}

	private static bool _applicationQuiting = false;
	public void OnApplicationQuit()
	{
		_applicationQuiting = true;
	}

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this)
		{
			Destroy(this);
		}
		DontDestroyOnLoad(this);
	}
	#endregion

	private void Start()
	{
		AudioSource audioSource = GetComponent<AudioSource>();

		audioSource.Play();
	}

	public void SwitchToInGameTrack()
	{
		StartCoroutine(FadeMixerGroup.FadeOut(_mainSource, 2.5f));
		StartCoroutine(FadeMixerGroup.FadeIn(_inGameSource, 1.5f));
		Invoke("StopAllCoroutines", 5f);
	}

	public void SwitchToEndGameTrack()
	{
		StartCoroutine(FadeMixerGroup.FadeOut(_inGameSource, 2.5f));
		StartCoroutine(FadeMixerGroup.FadeIn(_mainSource, 1.5f));
		Invoke("StopAllCoroutines", 5f);
	}

	public void PlayBatteringRamSound()
	{
		int rand = Random.Range(0, _batteringRamAudioClips.Length);
		_secondarySource.clip = _batteringRamAudioClips[rand];
		_secondarySource.Play();
	}

	public void PlayerCarCollisionSound()
	{
		int rand = Random.Range(0, _carCollisionAudioClips.Length);
		_secondarySource.clip = _carCollisionAudioClips[rand];
		_secondarySource.Play();
	}

	public void PlayBouncySpringSound()
	{
		_secondarySource.clip = _bouncySpringAudioClip;
		_secondarySource.Play();
	}

	public void PlayerCarKillSound()
	{
		_secondarySource.clip = _carKillAudioClip;
		_secondarySource.Play();
	}

	public void PlayItemPickUpSound()
	{
		_secondarySource.clip = _itemPickupAudioClip;
		_secondarySource.Play();
	}

	public void PlayMineDropSound()
	{
		_secondarySource.clip = _mineDropAudioClip;
		_secondarySource.Play();
	}

	public void PlayMineExplosionSound()
	{
		_secondarySource.clip = _mineExplosionAudioClip;
		_secondarySource.Play();
	}

	public void PlayNitroSound()
	{
		_secondarySource.clip = _nitroAudioClip;
		_secondarySource.Play();
	}



}
