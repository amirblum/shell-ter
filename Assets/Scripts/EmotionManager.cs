﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmotionManager : MonoBehaviour
{
    [SerializeField] UIController _uiController;
    [SerializeField] PlayerController[] _players;
    [SerializeField] BirdController _bird;
    [SerializeField] float _yDistanctNeededToWin;
    [SerializeField] Transform _levelMax;
    [SerializeField] Transform _cameraLevelMax;
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (_isInEnding) return;

        var sumY = 0f;

        foreach (var player in _players)
        {
            sumY += player.transform.position.y;
        }

        var averageY = sumY / _players.Length;
        
        var slopeIntensity = (averageY >= _intensityIncreaseBeginY)
            ? (averageY - _intensityIncreaseBeginY) / (_levelMax.position.y - _intensityIncreaseBeginY)
            : 0f;

        var cameraSlopeIntensity = (averageY >= _intensityIncreaseBeginY)
            ? (averageY - _intensityIncreaseBeginY) / (_cameraLevelMax.position.y - _intensityIncreaseBeginY)
            : 0f;

        slopeIntensity = Mathf.Min(1, slopeIntensity);
        // Debug.Log(slopeIntensity);

        _musicManager.SetState(0, slopeIntensity);
        SetIntensity(slopeIntensity);
        CameraZoom(cameraSlopeIntensity);
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

    public void PlayerReachedEnd(PlayerController player)
    {
        if (_isInEnding) return;

        var playerYDistance = Mathf.Abs(_players[0].transform.position.y - _players[1].transform.position.y);

        if (playerYDistance <=_yDistanctNeededToWin)
        {
            StartCoroutine(EndingCoroutine());
        }
        else
        {
            player.RollBack();
        }
    }

    private IEnumerator EndingCoroutine()
    {
        _isInEnding = true;
        
        foreach (var player in _players)
        {
            player.ForceForward();
        }

        _musicManager.PlayEnding();
        _bird.GoAway();
        var intensity = 1f;
        for (; intensity >= 0; intensity -= Time.deltaTime / _endingFadeTime)
        {
            yield return null;
            SetIntensity(intensity);
        }
    }

    public void PlayFinalEnding()
    {            
        foreach (var player in _players)
        {
            player.StopPhysics();
        }

        _uiController.ShowEndScreen();

        // todo bird-go-away, clouds, etc.
    }
}
