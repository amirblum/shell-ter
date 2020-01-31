using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScroller : MonoBehaviour
{
    [SerializeField] float _scrollSpeed;
    
    [SerializeField] Transform _child1;
    [SerializeField] Transform _child2;
    
    private float _child1StartX;
    private float _child2StartX;

    private Transform[] _children;

    protected void Awake()
    {
        _children = new Transform[] { _child1, _child2 };

        _child1StartX = _child1.position.x;
        _child2StartX = _child2.position.x;
    }

    protected void FixedUpdate()
    {
        foreach (var child in _children)
        {
            child.position -= Vector3.right * _scrollSpeed * Time.deltaTime;
        }

        if (_child2.position.x < _child1StartX)
        {
            _child1.position = new Vector3(_child2StartX, _child1.position.y, _child1.position.z);
            var temp = _child2;
            _child2 = _child1;
            _child1 = temp;
        }
    }
}
