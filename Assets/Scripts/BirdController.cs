using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] PlayerController[] _players;
    private float _sequenceDirection = 1;
    private float _initialY;
    [SerializeField] float _speed = 3f;
    [SerializeField] float _diveSpeed = 20f;
    bool _canTarget = true;
    PlayerController _target = null;

    void Awake()
    {
        _initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (_canTarget && _target == null)
        {
            PlayerController closestPlayer = null;
            float distanceToClosestPlayer = 9999; // Largest number imaginable to man
            foreach (var player in _players)
            {
                if (player.IsInShell)
                {
                    continue;
                }
                var distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < distanceToClosestPlayer)
                {
                    distanceToClosestPlayer = distance;
                    closestPlayer = player;
                }
            }
            _target = closestPlayer;
            _canTarget = false;
        }
        if (_target == null)
        {
            Hover();
        }
        else
        {
            // TODO: consider avoiding the mountain
            SetPosition(transform.position + Vector3.Normalize(_target.transform.position - transform.position) * _diveSpeed * Time.deltaTime);
        }
    }

    public void ResetTarget()
    {
        _target = null;
    }

    private void SetPosition(Vector3 position)
    {
        float direction = position.x - transform.position.x;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(direction);
        transform.position = position;
        transform.localScale = scale;
    }

    private void Hover()
    {
        float maxX = -9999;
        float minX = 9999;
        foreach (var player in _players) {
            maxX = Mathf.Max(maxX, player.transform.position.x);
            minX = Mathf.Min(minX, player.transform.position.x);
        }
        // float clampedMaxX = Mathf.Clamp(maxX, -7, 7);
        // float clampedMinX = Mathf.Clamp(minX, -7, 7);
        float clampedMaxX = 7;
        float clampedMinX = -7;
        float distanceX = _diveSpeed * _sequenceDirection * Time.deltaTime;
        var x = Mathf.Clamp(transform.position.x + distanceX, clampedMinX, clampedMaxX);
        var y = Mathf.Min(_initialY, transform.position.y + Mathf.Sign(_initialY - transform.position.y) * Time.deltaTime * _diveSpeed / 2);
        SetPosition(new Vector3(x, y, 0));

        if (x == clampedMinX || x == clampedMaxX)
        {
            _sequenceDirection *= -1;
            if (y == _initialY)
            {
                _canTarget = true;
            }
        }
    }
}
