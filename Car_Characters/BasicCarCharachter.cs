using UnityEngine;
using UnityEngine.InputSystem;


public class BasicCarCharachter : MonoBehaviour
{
	private PlayerConfig _playerConfig;
	private CarMovementBehavior _carMovementBehavior = null;
	private CarPhysicsBehavior _carPhysicsBehavior = null;
	private ImpactMultiplier _impactMultiplier = null;
	private RumbleBehavior _rumbleBehavior = null;
	private Inventory _inventory = null;
	private LaunchAttack _launchAttack = null;
	private Health _health = null;
	private const string PLAYER_TAG = "Player";
	private const string PROP_TAG = "Prop";
	private const string WALL_LAYER = "Wall";
	private const string CAR_MOD = "CarMod";
	private Rigidbody _rigidbodySphere = null;
	private bool _hasItemEquipped = false;
	public bool HasItemEquipped() => _hasItemEquipped;
	private CarModType _currType = CarModType.None;
	private PlayerManager _playerManager = null;
	private CarModManager _carModManager = null;
	private SpawnPointManager _spawnPointManager = null;
	private int _currModID = 0;


	private Transform[] _carModPoints = null; //front, top, rear

	[SerializeField]
	private GameObject _carVisual;

	[SerializeField]
	private ParticleSystem[] _wallHitParticleSystems;

	//private PlayerControls _playerControls; // Script containing input methods, and keybindings

	private int _playerID;
	public int PlayerID =>_playerID;

	private Gamepad _gamepad;

	private int _previousEnemyID = 0;
	private const float _enemyIDDuration = 5f;
	private float _enemyIDCounter = 0f;




	private bool _isStayingOnWall = false;
	//Previously Start function, initializes class variables
	void InitializePlayer()
	{
		//initliazing variables and seeing if they don't return null
		Rigidbody tmpRigid = GetComponent<Rigidbody>();
		if (tmpRigid) 
			_rigidbodySphere = tmpRigid;

		Health tmpHealth = GetComponent<Health>();
		if (tmpHealth)
			_health = tmpHealth;

		CarPhysicsBehavior tmpPhB = GetComponent<CarPhysicsBehavior>();
		if (tmpPhB) 
			_carPhysicsBehavior = tmpPhB;
		
		ImpactMultiplier tmpImp = GetComponent<ImpactMultiplier>();
		if (tmpImp)
			_impactMultiplier = tmpImp;
		
		CarMovementBehavior tmpMov = GetComponent<CarMovementBehavior>();
		if (tmpMov)
			_carMovementBehavior = tmpMov;
		
		Inventory tmpInv = GetComponent<Inventory>();
		if (tmpInv)
			_inventory = tmpInv;

		LaunchAttack tmpLaunch = GetComponent<LaunchAttack>();
		if (tmpLaunch)
		{
			_launchAttack = tmpLaunch;
		}

		_playerManager = PlayerManager.Instance;
		_carModManager = CarModManager.Instance;
		_spawnPointManager = SpawnPointManager.Instance;

		_playerID = _playerConfig._playerIdx;
		_rumbleBehavior = _playerConfig._rumbleBehavior;

		//Binding controls to incoming PlayerInput instance, provided as argument by InitializeCar in PlayerConfig
		_playerConfig._input.actionEvents[1].AddListener(_carMovementBehavior.DriveForward);
		_playerConfig._input.actionEvents[2].AddListener(_carMovementBehavior.DriveBackward);
		_playerConfig._input.currentActionMap.FindAction("SteeringMovement").performed += _carMovementBehavior.Steer;
		_playerConfig._input.currentActionMap.FindAction("SteeringMovement").performed += _carMovementBehavior.Steer;
		_playerConfig._input.currentActionMap.FindAction("UseAbility").performed += _inventory.UseAbility;
		_playerConfig._input.currentActionMap.FindAction("LaunchAttack").performed += _launchAttack.AttackLaunch;

		_health.ResetInvincible();
	}

