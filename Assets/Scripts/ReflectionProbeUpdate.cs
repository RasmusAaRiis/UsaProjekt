using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeUpdate : MonoBehaviour
{
    [Tooltip("Leave empty to update every frame")]
    [SerializeField] private float timeBetween;

    private float _timer;

    private ReflectionProbe _reflectionProbe;

    private void Start()
    {
        TryGetComponent(out _reflectionProbe);
    }

    private void FixedUpdate()
    {
        if (!_reflectionProbe)
        {
            if (timeBetween == 0)
            {
                _reflectionProbe.RenderProbe();
            }
            else
            {
                _timer += Time.deltaTime;

                if (_timer >= timeBetween)
                {
                    _reflectionProbe.RenderProbe();
                    _timer = 0;
                }
            }
        }
    }
}
