using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawnPointManager : MonoBehaviour
{
	private CarModManager _carModManager = null;

	private ItemSpawnPoint[] _itemSpawnPoints = null;
	private float _currCooldownTime = 0f;
	private float _maxCooldownTime = 3f;

    #region SINGLETON
    private static ItemSpawnPointManager _instance;
    public static ItemSpawnPointManager Instance
    {
        get
        {
            if (_instance == null && !_applicationQuiting)
            {
                //find it in case it was placed in the scene
                _instance = FindObjectOfType<ItemSpawnPointManager>();
                if (_instance == null)
                {
                    //none was found in the scene, create a new instance
                    GameObject newObject = new GameObject("Singleton_ItemSpawnPointManager");
                    _instance = newObject.AddComponent<ItemSpawnPointManager>();
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
        //we want this object to persist when a scene changes
        DontDestroyOnLoad(gameObject);
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion


    private void Start()
	{
		_itemSpawnPoints = FindObjectsOfType<ItemSpawnPoint>();
		_carModManager = FindObjectOfType<CarModManager>();
	}

	private void Update()
	{
		_currCooldownTime += Time.deltaTime;
		if (_currCooldownTime > _maxCooldownTime)
		{
			int randCar = Random.Range(0, _itemSpawnPoints.Length);
			_itemSpawnPoints[randCar].SpawnItem();
			_maxCooldownTime = Random.Range(3f, 5f);
			_currCooldownTime = 0f;
		}
	}

}
