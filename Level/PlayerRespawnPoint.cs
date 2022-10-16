using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnPoint : MonoBehaviour
{
    private bool _isAvailable = true;
    private int _totalInZone = 0;
    private const string PLAYER_TAG = "Player";

    public bool IsAvailable
    {
        get
        {
            return _isAvailable;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            _isAvailable = false;
            _totalInZone++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            _totalInZone--;
            if (_totalInZone == 0)
            {
                _isAvailable = true;
            }
        }
    }
}
