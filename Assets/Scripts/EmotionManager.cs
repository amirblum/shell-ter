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

    protected void Update()
    {
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
        _cameraShake.SetShakeAmount(slopeIntensity);
    }
}
