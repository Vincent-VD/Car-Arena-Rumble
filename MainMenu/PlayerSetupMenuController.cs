using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
	private int _playerIdx;

	struct MeshColorId
	{
		private int _mesh;
		public int Mesh { get; set; }
		private int _color;
		public int Color { get; set; }
	}

	[SerializeField]
	private GameObject _inactive = null;
	
	[SerializeField]
	private GameObject _active = null;

	[SerializeField]
	private TextMeshProUGUI _titleText = null;

	[SerializeField]
	private GameObject _readyPanel = null;

	[SerializeField]
	private GameObject _menuPanel = null;

	[SerializeField]
	private GameObject _readyButton = null;

	[SerializeField]
	private GameObject _pressXToConfirm = null;

	[SerializeField]
	private Sprite[] _playerConfigImages = null;

	private List<List<Sprite>> _playerConfigInner = new List<List<Sprite>>();

	[SerializeField]
	private Vector2 _meshAndColorAmount;

	private MeshColorId[] _playerConfigInts = new MeshColorId[4];

	[SerializeField]
	private Image _previewImage;

	private float _ignoreInputTime = 1.0f;
	public bool _inputEnabled = false;

	private void Start()
	{
		List<List<Sprite>> res = new List<List<Sprite>>();
		for (int outer = 0; outer <  _meshAndColorAmount.x; ++outer)
		{
			List<Sprite> list = new List<Sprite>();
			for (int inner = 0; inner < _meshAndColorAmount.y; ++inner)
			{
				list.Add(_playerConfigImages[(outer * (int)_meshAndColorAmount.y) + inner]);
			}
			res.Add(list);
		}

		_playerConfigInner = res;

		_previewImage.sprite = _playerConfigInner[0][0];

	}

	public void Activate()
	{
		_inactive.SetActive(false);
		_active.SetActive(true);
		Animator animator = GetComponent<Animator>();
		if (animator != null)
		{
			animator.SetTrigger("TrActivate");
		}
	}

	public void SetPlayerIndex(int idx)
	{
		_playerIdx = idx;
		_titleText.SetText("Player " + (idx + 1));
		Invoke(nameof(ResetIgnoreInput), _ignoreInputTime); //On start, put input on sleep timer (input gets ignored)
	}

	//Sets color to new material (got from PlayerConfigManager.ChangeColor)
	public void SetColor(NextMat nMat)
	{
		if (!_inputEnabled)
		{
			return;
		}

		_playerConfigInts[_playerIdx].Color = nMat.newId;

		_previewImage.sprite = _playerConfigInner[_playerConfigInts[_playerIdx].Mesh][nMat.newId];

		PlayerConfigManager configManager = FindObjectOfType<PlayerConfigManager>();
		if (configManager == null) return;

		int playerMeshId = configManager.GetPlayerMesh(_playerIdx);
		Debug.Log(playerMeshId);
		//PlayerManager.AssignPlayerMaterial(_previewGameObject.transform.GetChild(0).gameObject, nMat.mat, _playerIdx, playerMeshId);
	}

	//Sets color to new mesh (got from PlayerConfigManager.ChangeMesh)
	public void SetMesh(NextGOMat nMesh)
	{
		if (!_inputEnabled)
		{
			return;
		}

		_playerConfigInts[_playerIdx].Mesh = nMesh.newMeshId;
		_playerConfigInts[_playerIdx].Color = nMesh.newMatId;

		_previewImage.sprite = _playerConfigInner[nMesh.newMeshId][nMesh.newMatId];
	}

	public bool ReadyPlayer()
	{
		if (!_inputEnabled)
		{
			return false;
		}

		if (!PlayerManager.Instance.ChosenColors[_playerConfigInts[_playerIdx].Color])
		{
			PlayerManager.Instance.PlayerConfigs[_playerIdx]._playerSprite = _playerConfigInner[_playerConfigInts[_playerIdx].Mesh][_playerConfigInts[_playerIdx].Color];
			PlayerManager.Instance.ChosenColors[_playerConfigInts[_playerIdx].Color] = true;
			_pressXToConfirm.SetActive(false);
			_readyButton.SetActive(true);
			_inputEnabled = false;
			return true;
		}
		return false;
	}

	private void ResetIgnoreInput()
	{
		_inputEnabled = true;
	}

}
