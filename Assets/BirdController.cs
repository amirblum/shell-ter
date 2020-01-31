﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] PlayerController[] _players;
    private float _sequenceDirection = 1;
    private float _initialY;
    [SerializeField] float _speed = 3f;

    void Awake()
    {
        _initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
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
        if (closestPlayer == null)
        {
            Hover();
        }
        else
        {
            // TODO: consider avoiding the mountain
            SetPosition(transform.position + Vector3.Normalize(closestPlayer.transform.position - transform.position) * _speed * Time.deltaTime);
        }
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
        float distanceX = _speed * _sequenceDirection * Time.deltaTime;
        var x = Mathf.Clamp(transform.position.x + distanceX, -7, 7);
        var y = transform.position.y + Mathf.Sign(_initialY - transform.position.y) * Time.deltaTime * _speed;
        SetPosition(new Vector3(x, y, 0));

        if (Mathf.Abs(x) == 7)
        {
            _sequenceDirection *= -1;
        }
    }
}