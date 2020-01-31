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
    [SerializeField] float _forwardSpeed;
    [SerializeField] Vector3 _forwardDirection;

    protected void Update()
    {
        if (Input.GetKey(_forwardKey))
        {
            IsInShell = false;
        }
        else
        {
            IsInShell = true;
        }

    }
}
