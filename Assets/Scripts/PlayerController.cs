using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsCountedAsInShell
    {
        get
        {
                return _isInShell || _wantsToBeInShell;
        }
    }

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
                var shouldUseHitCollider = _isInShell && !_shellForced;
                _defaultCollider.gameObject.SetActive(!shouldUseHitCollider);
                _hitCollider.gameObject.SetActive(shouldUseHitCollider);
                _graphics.loop = !_isInShell;
                _graphics.AnimationName = _isInShell ? "Hide Idle" : "Crawl";
            }
        }
    }
    private bool _isInShell = false;
    private bool _wasJustHit = false;
    private Vector2 _upNormal = Vector2.up;
    private bool _wantsToBeInShell = false;
    private bool _shellForced;
    [SerializeField] float _shellForcedTime;
    [SerializeField] float _shellEnterTime;

    [Header("Defense/Hit")]
    [SerializeField] SpriteRenderer _slash;
    [SerializeField] bool _invincibilityDebug;

    [Header("Controls")]
    [SerializeField] KeyCode _forwardKey;

    [Header("Movement")]
    [SerializeField] bool _facingRight;
    [SerializeField] float _forwardThrust;
    private bool _forceForward;

    [Header("States")]
    [SerializeField] SkeletonAnimation _graphics;
    [SerializeField] Collider2D _defaultCollider;
    [SerializeField] Collider2D _hitCollider;

    private Rigidbody2D _rigidbody;

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _slash.enabled = false;
    }

    protected void Update()
    {
        if (_shellForced) return;

        var wasOutOfShell = !_isInShell;

        _wantsToBeInShell = !(Input.GetKey(_forwardKey) || _forceForward);

        if (wasOutOfShell && _wantsToBeInShell)
        {
            StartCoroutine(HideStuckCoroutine());
        }
        else if (!_wantsToBeInShell)
        {
            IsInShell = false;
        }
    }

    protected void FixedUpdate()
    {
        if (_shellForced || _wantsToBeInShell) return;
        if (_wasJustHit)
        {
            transform.up = _upNormal;
            _wasJustHit = false;
        }
        else
        {
            var directionMultiplier = _facingRight ? -1f : 1f;
            var rightAverage = (transform.right + Vector3.right).normalized;
            _rigidbody.AddForce(rightAverage * _forwardThrust * directionMultiplier);
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (!(_wasJustHit && _hitCollider.gameObject.active)) return;
        _upNormal /= collision.contacts[0].normal;
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        var bird = collider.gameObject.GetComponentInParent<BirdController>();
        if (bird == null) return;

        bird.ResetTarget();

        if (_isInShell || _invincibilityDebug) return;

        StartCoroutine(RollCoroutine());
        StartCoroutine(SlashCoroutine());
    }

    public void RollBack()
    {
        StartCoroutine(RollCoroutine());
    }

    private IEnumerator RollCoroutine()
    {
        IsInShell = true;
        var oldGravity = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 2f;
        _shellForced = true;
        _wantsToBeInShell = true;
        _wasJustHit = true;
        _graphics.loop = false;
        _graphics.AnimationName = "DMG";

        yield return new WaitForSeconds(_shellForcedTime);

        _shellForced = false;
        _rigidbody.gravityScale = oldGravity;
    }

    private IEnumerator SlashCoroutine()
    {
        var slashTime = _shellForcedTime / 8f;

        _slash.enabled = true;
        yield return new WaitForSeconds(slashTime / 3f);
        _slash.enabled = false;
        yield return new WaitForSeconds(slashTime / 3f);
        _slash.enabled = true;
        yield return new WaitForSeconds(slashTime / 3f);
        _slash.enabled = false;
    }

    private IEnumerator HideStuckCoroutine()
    {
        _shellForced = true;
        
        _graphics.loop = false;
        _graphics.timeScale = 0.2f;
        _graphics.AnimationName = "Hide In";

        yield return new WaitForSeconds(_shellEnterTime);

        _graphics.timeScale = 1f;

        IsInShell = true;
        _shellForced = false;
    }

    public void StopPhysics()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        // TODO: consider transitioning through Love animation
        _shellForced = true;
        _graphics.loop = true;
        _graphics.AnimationName = "LoveIdle";
        _forceForward = false;
    }

    public void ForceForward()
    {
        _forceForward = true;
    }
}
