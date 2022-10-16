using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _countdown = null;

    [SerializeField]
    private RoundTimer _roundTimer = null;

    private const string _goText = "Go!";
    private float _counter = 3f;
    private float _counterEnd = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _counter = PlayerManager.Instance.PlayersOffTime + 0.9f;
        SoundManager.Instance.SwitchToInGameTrack();

    }

    // Update is called once per frame
    void Update()
    {
        if (_countdown && _counter > 1f)
        {
            _countdown.text = ((int)_counter).ToString();
            _counter -= Time.deltaTime;
        }
        else if(_counterEnd > 0f)
        {
            _countdown.text = _goText;
            _counterEnd -= Time.deltaTime;
            _roundTimer.IsActive = true;
           
        }
        else
        { 
	        Destroy(gameObject);
        }

     
    }
}
