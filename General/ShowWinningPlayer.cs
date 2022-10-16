using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWinningPlayer : MonoBehaviour
{
    
    [SerializeField]
    private MeshRenderer _meshFilter = null;

    private void Start()
    {
        DeActivate();
    }
    public void SetStatus(bool active)
    {
        if (active)
        {
            Activate();
        }
        else
        {
            DeActivate();
        }
    }
    //Makes render part invisible
    private void DeActivate()
    {
        _meshFilter.enabled = false;
    }
    //Makes render part visible
    private void Activate()
    {
        _meshFilter.enabled = true;
    }
}
