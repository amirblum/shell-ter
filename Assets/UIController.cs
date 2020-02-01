using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject _startScreen;
    [SerializeField] GameObject _endScreen;

    protected void Awake()
    {
        _startScreen.SetActive(true);
        _endScreen.SetActive(false);
    }
}
