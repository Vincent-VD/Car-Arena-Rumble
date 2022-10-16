using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineDropper : BaseItem
{
    [SerializeField]
    private GameObject _mineObj = null;

    [SerializeField]
    private GameObject[] _mineVisuals = null;

    [SerializeField]
    private float _mineDropCooldown = 1f;

    [SerializeField]
    private Transform _mineSpawnPos = null;

    private const CarModType _carModType = CarModType.MineDropper;
    private const CarModPlace _carModPlace = CarModPlace.Rear;
    private bool _isActive = false;

    private const int _maxMineDrops = 3;
    private int _currentMineDrops = 0;



    // Start is called before the first frame update
    void Start()
    {
        _maxTimer = _mineDropCooldown;
        _currTimer = 2f;
    
        GetComponentInChildren<Mine>().PlayerID = Parent.gameObject.GetInstanceID();
    }

    // Update is called once per frame
    void Update()
    {
        //Durations of the boost
        if (_currTimer < _maxTimer && _isActive)
        {
            _currTimer += Time.deltaTime;
        }
        else if (_currTimer >= _maxTimer && _isActive)
        {
            _isActive = false;
            _currentMineDrops++;
        }
        if (_currentMineDrops == _maxMineDrops)
        {
            _isActive = false;
            Discard();
        }

        if (parentAttachTransform)
        {
            //Update gameObject transform to match 'parent' transform (transform of car mod point)
            gameObject.transform.position = parentAttachTransform.position;
            gameObject.transform.rotation = parentAttachTransform.rotation;
        }
        //checks if the last of the array is empty or not
        if (!_mineVisuals[2])
            return;
        //gets the mine and checks if it has exploded
        var mine = _mineVisuals[2].GetComponent<Mine>();
        if (mine.HasExploded)
        {
            _currentMineDrops++;
            DestroyMineVisual();
            mine.HasExploded = false;
        }
    }

    public override int ModPartModifier
    {
        get
        {
            return 0;
        }
    }
    public override void UseAbility()
    {
        if (!_isActive)
        {
            SoundManager.Instance.PlayMineDropSound();
            //resets cooldown
            _isActive = true;
            _currTimer = 0f;
            //removes the first non null visual object, so you think you used that one
            DestroyMineVisual();
            //Spawn mine
            SpawnMine();
        }
    }

    //Spawn mine
    private void SpawnMine()
    {
        //spawns a mine on the mineSpawnPos 
        GameObject tmp = Instantiate(_mineObj, _mineSpawnPos.position, _mineSpawnPos.rotation);
        //Gives the player id to the mine to prevent earning kills when killing your own mines
        tmp.GetComponent<Mine>().PlayerID = Parent.gameObject.GetInstanceID();
    }

    //this destroys a mesh visual in the minedropper barrel, this is to show visual that there are only x amnount of mines left in it
    private void DestroyMineVisual()
    {
        for (int i = 0; i < _mineVisuals.Length; i++)
        {
            if (_mineVisuals[i] != null)
            {
                Destroy(_mineVisuals[i]);
                return;
            }
        }
    }
}
