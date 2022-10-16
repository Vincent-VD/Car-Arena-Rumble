using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    //This is the gamemode where the rules and aka game loop will be made/defined
    //Because there is only 1 game mode, this will make it easy now

    [SerializeField] 
    private int _winningScore = 10;
    [SerializeField]
    private string _endLevelName = "SC_EndScreen02";

    private PlayersScoreManager _playersScoreManager = null;
    private bool _hasCalledLoad = false;
    private const float _waitDuration = 5f;

    //Here we should call the Playermanager to initiialze the cars.
    //Because the playermanager manages the player aka cars
    void Start()
    {
        PlayerManager.Instance.InitializePlayers();
        _playersScoreManager = PlayersScoreManager.Instance;
        _playersScoreManager.InitializePlayerScores();
        //setting the winning score
        _playersScoreManager.WinningScore = _winningScore;
    }


    void Update()
    {
        //checks if the end has been achieved
        if (_playersScoreManager.EndAchieved && !_hasCalledLoad)
        {
            //game should end then
            _hasCalledLoad = true;
            PlayerManager.Instance.FreezePlayers();
            Invoke("LoadEndScene", _waitDuration);
        }
    }

    private void LoadEndScene()
    {
	    BasicCarCharachter[] carCharachters = GameObject.FindObjectsOfType<BasicCarCharachter>();
	    foreach (var car in carCharachters)
	    {
		    car.ClearControlBindings();
	    }
	    Destroy(SpawnPointManager.Instance.gameObject);
	    Destroy(CarModManager.Instance.gameObject);
	    Destroy(ItemSpawnPointManager.Instance.gameObject);
        SoundManager.Instance.SwitchToEndGameTrack();
        PlayerManager.Instance.Players.Clear();
        SceneManager.LoadScene(_endLevelName);
    }
}
