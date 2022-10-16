using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarModManager : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _carModGameObjects = null;

	[SerializeField]
	private Sprite[] _carModSprites = null;

	#region SINGLETON
	private static CarModManager _instance;
	public static CarModManager Instance
	{
		get
		{
			if (_instance == null && !_applicationQuiting)
			{
				//find it in case it was placed in the scene
				_instance = FindObjectOfType<CarModManager>();
				if (_instance == null)
				{
					//none was found in the scene, create a new instance
					GameObject newObject = new GameObject("Singleton_CarModManager");
					_instance = newObject.AddComponent<CarModManager>();
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
	
	public CarModExport GenerateCarModExport()
	{
		//Generates random number based on the amount of CarModTypes available
		int rand = Random.Range(0, Enum.GetValues(typeof(CarModType)).Cast<int>().Max() + 1);
		Mesh mf = null;
		Sprite sprite = _carModSprites[rand];
		return new CarModExport(sprite, (CarModType)rand); //struct containing a Sprite and CarModType
	}

	public GameObject GetCarModGameObject(CarModType modType)
	{
		return Instantiate(_carModGameObjects[(int)modType]) as GameObject; ;
	}

}
