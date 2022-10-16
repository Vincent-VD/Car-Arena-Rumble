using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _carPrefab = null;

    [SerializeField]
    private GameObject[] _previewMeshes;

    public GameObject[] PreviewMeshes => _previewMeshes;

    [SerializeField]
    private List<Material> _previewMaterials;

    [SerializeField]
    public float _carRespawnTime = 1f;
    public List<Material> PreviewMaterials
    {
	    get => _previewMaterials;
	    set => _previewMaterials = value;
    }

    private List<PlayerConfig> _playersConfigs = new List<PlayerConfig>();
    public List<PlayerConfig> PlayerConfigs => _playersConfigs;

    private List<GameObject> _players = new List<GameObject>();
    public List<GameObject> Players => _players;
    private const float _offTime = 3f;
    public float PlayersOffTime => _offTime;

    private bool[] _chosenColors; //false is not chosen, true is chosen

    public CameraBehavior CameraBeh = null;
    

    public bool[] ChosenColors
    {
	    get => _chosenColors;
        set => _chosenColors = value;
    }

    #region SINGLETON
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null && !_applicationQuiting)
            {
                //find it in case it was placed in the scene
                _instance = FindObjectOfType<PlayerManager>();
                if (_instance == null)
                {
                    //none was found in the scene, create a new instance
                    GameObject newObject = new GameObject("Singleton_PlayerManager");
                    _instance = newObject.AddComponent<PlayerManager>();
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
        _chosenColors = new bool[_previewMaterials.Count];
    }
    #endregion
    
    public int[] GetCarIDs()
    {
        List<int> res = new List<int>();
        foreach (var car in _playersConfigs)
        {
            res.Add(car._playerIdx);
        }
        return res.ToArray();
    }

    //Initiaze all the players 
    public void InitializePlayers()
    {
        for (int iter = 0; iter < _playersConfigs.Count; ++iter)
        {
            //Create cars and initialize
            var player = Instantiate(_carPrefab,
                SpawnPointManager.Instance.SpawnPoints[iter].transform.position, SpawnPointManager.Instance.SpawnPoints[iter].transform.rotation);
            _players.Add(player.GetComponent<BasicCarCharachter>().InitializeCar(_playersConfigs[iter]));
            //Let them wait for x seconds before they can move
            _players[iter].GetComponent<CarMovementBehavior>().IsActive = false;
        }
        //wake them up in x seconds
        Invoke("ActivatePlayers", _offTime);
    }
    //Freezes the players from moving for x secounds
    public void FreezePlayers(float duration)
    {
        for (int iter = 0; iter < _players.Count; ++iter)
        {
            _players[iter].GetComponent<CarMovementBehavior>().IsActive = false;
        }
        Invoke("ActivatePlayers", duration);
    }
    //Freezes the player movement
    public void FreezePlayers()
    {
        for (int iter = 0; iter < _players.Count; ++iter)
        {
            _players[iter].GetComponent<CarMovementBehavior>().IsActive = false;
        }

    }
    //Unfreezes the player movements
    private void ActivatePlayers()
    {
        for (int iter = 0; iter < _players.Count; ++iter)
        {
            _players[iter].GetComponent<CarMovementBehavior>().IsActive = true;
        }
    }
    //Gets the average pos of all the players
    public Vector3 GetAveragePlayerPos()
    {
        Vector3 averagePos = Vector3.zero;
        int counter = 0;
        foreach (var VARIABLE in _players)
        {
            if (VARIABLE.GetComponent<Health>().IsAlive)
            {
                averagePos += VARIABLE.transform.position;
                counter++;
            }
        }
        averagePos /= counter;
        return averagePos;
    }

    public void UpdateWinningStatus()
    {
        var playerScores = PlayersScoreManager.Instance.PlayerScores;

        foreach (var pScore in playerScores)
        { 
            foreach (var player in _players)
            {
                if (player.GetInstanceID() == pScore.playerID)
                {
                    player.GetComponent<ShowWinningPlayer>().SetStatus(pScore.hasMostKills);
                }
            }
        }
    }

    public static void AssignPlayerMaterial(GameObject parentGameObject, Material matToAssign, int playerId, int playerMeshId)
    {
        GameObject child = parentGameObject.transform.GetChild(0).gameObject;
        for (int i = 0; i < child.transform.childCount; i++)
        {
            GameObject obj = child.transform.GetChild(i).gameObject;
            if (obj.name.Contains("Body", StringComparison.CurrentCultureIgnoreCase))
            {
                parentGameObject = obj;
                break;
            }
        }

        var mats = parentGameObject.GetComponent<MeshRenderer>().materials;
        Debug.Log(PlayerManager.Instance.PreviewMeshes[playerMeshId].name.Substring(0, 13));
        switch (PlayerManager.Instance.PreviewMeshes[playerMeshId].name.Substring(0, 13))
        {
            case "PF_VisualCar3":
                mats[0] = matToAssign;
                break;
            case "PF_VisualCar4":
                mats[1] = matToAssign;
                break;
            case "PF_VisualCar1":
                mats[0] = matToAssign;
                break;
            case "PF_VisualCar2":
	            mats[1] = matToAssign;
                break;
	    }

        parentGameObject.GetComponent<MeshRenderer>().materials = mats;
    }

    public void AddFakePlayer(GameObject fakePlayer)
    {
        _players.Add(fakePlayer);
    }

 
    public void RemoveMaterialAtIndex(int matIdx)
    {
	   _previewMaterials.RemoveAt(matIdx);
    }

    public void DeletePlayerConfigs()
    {
        _playersConfigs.Clear();
    }

}
