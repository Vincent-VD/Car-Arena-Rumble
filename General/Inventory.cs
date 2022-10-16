using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{

	private BasicCarCharachter _parentOwner = null;

	private GameObject _carMod = null;
	public GameObject CarMod => _carMod;


	private bool _hasItemEquipped = false;
	public bool HasItemEquipped
	{
		get => _hasItemEquipped;
		set => _hasItemEquipped = value;
	}

	private void Start()
	{
		_parentOwner = this.gameObject.GetComponent<BasicCarCharachter>();
	}

	//Add item to active slot if empty, else to inventory slot
	//  => returns true if item is added
	public bool AddItem(CarModType modType)
	{

		if (_hasItemEquipped) return false;

		_carMod = CarModManager.Instance.GetCarModGameObject(modType); //gets car mod GO from CarModManager to then add to player
		_parentOwner.AttachMod(_carMod, modType);
		_hasItemEquipped = true;
		return true;

	}

	//Use ability if item charges (ex nitro)
	public void UseAbility(InputAction.CallbackContext callback)
	{
		if (callback.phase != InputActionPhase.Performed) return;
		if (_hasItemEquipped)
		{
			_carMod.GetComponent<BaseItem>().UseAbility();
			Debug.Log("Ability used");
		}
	}

	//Remove car mod
	public void Discard()
	{
		if (!_hasItemEquipped) return;

		_hasItemEquipped = false;
		_carMod.GetComponent<BaseItem>().Discard(); //Destroys car mod
		Debug.Log("Ability discarded");
	}

}
