using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _cinemaCamera = null;

    [SerializeField]
    private CinemachineTargetGroup _cinemaTargetGroup = null;

    [SerializeField]
    private Transform[] _cameraPointAnchors = null;

    [SerializeField]
    private float _amplitudeGain = 1f;

    [SerializeField]
    private float _frequencyGain = 1f;

    [SerializeField]
    private float _shakeDuration = 0.75f;

    private CinemachineBasicMultiChannelPerlin _cinemachinebasicMultiChannelPerlin = null;
    private PlayerManager _playerManager = null;
    private bool _hasAssignendTargets = false;
    private float _height = 0f, _shakeCounter = 0f;
    private const int _carWeight = 20, _carRadius = 15, _offsetPointWeight = 15, _offsetPointRadius = 1;

    // Start is called before the first frame update
    void Start()
    {
        _playerManager = PlayerManager.Instance;
        _cinemachinebasicMultiChannelPerlin = _cinemaCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(); 
        _height = transform.position.y;
        PlayerManager.Instance.CameraBeh = this;
        _cinemachinebasicMultiChannelPerlin.m_FrequencyGain = _frequencyGain;
    }

    // Update is called once per frame
    void Update()
    {
        //I learned this info from https://www.youtube.com/watch?v=ACf1I27I6Tk
        if (_shakeCounter > 0)
        {
            _shakeCounter -= Time.deltaTime;
            _cinemachinebasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(_amplitudeGain, 0f, (1f - _shakeCounter / _shakeDuration));
            //_cinemachinebasicMultiChannelPerlin.m_FrequencyGain = Mathf.Lerp(_frequencyGain, 0f, (1f - _shakeCounter / _shakeDuration));
        }
        else if (!_hasAssignendTargets)
        {
            //this gets called only once
            AssignTargets();
        }
    }

    void LateUpdate()
    {
        Vector3 tmpPos = transform.position;
        tmpPos.y = _height;
        transform.position = tmpPos;
    }

    //this assigns the cars and the offset as the targets of the targetgroup
    //where the camera looks at
    public void AssignTargets(int radius = _carRadius, int weight = _carWeight)
    {
        int index = 0;
        _cinemaTargetGroup.m_Targets = new CinemachineTargetGroup.Target[_playerManager.Players.Count + _cameraPointAnchors.Length];
        CinemachineTargetGroup.Target tmp = new CinemachineTargetGroup.Target();
        //Put in the car targets, for the camera to look at
        tmp.radius = radius;
        tmp.weight = weight;
        foreach ( var player in _playerManager.Players)
        {
            tmp.target = player.transform;
            _cinemaTargetGroup.m_Targets[index] = tmp;
           ++index;
        }
        //Put in the camera anchor, for the camera to look at and to shape the view
        tmp.radius = _offsetPointRadius;
        tmp.weight = _offsetPointWeight;
        foreach (var anchor in _cameraPointAnchors)
        {
            tmp.target = anchor.transform;
            _cinemaTargetGroup.m_Targets[index] = tmp;
            ++index;
        }

        _cinemaCamera.Follow = _cinemaTargetGroup.transform;

        _hasAssignendTargets = true;
    }

    public void ShakeCamera()
    {
        //I learned this info from https://www.youtube.com/watch?v=ACf1I27I6Tk
        //This sets the camera to shake
        //_cinemachinebasicMultiChannelPerlin.m_AmplitudeGain = _amplitudeGain;
        //_cinemachinebasicMultiChannelPerlin.m_FrequencyGain = _frequencyGain;
        _shakeCounter = _shakeDuration;
    }

}
