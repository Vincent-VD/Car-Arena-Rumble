using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float _invincibilityTimeOnSpawn = 1.0f;

    [SerializeField]
    private ParticleSystem[] _deathParticleSystems = null;

    private bool _IsInvincible = false;
    private bool _isAlive = true;


    public bool IsAlive
    {
        get => _isAlive;
        set => _isAlive = value;
    }

    public bool IsInvincible
    {
        get => _IsInvincible;
        set => _IsInvincible = value;
    }

    public void PlayDeathPartciles(Vector3 pos)
    {
        for (int i = 0; i < _deathParticleSystems.Length; i++)
        {
            _deathParticleSystems[i].transform.position = pos;
            _deathParticleSystems[i].Play();
        }
    }
   
    public void ResetInvincible()
    {
        Invoke("TurnOffInvincible", _invincibilityTimeOnSpawn);
    }
    private void TurnOffInvincible()
    {
        _IsInvincible = false;
    }

}
