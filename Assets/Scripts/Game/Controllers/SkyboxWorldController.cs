using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxWorldController : MonoBehaviour
{
    [SerializeField] private float _speed = 0.01f;
    [SerializeField] private Material _skybox;
    [SerializeField] private AnimationCurve _exposureCurve;
    private float _currentSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _currentSpeed += _speed;
        if (_currentSpeed > 1.0f)
            _currentSpeed = 0.0f;
        
        _skybox.SetFloat("_Exposure", _exposureCurve.Evaluate(_currentSpeed));
    }
}
