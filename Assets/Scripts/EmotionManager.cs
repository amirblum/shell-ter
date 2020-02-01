using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionManager : MonoBehaviour
{
    [SerializeField] PlayerController[] _players;
    [SerializeField] Transform _levelMax;
    [SerializeField] float _intensityIncreaseBeginY = 1f;

    [SerializeField] MusicManager _musicManager;
    [SerializeField] CameraShake _cameraShake;
    [SerializeField] SpriteRenderer[] _moodSprites;
    [SerializeField] Color _moodColor;

    [SerializeField] float _endingFadeTime;

    private bool _isInEnding;

    protected void Update()
    {
        if (_isInEnding) return;

        var numOutOfShell = 0;
        var sumY = 0f;

        foreach (var player in _players)
        {
            if (!player.IsInShell)
            {
                numOutOfShell++;
            }
            
            sumY += player.transform.position.y;
        }

        var averageY = sumY / _players.Length;
        
        var slopeIntensity = (averageY >= _intensityIncreaseBeginY)
            ? (averageY - _intensityIncreaseBeginY) / (_levelMax.position.y - _intensityIncreaseBeginY)
            : 0f;

        slopeIntensity = Mathf.Min(1, slopeIntensity);
        Debug.Log(slopeIntensity);

        _musicManager.SetState(numOutOfShell, slopeIntensity);
        SetIntensity(slopeIntensity);
    }

    private void SetIntensity(float slopeIntensity)
    {
        _cameraShake.SetShakeAmount(slopeIntensity);

        var currentColor = Color.Lerp(Color.white, _moodColor, slopeIntensity);
        foreach (var moodSprite in _moodSprites)
        {
            moodSprite.color = currentColor;
        }
    }

    public void TriggerEnding()
    {
        if (_isInEnding) return;

        StartCoroutine(EndingCoroutine());
    }

    private IEnumerator EndingCoroutine()
    {
        _isInEnding = true;
        
        foreach (var player in _players)
        {
            player.StopPhysics();
        }

        var intensity = 1f;
        for (; intensity >= 0; intensity -= Time.deltaTime / _endingFadeTime)
        {
            yield return null;
            SetIntensity(intensity);
        }
        
        _musicManager.PlayEnding();
    }
}
