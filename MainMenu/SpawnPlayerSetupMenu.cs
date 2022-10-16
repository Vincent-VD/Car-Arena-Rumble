using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject _setupMenuPrefab;

	[SerializeField]
	private PlayerInput _input;

	private PlayerControls _playerControls;

	private PlayerConfigManager _playerConfigManager;

	public void Activate(GameObject setupMenu, PlayerInput input)
	{
		_setupMenuPrefab = setupMenu;
		_input = input;
		_setupMenuPrefab.SetActive(true);
		_setupMenuPrefab.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(_input.playerIndex); //Set player index when a new player setup menu gets created
		_setupMenuPrefab.GetComponent<PlayerSetupMenuController>().Activate();
		_playerControls = new PlayerControls(); // Script containing input methods, and keybindings
	}

	private void Start()
	{
		_playerConfigManager = FindObjectOfType<PlayerConfigManager>();
		PlayerManager.Instance.PlayerConfigs[_input.playerIndex]._rumbleBehavior = GetComponent<RumbleBehavior>();
		GetComponent<RumbleBehavior>().RumbleSoft();
	}

	public void ConfirmButton(InputAction.CallbackContext context)
	{
		if (context.phase != InputActionPhase.Started) return;
		PlayerSetupMenuController setup = _setupMenuPrefab.GetComponent<PlayerSetupMenuController>();
		if (setup.ReadyPlayer())
		{
			_playerConfigManager.ReadyPlayer(_input.playerIndex);
		}
	}

	
	public void ChangeColor(InputAction.CallbackContext context)
	{
		if (context.phase != InputActionPhase.Performed) return;
		if (context.action.name == _playerControls.MenuController.ChangeColorDown.name)
		{
			PlayerSetupMenuController setup = _setupMenuPrefab.GetComponent<PlayerSetupMenuController>();
			if (!setup._inputEnabled) return;
			NextMat mat = _playerConfigManager.ChangeColor(_input.playerIndex, -1);
			setup.SetColor(mat);
		}
		if (context.action.name == _playerControls.MenuController.ChangeColorUp.name)
		{
			PlayerSetupMenuController setup = _setupMenuPrefab.GetComponent<PlayerSetupMenuController>();
			if (!setup._inputEnabled) return;
			NextMat mat = _playerConfigManager.ChangeColor(_input.playerIndex, 1);
			setup.SetColor(mat);
		}
	}

	public void ChangeMesh(InputAction.CallbackContext context)
	{
		if (context.phase != InputActionPhase.Performed) return;
		if (context.action.name == _playerControls.MenuController.ChangeCarLeft.name)
		{
			PlayerSetupMenuController setup = _setupMenuPrefab.GetComponent<PlayerSetupMenuController>();
			if (!setup._inputEnabled) return;
			NextGOMat mesh = _playerConfigManager.ChangeMesh(_input.playerIndex, -1);
			setup.SetMesh(mesh);
		}
		if (context.action.name == _playerControls.MenuController.ChangeCarRight.name)
		{
			PlayerSetupMenuController setup = _setupMenuPrefab.GetComponent<PlayerSetupMenuController>();
			if (!setup._inputEnabled) return;
			NextGOMat mesh = _playerConfigManager.ChangeMesh(_input.playerIndex, 1);
			setup.SetMesh(mesh);
		}
	}
}
