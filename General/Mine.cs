using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
	[SerializeField]
	private float m_explosionForce = 1f;

	[SerializeField]
	private ParticleSystem[] _mineParticleSystems = null;

	[SerializeField]
	private float _timeToBeActive = 2f;

	[SerializeField]
	private bool _dieAfterExplosion = true;

	[SerializeField]
	private Renderer _mineRenerer = null;

    [SerializeField]
	private Material _offMaterial = null;

	[SerializeField]
	private ParticleSystem _mineActivePartciles = null;

	private Material _defaultMaterial = null;

	private int _playerId = 0;

	private const string PLAYER_TAG = "Player";
	private const float _forceMultiplier = 1000000f;
	private bool _hasExploded = false;
	private float _counter = 0f;
	private float _secondsUntilKilled = 0.15f;

	//This part sets the time when the mine becomes active
	public float TimeTobeActive
	{
		get => _timeToBeActive;
		set => _timeToBeActive = value;
	}

	public int PlayerID
	{
		get => _playerId;
		set => _playerId = value;
	}
	public bool DieAfterExplosion
	{
		get => _dieAfterExplosion;
		set => _dieAfterExplosion = value;
	}
	public bool HasExploded
	{
		get => _hasExploded;
		set => _hasExploded = value;
	}

    private void Start()
    {
		_defaultMaterial = _mineRenerer.material;
		_mineRenerer.material = _offMaterial;
	}
    private void Update()
	{
		if (_counter >= _timeToBeActive && _mineRenerer.material.color == _defaultMaterial.color)
		{
			return;
		} 
		else if (_counter >= _timeToBeActive && _mineRenerer.material.color == _offMaterial.color)
		{
			_mineActivePartciles.Play();
			_mineRenerer.material = _defaultMaterial;
			return;
		}



            _counter += Time.deltaTime;
        
	}
	//When in trigger and its a player, it explodes, emmits the particles and destroys it self in x seconds
	private void OnTriggerEnter(Collider other)
	{
		Transform otherParent = other.transform.parent;
		if (otherParent == null)
			return;

		if (otherParent.CompareTag(PLAYER_TAG) && !_hasExploded && _counter >= _timeToBeActive)
		{
			SoundManager.Instance.PlayMineExplosionSound();
			//means that it has really will explode
			_hasExploded = true;
			//plays the partice
			PlayParticles();
			//adds the impact multiplier, so that the explosion force increases with the impact multiplier
			ImpactMultiplier impMult = otherParent.GetComponent<ImpactMultiplier>();
			Vector3 direction = (otherParent.position- transform.position);
			direction = direction.normalized;
			//to prevent launching upwards
			direction.y = 0.1f;
			otherParent.GetComponent<Rigidbody>().AddForce(direction * (_forceMultiplier * m_explosionForce * impMult.ImpactMultiplierVal));
			//Checks if the collider with the mine is not the one that set the mines
			if (otherParent.gameObject.GetInstanceID() != _playerId)
			{
				///If they are not, then set the enemy id, so that it can count as a kill
				otherParent.GetComponent<BasicCarCharachter>().SetEnemyId(_playerId);
			}
			impMult.IncreaseMultplier();
			if (_dieAfterExplosion)
			{
				//This game objects dies in x seconds
				Invoke("Kill", _secondsUntilKilled);
			}
			else
            {//resets time, to not spam explosion
				_counter = 0f;
				_mineActivePartciles.Stop();
			}
		  
		}
	}

	//PLays all the particles, once, they should not be looped
	private void PlayParticles()
	{
		foreach (var VARIABLE in _mineParticleSystems)
		{
			if (!VARIABLE.isPlaying)
			{
				VARIABLE.Play();
			}
		}
	}

	//Destroys objects
	private void Kill()
	{
		Destroy(gameObject);
	}
}
