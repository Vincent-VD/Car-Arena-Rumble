using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BatteringRam : BaseItem
{
    [SerializeField]
    private int _impactModifier = 5;
    private const CarModType _carModType = CarModType.BatteringRam;
    private const CarModPlace _carModPlace = CarModPlace.Front;
	private const string PLAYER_TAG = "Player";

	private bool _isActive = false;

	private void Start()
	{
		_maxTimer = 8f;
	}
	
    // Update is called once per frame
    private void Update()
    {
	    _currTimer += Time.deltaTime;
	    if (_currTimer > _maxTimer)
	    {
		    Discard();
	    }

	    if (parentAttachTransform)
	    {
			//Update gameObject transform to match 'parent' transform (transform of car mod point)
		    transform.position = parentAttachTransform.position;
		    transform.rotation = parentAttachTransform.rotation;
        }
    }

    public override int ModPartModifier
    {
        get
        {
            return _impactModifier;
        }
    }
    public override bool CountAsRamming
    {
        get
        {
            return true;
        }
    }
    public override void UseAbility()
    {
    }
}
