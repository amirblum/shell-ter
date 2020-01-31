using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsInShell
    {
        get
        {
            return _isInShell;
        }
        private set
        {
            var wasInShell = _isInShell;
            _isInShell = value;

            if (wasInShell != _isInShell)
            {
                _graphics.loop = !IsInShell;
                _graphics.AnimationName = _isInShell ? "Hide" : "Crawl";
            }
        }
    }
    private bool _isInShell = true;
    private bool _shellForced;
    [SerializeField] float _shellForcedTime;

    [Header("Controls")]
    [SerializeField] KeyCode _forwardKey;
    [SerializeField] KeyCode _breakShellKey;

    [Header("Movement")]
    [SerializeField] bool _facingRight;
    [SerializeField] float _forwardThrust;

    [Header("States")]
    [SerializeField] SkeletonAnimation _graphics;
    [SerializeField] Collider2D _defaultCollider;
    [SerializeField] Collider2D _hitCollider;

    private Rigidbody2D _rigidbody;

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected void Update()
    {
        if (_shellForced) return;

        var wasInShell = IsInShell;

        IsInShell = !Input.GetKey(_forwardKey);

        if (!wasInShell && IsInShell)
        {
            StartCoroutine(HideStuckCoroutine());
        }
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

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        var bird = collider.gameObject.GetComponentInParent<BirdController>();
        if (bird == null) return;
        bird.ResetTarget();

        if (IsInShell) return;

        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        IsInShell = true;
        _defaultCollider.gameObject.SetActive(false);
        _hitCollider.gameObject.SetActive(true);
        var oldGravity = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 1f;
        _shellForced = true;

        yield return new WaitForSeconds(_shellForcedTime);

        _shellForced = false;
        _rigidbody.gravityScale = oldGravity;
        _hitCollider.gameObject.SetActive(false);
        _defaultCollider.gameObject.SetActive(true);
    }

    private IEnumerator HideStuckCoroutine()
    {
        _shellForced = true;
        yield return new WaitForSeconds(_shellForcedTime / 2);
        _shellForced = false;
    }
}
