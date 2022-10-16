using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CarModType
{
	None = -1,
	BatteringRam = 0,
	Nitro = 1,
	MineDropper = 2

}

public enum CarModPlace
{
    None = -1,
    Front = 0,
    Top = 1,
	Rear = 2
}

public struct CarModExport
{
	public CarModExport(Sprite sprite, CarModType type)
	{
		this.sprite = sprite;
		itemType = type;
	}

	public Sprite sprite;
	public CarModType itemType;

}


public class BaseItem : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] _particleSystemArray = null;

	protected Transform parentAttachTransform = null;
	protected BasicCarCharachter parent = null;

	//Timers to be used for inherited car mods
	protected float _currTimer = 0f;
	protected float _maxTimer;

	public void ActivatePartciles()
    {
        for (int i = 0; i < _particleSystemArray.Length; i++)
        {
            if (!_particleSystemArray[i].isPlaying)
            {
				_particleSystemArray[i].Play();
			}
        }
    }
	public void ActivatePartciles(Vector3 pos)
	{
		for (int i = 0; i < _particleSystemArray.Length; i++)
		{
			if (!_particleSystemArray[i].isPlaying)
			{
				_particleSystemArray[i].transform.position = pos;
				_particleSystemArray[i].Play();
			}
		}
	}

	public void PausePartciles()
	{
		for (int i = 0; i < _particleSystemArray.Length; i++)
		{
			if (_particleSystemArray[i].isPlaying)
			{
				_particleSystemArray[i].Pause();
			}
		}
	}

	public void StopPartciles()
	{
		for (int i = 0; i < _particleSystemArray.Length; i++)
		{
			if (_particleSystemArray[i].isPlaying)
			{
				_particleSystemArray[i].Stop();
			}
		}
	}

	public Transform ParentAttachTransform
	{
		set => parentAttachTransform = value;
	}

	public BasicCarCharachter Parent
	{
		get => parent;
		set => parent = value;
	}

	//this is the default mod part modifier and this gets added to the knockback calculations
	//if there is a modpart that doesnt do any knockback, just make it 0 them
    public virtual int ModPartModifier => 0;
	public virtual bool CountAsRamming => false;

	public void Discard()
    {
	    parent.GetComponentInParent<Inventory>().HasItemEquipped = false;
	    Destroy(this.gameObject);
    }

	public virtual void UseAbility()
	{
	}

}
