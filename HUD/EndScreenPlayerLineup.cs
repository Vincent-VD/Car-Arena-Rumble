using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndScreenPlayerLineup : MonoBehaviour
{
	[SerializeField]
	private int _playerId = 0;

	[SerializeField]
	private Mesh _stickyNoteMesh = null;

	[SerializeField]
	private GameObject _stickyNoteTarget = null;

	[SerializeField]
	private GameObject _firstRow = null;

	[SerializeField]
	private GameObject _secondRow = null;

	[SerializeField]
	private Material[] _stickyNoteColors = null;

	[SerializeField]
	private GameObject _cameraFocusPoint = null;

	[SerializeField] private TextMeshPro _playerText = null;
	[SerializeField] private TextMeshPro _killScoreText = null;
	[SerializeField] private TextMeshPro _ramScoreText = null;
	[SerializeField] private TextMeshPro _maxLaunchSpeed = null;

	private PlayerConfig _config = null;

	private void Start()
	{
		if (!(_playerId >= PlayersScoreManager.Instance.PlayerScores.Length))
		{
			PlayerManager.Instance.AddFakePlayer(_cameraFocusPoint);
			CameraBehavior camera = GameObject.FindObjectOfType<CameraBehavior>();
			if (camera != null)
			{
				camera.AssignTargets(5, 40);
			}

			_config = PlayerManager.Instance.PlayerConfigs[_playerId];

			_config._input.SwitchCurrentActionMap("End Controller");

			_config._input.currentActionMap.FindAction("GoBack").performed += GoBack;
		}
	}

    private void Awake()
    {
	    if (!(_playerId >= PlayersScoreManager.Instance.PlayerScores.Length))
	    {
			//Debug.LogError("EndPlayerLineup" + _playerId);
		    PlayersScoreManager.playerScore score = PlayersScoreManager.Instance.PlayerScores[_playerId];

			//Debug.LogError(score.playerID + "  " + score.killsAmount);

		    _playerText.SetText("Player " + (_playerId + 1) + ":");

		    _killScoreText.SetText("Kills:\t\t    " + score.killsAmount);

		    _ramScoreText.SetText("Ram Score:\t   " + score.rammingScore);

		    _maxLaunchSpeed.SetText("Max Launch Speed:  " + score.velocity);

		    PlayerConfig config = PlayerManager.Instance.PlayerConfigs[_playerId];
		    GameObject go = Instantiate(PlayerManager.Instance.PreviewMeshes[config._playerMeshId]);
		    Material mat = PlayerManager.Instance.PreviewMaterials[config._playerMaterialId];
		    go.transform.Rotate(new Vector3(0, 1, 0), -150);

		    _stickyNoteTarget.GetComponent<MeshFilter>().mesh = _stickyNoteMesh;
		    _stickyNoteTarget.GetComponent<MeshRenderer>().material = _stickyNoteColors[config._playerMaterialId];

			PlayerManager.AssignPlayerMaterial(go, mat, config._playerIdx, config._playerMeshId);

		    Destroy(go.GetComponent<CarAnimationBehavior>());
		    Destroy(go.GetComponent<CarModPoints>());

		    if (score.hasMostKills)
		    {
			    go.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
			    go.transform.position = _firstRow.transform.position;
			    go.transform.SetParent(_firstRow.transform);
		    }
		    else
		    {
			    go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
				go.transform.position = _secondRow.transform.position;
			    go.transform.SetParent(_secondRow.transform);
		    }
	    }
    }

    public void GoBack(InputAction.CallbackContext callbackContext)
    {
	    ResetGame resetGame = GameObject.Find("ResetGame").GetComponent<ResetGame>();
	    if (resetGame != null)
	    {
			resetGame.Reset();
	    }
    }

    public void ClearControlBindings()
    {
	    if (_config != null)
	    {
		    _config._input.currentActionMap.FindAction("GoBack").performed -= GoBack;
	    }
	}

}