	public void ClearControlBindings()
	{
		_playerConfig._input.currentActionMap.FindAction("SteeringMovement").performed -= _carMovementBehavior.Steer;
		_playerConfig._input.currentActionMap.FindAction("SteeringMovement").performed -= _carMovementBehavior.Steer;
		_playerConfig._input.currentActionMap.FindAction("UseAbility").performed -= _inventory.UseAbility;
		_playerConfig._input.currentActionMap.FindAction("LaunchAttack").performed -= _launchAttack.AttackLaunch;
	}

	//Assigns config to player, sets mesh and material selected in setup screen
	public GameObject InitializeCar(PlayerConfig config)
	{
		_playerConfig = config;
		config._input.SwitchCurrentActionMap("Player Controller"); //change action map from MenuController to PlayerController

		GameObject go = Instantiate(PlayerManager.Instance.PreviewMeshes[config._playerMeshId]);
		_carModPoints = go.GetComponent<CarModPoints>().ModPoints;
		//this ads the movement behavior to the animation behavior that is in the car prefab that is just created.
		go.GetComponent<CarAnimationBehavior>().SetCarMovementBehavior(GetComponent<CarMovementBehavior>());
		go.GetComponent<CarAnimationBehavior>().SetHealth(GetComponent<Health>());
		Material mat = PlayerManager.Instance.PreviewMaterials[config._playerMaterialId];
		go.transform.SetParent(_carVisual.transform);
		go.transform.rotation = new Quaternion();
		go.transform.localScale *= 1.5f;
		go.transform.localPosition = Vector3.zero;
		//_carVisual = go;

		PlayerManager.AssignPlayerMaterial(go, mat, config._playerIdx, config._playerMeshId);

		InitializePlayer();
		return this.gameObject;
	}

	void Update()
	{
		if (_enemyIDCounter >= _enemyIDDuration)
		{
			_previousEnemyID = 0;
		}
		_enemyIDCounter += Time.deltaTime;

	}

	public void RestetIn(float sec)
	{
		Invoke("OnResetRumble", sec - 0.3f);
		Invoke("Reset", sec);
    }

	private void OnResetRumble()
	{
		_rumbleBehavior.RumbleIntense();
	}

	//Resets car and respawns at location
	public void Reset()
	{
		//checks if the enemy id is not on default
		if (_previousEnemyID != 0)
		{
			//inceases score of enemy who killed you
			PlayersScoreManager.Instance.IncreaseKillAmnount(_previousEnemyID);
			_previousEnemyID = 0;
		}
		//The car will be respawned and be removed of its car mods
		//It also becomes invincible against car and car mods for x amount of time
		Quaternion align = SpawnPointManager.Instance.RespawnPlayer(gameObject);
		_carMovementBehavior.AlignForwardBody(align);
		_health.IsInvincible = true;
		_health.IsAlive = true;
		_inventory.Discard();
		_impactMultiplier.ResetMultiplier();
		_health.ResetInvincible();

	}

	//This wakesup the rigidbody


