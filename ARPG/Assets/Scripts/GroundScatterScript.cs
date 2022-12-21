using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DrawBoxCast;

public class GroundScatterScript : MonoBehaviour
{
    public GameObject[] groundScatterPoints;
    private float _delayBetween;
    private float _timer;
    private void Start()
    {
        _delayBetween = 0;
    }

    private void Update()
    {
        for (var i = 0; i < groundScatterPoints.Length; i++)
        {
            while (_timer < _delayBetween)
            {
                _timer += Time.deltaTime;
            }
            _delayBetween += 0.175f;
            
        }

        
    }
}
