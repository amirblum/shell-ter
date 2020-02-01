using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private bool _playerLeftTouching;
    private bool _playerRightTouching;

    [SerializeField] EmotionManager _emotionManager;

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        var player = collider.gameObject.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            _emotionManager.PlayerReachedEnd(player);
        }
    }
}