	private void OnCollisionEnter(Collision collision)
	{
		//looks if there is a Physics behaviour
		if (_carPhysicsBehavior == null)
			return;
		//look if its trigger is due to a player 
		// And if this car is not invincible 
		if (collision.body.CompareTag(PLAYER_TAG) && !_health.IsInvincible)
		{
			//car on car collision
			_carPhysicsBehavior.CarCollisionOnHit(collision, _impactMultiplier.ImpactMultiplierVal);
			//increasing the impact multiplier
			if (_carPhysicsBehavior.IsRammed)
			{
				SoundManager.Instance.PlayerCarCollisionSound();
				_impactMultiplier.IncreaseMultplier();
				//Gets the enemy id who rammed you and give the info to the correct places
				SetEnemyId(collision.transform.gameObject.GetInstanceID());
				//Increases the enemy ramming score
				PlayersScoreManager.Instance.RammingScoreBonus(_previousEnemyID);
			}
			_rumbleBehavior.RumblePulse();
		}
		//look if its trigger is due to a wall/prop
		else if (collision.body.CompareTag(PROP_TAG) && !_isStayingOnWall)
		{
			SoundManager.Instance.PlayerCarCollisionSound(); //CHANGE TO MORE FITTING SOUND
			//car on wall collision
			_carPhysicsBehavior.WallCollisionOnHit(collision, _impactMultiplier.ImpactMultiplierVal, _carMovementBehavior.ForwardAcceleration);
		
		}
		//look if its trigger is due to a car mod 
		// And if this car is not invincible 
		else if (collision.body.tag == CAR_MOD  && !_health.IsInvincible)
		{
			var otherParent = collision.body.gameObject.GetComponent<BaseItem>().Parent;
			if (otherParent.PlayerID == PlayerID)
				return;
								
			//Car on carmod collision from another car
			_carPhysicsBehavior.CarModCollisionOnHit(collision, _impactMultiplier.ImpactMultiplierVal);
			if (_carPhysicsBehavior.IsRammed)
			{
				SoundManager.Instance.PlayBatteringRamSound();

				_impactMultiplier.IncreaseMultplier();

				//Gets the enemy id who rammed you and give the info to the correct places
				SetEnemyId(otherParent.gameObject.GetInstanceID());
				//Increases the enemy ramming score
				PlayersScoreManager.Instance.RammingScoreBonus(_previousEnemyID);
			}

			_rumbleBehavior.RumblePulse();
		}
	}

	//This will be used for when a car is constantly on the wall
	void OnCollisionStay(Collision collision)
	{
		if (!collision.body.CompareTag(PROP_TAG))
			return;
		if (_carMovementBehavior == null)
			return;

		ShowParticleOnHit(collision);
		_isStayingOnWall = true;
		_carMovementBehavior.SlowOnWall();
	}

	void OnCollisionExit(Collision collision)
	{
		if (!collision.body.CompareTag(PROP_TAG))
			return;

		_isStayingOnWall = false;
	}

	//Attaches car mod GO to correct socket
	//  Assures correct translation, rotation, scale
	public void AttachMod(GameObject carMod, CarModType modType)
	{
		int modPoint = GetModPoint(modType);
		carMod.GetComponent<BaseItem>().ParentAttachTransform = _carModPoints[modPoint];
		carMod.GetComponent<BaseItem>().Parent = this;
		carMod.transform.SetParent(_carModPoints[modPoint]);
		switch (modType)
		{
			case CarModType.Nitro:
				carMod.transform.position += new Vector3(0f, 0.7f, 0.3f);
				carMod.transform.localScale = new Vector3(2, 2, 2);
				break;
			default:
				carMod.transform.localScale = new Vector3(1, 1, 1);
				break;
		}
		//carMod.transform.GetChild(0).transform.rotation = Quaternion.identity;
		//carMod.transform.GetChild(0).transform.localScale = new Vector3(5, 5, 5);
		_hasItemEquipped = true;
	}

	//Returns which index (transform of the _carModPoints) the provided CarModType gets attached to
	private int GetModPoint(CarModType modType)
	{
		switch (modType)
		{
			case CarModType.BatteringRam:
			{
				return 0;
			}
			case CarModType.Nitro:
			{
				return 2;
			}
			case CarModType.MineDropper:
			{
				return 2;
			}
			default:
			{
				Debug.Log("Invalid ModType: " + modType.ToString());
				return -1;
			}
		}
	}
	//Sets the id of the last enemy got hit by
	public void SetEnemyId( int id)
    {
		//Gets the enemy id who rammed you and give the info to the correct places
		_previousEnemyID = id;
		//resets counter of enemy id resets
		_enemyIDCounter = 0f;
	}
	private void ResetHaptics()
	{
		_gamepad.ResetHaptics();
	}




	private void ShowParticleOnHit(Collision collision)
	{
		foreach (var VARIABLE in _wallHitParticleSystems)
		{
			if (!VARIABLE.isPlaying)
			{
				VARIABLE.transform.position = collision.GetContact(0).point;
				VARIABLE.Play();
				return;
			}
		}

	}
}