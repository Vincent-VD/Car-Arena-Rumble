using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField]
    private PlayerRespawnPoint[] _spawnPoints = null;

    #region SINGLETON
    private static SpawnPointManager _instance;
    public static SpawnPointManager Instance
    {
        get
        {
            if (_instance == null && !_applicationQuiting)
            {
                //find it in case it was placed in the scene
                _instance = FindObjectOfType<SpawnPointManager>();
                if (_instance == null)
                {
                    //none was found in the scene, create a new instance
                    GameObject newObject = new GameObject("Singleton_SpawnPointManager");
                    _instance = newObject.AddComponent<SpawnPointManager>();
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
        //DontDestroyOnLoad(gameObject);
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

    //Gets the spawnpoints
    public PlayerRespawnPoint[] SpawnPoints
    {
        get
        {
            return _spawnPoints;
        }

    }
    //respawns the player
    public Quaternion RespawnPlayer(GameObject player)
    {
        Transform bestTransform = GetBestAvailablePoint();
        player.transform.position = bestTransform.position;
        player.transform.rotation = bestTransform.rotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        return bestTransform.rotation;
    }
    //gets the closest Available Transform
    private Transform GetBestAvailablePoint()
    {
        int index = 0;
        Vector3 playersAvPos = PlayerManager.Instance.GetAveragePlayerPos();
        Transform[] transforms = getAvailableTransforms();
        for (int i = 0; i < transforms.Length; i++)
        {
            //checks who is closest with distance
            if (Vector3.Distance(transforms[i].position, playersAvPos) <
                Vector3.Distance(transforms[index].position, playersAvPos))
            {
                index = i;
            }
        }
        return transforms[index].transform;
    }

    //Gets the Available Transforms of the spawnpoints
    private Transform[] getAvailableTransforms()
    {
        //looks how many there are
        int counter = 0;
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (_spawnPoints[i].IsAvailable)
            {
                counter++;
            }
        }
        Transform[] tmpArray = new Transform[counter];
        counter = 0;
        //adds the Available ones
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            if (_spawnPoints[i].IsAvailable)
            {
                tmpArray[counter] = _spawnPoints[i].transform;
                counter++;

            }
        }
        return tmpArray;
    }

}
