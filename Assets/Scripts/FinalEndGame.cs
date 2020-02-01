using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalEndGame : MonoBehaviour
{
    private bool _playerLeftTouching;
    private bool _playerRightTouching;

    [SerializeField] EmotionManager _emotionManager;

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLeft"))
        {
            _playerLeftTouching = true;
        }
        else if (collision.gameObject.CompareTag("PlayerRight"))
        {
            _playerRightTouching = true;
        }

        if (_playerLeftTouching && _playerRightTouching)
        {
            _emotionManager.PlayFinalEnding();
        }
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerLeft"))
        {
            _playerLeftTouching = false;
        }
        else if (collision.gameObject.CompareTag("PlayerRight"))
        {
            _playerRightTouching = false;
        }
    }
}
