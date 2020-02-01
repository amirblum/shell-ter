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

    [Header("CameraZoom")]
    [SerializeField] float _zoomFromPercent;
    [SerializeField] Transform _cameraHolder;
    [SerializeField] float _blackBarPercent = 0.3f;
    [SerializeField] RectTransform _endingBarTop;
    [SerializeField] RectTransform _endingBarBottom;
    [SerializeField] float _cameraEndingY;
    private Vector3 _cameraDefaultPos;
    private Vector3 _cameraEndingPos;

    private bool _isInEnding;

    protected void Start()
    {
        _cameraDefaultPos = _cameraHolder.position;
        _cameraEndingPos = new Vector3(_cameraDefaultPos.x, _cameraEndingY, _cameraDefaultPos.z);
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            Time.timeScale--;
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            Time.timeScale++;
        }

        if (_isInEnding) return;

        var numOutOfShell = 0;
        var sumY = 0f;

        foreach (var player in _players)
        {
            if (!player.IsCountedAsInShell)
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
        CameraZoom(slopeIntensity);
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

    private void CameraZoom(float slopeIntensity)
    {
        var cameraZoomLerp = Mathf.Clamp01((slopeIntensity - _zoomFromPercent) / (1f - _zoomFromPercent));
        _endingBarTop.anchorMin = new Vector2(0f, Mathf.Lerp(1f, 1f - _blackBarPercent, cameraZoomLerp));
        _endingBarBottom.anchorMax = new Vector2(1f, Mathf.Lerp(0f, _blackBarPercent, cameraZoomLerp));
        _cameraHolder.transform.position = Vector3.Lerp(_cameraDefaultPos, _cameraEndingPos, cameraZoomLerp);
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
