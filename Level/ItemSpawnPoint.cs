using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{

	[SerializeField]
	private SpriteRenderer _spriteRenderer = null;

    [SerializeField]
	private ParticleSystem _hasItemParticles = null;

	[SerializeField]
	private ParticleSystem _pickedUpItemParticles = null;

	private ItemSpawnPointManager _itemSpawnPointManager = null;
	private CarModManager _carModManager = null;

	private CarModExport _carModExport;

	private bool _hasItem = false;
	private bool _canSpawnItem = false;
	private float _currentCooldownTime = 0.0f;
	private float _maxCooldownTime = 3f;

	// Start is called before the first frame update
	void Start()
	{
		gameObject.SetActive(true);
		_itemSpawnPointManager = ItemSpawnPointManager.Instance;
		_carModManager = CarModManager.Instance;
	}

	// Update is called once per frame
	void Update()
	{
		if (!_canSpawnItem)
		{
			_currentCooldownTime += Time.deltaTime;
		}
		if (!_hasItem && _currentCooldownTime >= _maxCooldownTime)
		{
			//SpawnPointManager decides which spawn point spawns an item
			// when cooldown expires, new cooldown time is assigned
			_canSpawnItem = true;
			float randNum = Random.Range(6f, 8f);
			_maxCooldownTime = randNum;
			_currentCooldownTime = 0f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Inventory carInventory = other.GetComponentInParent<Inventory>();
			if (_hasItem && !carInventory.HasItemEquipped) //Check if spawn point has an item active, and if the player doesn't have an item equipped
			{
				SoundManager.Instance.PlayItemPickUpSound();
				_hasItem = false;
				_spriteRenderer.sprite = null;
				_currentCooldownTime = 0.0f;
				carInventory.AddItem(_carModExport.itemType);
				//Stops the particle system
				if (_hasItemParticles.isPlaying)
					_hasItemParticles.Stop();

				if (!_pickedUpItemParticles.isPlaying)
					_pickedUpItemParticles.Play();
			}
		}
	}

	public void SpawnItem()
	{
		if (_canSpawnItem)
		{
			//Plays the particle system
            if (!_hasItemParticles.isPlaying)
				_hasItemParticles.Play();
		
			//Get mesh and material from car mod prefab, and assign them to the _meshGameObject (->visuals only)
			_hasItem = true;
			CarModExport export = _carModManager.GenerateCarModExport();
			_carModExport = export;
			_spriteRenderer.sprite = export.sprite;
			_canSpawnItem = false;
		}
	}
}
