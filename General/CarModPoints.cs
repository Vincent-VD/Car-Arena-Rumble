using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarModPoints : MonoBehaviour
{
	//Simple class containing transforms of mod points for each VisualCar
	//  Assigned to BasicCarCharacter's _carModPoints variable at initialization

	[SerializeField]
	private Transform[] _modPoints;

	public Transform[] ModPoints
	{ get { return _modPoints; } }

}
