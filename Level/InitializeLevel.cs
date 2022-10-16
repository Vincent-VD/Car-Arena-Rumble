using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

public class InitializeLevel : MonoBehaviour
{
	[SerializeField]
	private Transform[] _playerSpawns;

	[SerializeField]
	private GameObject _carPrefab;

    // Start is called before the first frame update
    void Start()
    {
		//Iterates over every PlayerConfig, and creates player vehicles
	    var playerConfigs = PlayerManager.Instance.PlayerConfigs.ToArray();
	    for(int iter = 0; iter < playerConfigs.Length; iter++)
	    {
		    var player = Instantiate(_carPrefab, _playerSpawns[iter].position, _playerSpawns[iter].rotation);
		    PlayerManager.Instance.Players.Add(player.GetComponent<BasicCarCharachter>().InitializeCar(playerConfigs[iter]));
	    }
    }
}
