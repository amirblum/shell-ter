using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsInShell { get; private set; }

    [Header("Controls")]
    [SerializeField] KeyCode _forwardKey;
    [SerializeField] KeyCode _breakShellKey;

    [Header("Movement")]
    [SerializeField] bool _facingRight;
    [SerializeField] float _forwardThrust;

    private Rigidbody2D _rigidbody;

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected void Update()
    {
        IsInShell = !Input.GetKey(_forwardKey);
    }

    protected void FixedUpdate()
    {
        if (!IsInShell)
        {
            var directionMultiplier = _facingRight ? -1f : 1f;
            var rightAverage = (transform.right + Vector3.right).normalized;
            _rigidbody.AddForce(rightAverage * _forwardThrust * directionMultiplier);
        }
        
    }
}
