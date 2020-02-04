using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _hitSFX;
    [SerializeField] AudioClip _moveSFX;
    [SerializeField] AudioClip _enterShellSFX;
    [SerializeField] AudioClip _exitShellSFX;

    public enum State
    {
        IDLE,
        GETTING_IN_SHELL,
        IN_SHELL,
        GETTING_OUT_OF_SHELL,
        MOVING,
        HIT,
        ABOUT_TO_LOVE,
        LOVING,
    }

    private State _state;
    private bool _invincible;
    private bool _controlsDisabled = false;

    public bool invincible
    {
        get
        {
            return _invincible || _invincibilityDebug;
        }
    }

    public State state
    {
        get
        {
            return _state;
        }
        set
        {
            switch (value)
            {
                case State.IDLE:
                    _graphics.loop = true;
                    _graphics.AnimationName = "Idle";
                    break;
                case State.GETTING_IN_SHELL:
                    _audioSource.PlayOneShot(_enterShellSFX);
                    _graphics.loop = false;
                    _graphics.timeScale = 0.2f;
                    _graphics.AnimationName = "Hide In";
                    break;
                case State.IN_SHELL:
                    _graphics.loop = true;
                    _graphics.timeScale = 1f;
                    _graphics.AnimationName = "Hide Idle";
                    break;
                case State.GETTING_OUT_OF_SHELL:
                    _audioSource.PlayOneShot(_exitShellSFX);
                    _graphics.loop = false;
                    _graphics.AnimationName = "Hide Out";
                    break;
                case State.MOVING:
                    SetCollider(ColliderState.DEFAULT);
                    // TODO: loop crawl sound
                    _graphics.loop = true;
                    _graphics.AnimationName = "Crawl";
                    break;
                case State.HIT:
                    SetCollider(ColliderState.SHELL);
                    _audioSource.PlayOneShot(_hitSFX);
                    _graphics.loop = false;
                    _graphics.AnimationName = "DMG";
                    break;
                case State.ABOUT_TO_LOVE:
                    _graphics.loop = false;
                    _graphics.AnimationName = "Love";
                    break;
                case State.LOVING:
                    _graphics.loop = false;
                    _graphics.AnimationName = "LoveIdle";
                    break;
            }
            _state = value;
        }
    }

    protected enum ColliderState
    {
        DEFAULT,
        SHELL,
    }

    protected void SetCollider(ColliderState state)
    {
        var shouldUseHitCollider = state == ColliderState.SHELL;
        _defaultCollider.gameObject.SetActive(!shouldUseHitCollider);
        _hitCollider.gameObject.SetActive(shouldUseHitCollider);
    }

    private Vector2 _upNormal = Vector2.up;

    [Header("Timing")]
    [SerializeField] float _invicibilityTime;
    [SerializeField] float _shellEnterTime;
    [SerializeField] float _shellExitTime;

    [Header("Defense/Hit")]
    [SerializeField] SpriteRenderer _slash;
    [SerializeField] bool _invincibilityDebug;

    [Header("Controls")]
    [SerializeField] KeyCode _forwardKey;
    [SerializeField] Image _controlsImage;
    private bool _movedOnce;

    [Header("Movement")]
    [SerializeField] bool _facingRight;
    [SerializeField] float _forwardThrust;
    private bool _wasJustHit;
    private bool _forceForward;
    private IEnumerator _activeStateChangeRoutine;

    [Header("States")]
    [SerializeField] SkeletonAnimation _graphics;
    [SerializeField] Collider2D _defaultCollider;
    [SerializeField] Collider2D _hitCollider;

    private Rigidbody2D _rigidbody;

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _slash.enabled = false;
        state = State.IDLE;
        StartStateChangeCoroutine(GetInShell());
    }

    protected void StartStateChangeCoroutine(IEnumerator routine)
    {
        if (_activeStateChangeRoutine != null)
        {
            StopCoroutine(_activeStateChangeRoutine);
        }
        StartCoroutine(routine);
        _activeStateChangeRoutine = routine;
    }

    protected IEnumerator GetInShell()
    {
        state = State.GETTING_IN_SHELL;
        yield return new WaitForSeconds(_shellExitTime);
        state = State.IN_SHELL;
    }

    protected IEnumerator GetOutOfShell()
    {
        state = State.GETTING_OUT_OF_SHELL;
        yield return new WaitForSeconds(_shellEnterTime);
        state = State.MOVING;
    }

    private IEnumerator ControlsFadeOutCoroutine()
    {
        var fadeOutTime = 0.5f;
        var color = Color.white;
        for (; fadeOutTime >= 0; fadeOutTime -= Time.deltaTime)
        {
            color.a = fadeOutTime / 0.5f;
            _controlsImage.color = color;
            yield return null;
        }
    }

    protected void FixedUpdate()
    {
        if (_controlsDisabled) return;
        var wantsToMove = _forceForward || Input.GetKey(_forwardKey);
        // Debug.Log($"update: {state} - {wantsToMove}");
        if (!wantsToMove)
        {
            if (state == State.MOVING || state == State.GETTING_OUT_OF_SHELL)
            {
                StartStateChangeCoroutine(GetInShell());
            }
            return;
        }
        switch (state)
        {
            case State.IN_SHELL:
            case State.GETTING_IN_SHELL:
                if (!_movedOnce)
                {
                    _movedOnce = true;
                    StartCoroutine(ControlsFadeOutCoroutine());
                }
                StartStateChangeCoroutine(GetOutOfShell());
                break;
            case State.MOVING:
                if (_wasJustHit)
                {
                    transform.up = _upNormal;
                    _wasJustHit = false;
                }
                var directionMultiplier = _facingRight ? -1f : 1f;
                var rightAverage = (transform.right + Vector3.right).normalized;
                _rigidbody.AddForce(rightAverage * _forwardThrust * directionMultiplier);
                break;
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (!(_wasJustHit && _hitCollider.gameObject.active)) return;
        _upNormal = collision.contacts[0].normal;
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        var bird = collider.gameObject.GetComponentInParent<BirdController>();
        if (bird == null) return;

        bird.ResetTarget();

        if (invincible || state == State.IN_SHELL) return;

        StartStateChangeCoroutine(RollCoroutine());
        StartCoroutine(SlashCoroutine());
    }

    public void RollBack()
    {
        StartStateChangeCoroutine(RollCoroutine());
    }

    private IEnumerator RollCoroutine()
    {
        var oldGravity = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 2f;
        state = State.HIT;
        _invincible = true;
        _wasJustHit = true;
        _controlsDisabled = true;

        yield return new WaitForSeconds(_invicibilityTime);

        state = State.IN_SHELL;
        _invincible = false;
        _controlsDisabled = false;
        _rigidbody.gravityScale = oldGravity;
    }

    private IEnumerator SlashCoroutine()
    {
        var slashTime = 0.5f;

        _slash.enabled = true;
        yield return new WaitForSeconds(slashTime / 3f);
        _slash.enabled = false;
        yield return new WaitForSeconds(slashTime / 3f);
        _slash.enabled = true;
        yield return new WaitForSeconds(slashTime / 3f);
        _slash.enabled = false;
    }

    public void StopPhysics()
    {
        StartStateChangeCoroutine(EndCoroutine());
    }

    private IEnumerator EndCoroutine()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        // TODO: consider transitioning through Love animation
        _forceForward = false;
        state = State.ABOUT_TO_LOVE;
        yield return new WaitForSeconds(0.2f);
        state = State.LOVING;
    }

    public void ForceForward()
    {
        _invincible = true;
        _forceForward = true;
    }
}
