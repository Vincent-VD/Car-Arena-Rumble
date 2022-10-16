using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Nitro : BaseItem
{

    private const CarModType _carModType = CarModType.Nitro;
    private const CarModPlace _carModPlace = CarModPlace.Rear;

    [SerializeField]
    private float _nitroDuration = 1f;

    [SerializeField]
    private float _nitroSpeedBoost  = 0.75f;

    [SerializeField]
    private ParticleSystem[] _nitroParticleSystems;

    private bool _isActive = false;

    private void Start()
    {
        //nitro duration
        _maxTimer = _nitroDuration;
        _currTimer = 2f;

    }

    private const int _nitroMaxUses = 3;
    private int _currentNitroUses = 0;
    private const float _nitroSpeedBoostMultiplier = 25000f;

    // Update is called once per frame
    private void Update()
    {
        //Durations of the boost
        if (_currTimer < _maxTimer && _isActive)
        {
            _currTimer += Time.deltaTime;
            NitroBoost();
        }
        else if (_currTimer >= _maxTimer && _isActive)
        {
            _isActive = false;
            StopPartciles();
            DeactivateNitroParticles();
            _currentNitroUses++;
        }
        if (_currentNitroUses == _nitroMaxUses)
        {
            _isActive = false;
            Invoke("Discard", _maxTimer);
            return;
        }
    


        if (parentAttachTransform)
        {
            //Update gameObject transform to match 'parent' transform (transform of car mod point)
            transform.position = parentAttachTransform.position;
            transform.localPosition += new Vector3(0f, 0.55f, 0.3f);
            transform.rotation = parentAttachTransform.rotation;
        }
    }

    //Gives the car a boost
    private void NitroBoost()
    {
        parent.gameObject.GetComponent<Rigidbody>().AddForce(
            parent.gameObject.GetComponent<CarMovementBehavior>().ForwardBody.forward * (_nitroSpeedBoost * _nitroSpeedBoostMultiplier));

 
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
       
            SoundManager.Instance.PlayNitroSound();
            _isActive = true;
            _currTimer = 0f;
            ActivatePartciles();
            ActivateNitroParticles();

        }
    }
    //plays the particles
    private void ActivateNitroParticles()
    {
        foreach (var item in _nitroParticleSystems)
        {
            if (!item.isPlaying)
            {
                item.Play();
            }
        }
    }

    //pause the particles
    private void DeactivateNitroParticles()
    {
        foreach (var item in _nitroParticleSystems)
        {
            if (item.isPlaying)
            {
                item.Stop();
            }
        }
    }
}
