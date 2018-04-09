using System.Collections;
using System.Collections.Generic;
using SmartNet;
using UnityEngine;

public class ColorChecker : MonoBehaviour
{
    public bool IsActive
    {
        get { return _isActive; }
    }

    private Rigidbody _rb;
    private Material _material;
    private bool _isActive = false;
    private Color _baseColor;

    // Use this for initialization
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _material = GetComponent<Renderer>().material;
        _baseColor = _material.color;
    }

    void FixedUpdate()
    {
        if (NetworkServer.Active)
        {
            if (_rb.isKinematic)
                return;
            if (_rb.IsSleeping() && _isActive)
            {
                _isActive = false;
                _material.color = _baseColor;
            }
            else if (!_rb.IsSleeping() && !_isActive)
            {
                _isActive = true;
                _material.color = Color.red;
            }
        }
    }
}