using System.Linq;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public struct NextGOMat
{
	public NextGOMat(GameObject go, int newMeshId, int newMatId)
	{
		this.go = go;
		this.newMeshId = newMeshId;
		this.newMatId = newMatId;
	}
	public GameObject go;
	public int newMeshId;
	public int newMatId;
}

public struct NextMat
{
	public NextMat(Material mat, int newId)
	{
		this.mat = mat;
		this.newId = newId;
	}
	public Material mat;
	public int newId;
}

public class PlayerConfigManager : MonoBehaviour
{
	[SerializeField]
	private int _maxPlayers = 4;

	[SerializeField] 
    private string _nextLevelName = "SC_TestScene04";

	private int[] _previewMatIdx = { 0, 0, 0, 0 };

	private int[] _previewMeshIdx = { 0, 0, 0, 0 };

	[SerializeField]
	private GameObject[] _playerSetupMenus = null;

	[SerializeField]
	private Transform _root = null;

	[SerializeField]
	private Animator _startAnimation = null;

	private void Start()
	{
		if (_startAnimation != null)
		{
			_startAnimation.SetTrigger("TrActivate");
		}
	}

	//Changes mesh to next or previous mesh, depending on 'val' (+1 for next, -1 for prev)
	//	Returns new mesh so it can be set in PlayerSetupMenuController
	public NextGOMat ChangeMesh(int index, int val)
	{
		GameObject[] meshes = PlayerManager.Instance.PreviewMeshes;

		int newVal = _previewMeshIdx[index] + val;
		if (newVal < 0)
		{
			newVal = meshes.Length - 1;
		}
		else if (newVal >= meshes.Length)
		{
			newVal = 0;
		}

		
		Debug.Log("New preview mesh idx: " + newVal);
		_previewMeshIdx[index] = newVal;
		GameObject go = meshes[_previewMeshIdx[index]];

		PlayerManager.Instance.PlayerConfigs[index]._playerMeshId = newVal;

		int colorChange = 0;

		while (PlayerManager.Instance.ChosenColors[colorChange])
		{
			++colorChange;
		}
		_previewMatIdx[index] = colorChange;
		PlayerManager.Instance.PlayerConfigs[index]._playerMaterialId = colorChange;
		NextGOMat res = new NextGOMat(go, newVal, colorChange);
		return res;
	}

	//Changes material to next or previous material, depending on 'val' (+1 for next, -1 for prev)
	//	Returns new material so it cna be set in PlayerSetupMenuController
	public NextMat ChangeColor(int index, int val)
	{
		int newVal;
		Material[] mats = PlayerManager.Instance.PreviewMaterials.ToArray();

		do
		{
			newVal = _previewMatIdx[index] + val;
			if (newVal < 0)
			{
				newVal = mats.Length - 1;
			}
			else if (newVal >= mats.Length)
			{
				newVal = 0;
			}
			_previewMatIdx[index] = newVal;
		} while (PlayerManager.Instance.ChosenColors[newVal] != false);
		
		Material mat = mats[_previewMatIdx[index]];

		PlayerManager.Instance.PlayerConfigs[index]._playerMaterialId = newVal;
		NextMat res = new NextMat(mat, newVal);
		return res;
	}

	public void ReadyPlayer(int index)
	{
		PlayerManager.Instance.PlayerConfigs[index]._isReady = true;
		Debug.Log(PlayerManager.Instance.PlayerConfigs[index]._playerMeshId);
		//If all players ready, load target scene
		if (PlayerManager.Instance.PlayerConfigs.Count >= 2 && PlayerManager.Instance.PlayerConfigs.All(p => p._isReady == true))
		{
			SceneManager.LoadScene(_nextLevelName);
        }
	}

	//Player join, called by Player Input Manger on player join
	public void HandlePlayerJoin(PlayerInput input)
	{
		Debug.Log("Player joined " + input.playerIndex);
		if (PlayerManager.Instance.PlayerConfigs.Count == 0)
		{
			PlayerManager.Instance.PlayerConfigs.Add(new PlayerConfig(input));
			SpawnPlayerSetupMenu[] playerSpawns = GameObject.FindObjectsOfType<SpawnPlayerSetupMenu>();
			playerSpawns[input.playerIndex].Activate(_playerSetupMenus[input.playerIndex], input);
			ChangeBackground updateComp = GetComponent<ChangeBackground>();
			if (updateComp != null)
			{
				updateComp.UpdateBackground();
			}
		}
		if (PlayerManager.Instance.PlayerConfigs.Any(p => p._playerIdx != input.playerIndex))
		{
			PlayerManager.Instance.PlayerConfigs.Add(new PlayerConfig(input));
			SpawnPlayerSetupMenu[] playerSpawns = GameObject.FindObjectsOfType<SpawnPlayerSetupMenu>();
			playerSpawns[^(input.playerIndex + 1)].Activate(_playerSetupMenus[input.playerIndex], input);
			ChangeBackground updateComp = GetComponent<ChangeBackground>();
			if (updateComp != null)
			{
				updateComp.UpdateBackground();
			}
		}
	}

	public int GetPlayerMesh(int playerId)
	{
		return _previewMeshIdx[playerId];
	}

	private void ActivateAll()
	{
		foreach (var go in _playerSetupMenus)
		{
			go.SetActive(true);
		}
	}
}

//Contains configuration for a player
//This gets configured into the car when game starts
public class PlayerConfig
{
	public PlayerConfig(PlayerInput input)
	{
		_input = input;
		_playerIdx = input.playerIndex;
	}

	public PlayerInput _input { get; set; }
	public int _playerIdx { get; set; }
	public bool _isReady { get; set; }
	public int _playerMeshId { get; set; }
	public int _playerMaterialId { get; set; }
	public Sprite _playerSprite { get; set; }
	public RumbleBehavior _rumbleBehavior { get; set; }
}