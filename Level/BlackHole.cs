using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField]
    private float _forceAmount = 1f;
    private const float _forceMultiplier = 1000000f;
    private const string PLAYER_TAG = "Player";


    private void OnTriggerEnter(Collider other)
    {
        Transform otherParent = other.transform.parent;
        if (otherParent == null)
            return;

        if (otherParent.CompareTag(PLAYER_TAG) )
        {
            otherParent.GetComponent<Rigidbody>().AddForce((-transform.up + transform.forward) * (_forceMultiplier * _forceAmount));
        }
    }
}
