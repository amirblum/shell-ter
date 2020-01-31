using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class MusicManager : MonoBehaviour
{
    [Range(0, 1)]
    public float slopeIntensity = 0;

    [SerializeField] PlayerController[] _players;
    [SerializeField] float _maxY = 4.0f;

    public enum State
    {
        STOP = 0,
        ONE_IS_MOVING = 1,
        BOTH_MOVING = 2,
        WIN_STATE = 3
    }

    //[Serializeable]
    public State requestedState = State.STOP;
    State currState;

    private FMOD.Studio.EventInstance instance_e;

    FMOD.Studio.PARAMETER_ID intensityParamID;
    FMOD.Studio.PARAMETER_ID stateParamID;

    // Start is called before the first frame update
    void Start()
    {
        currState = requestedState;
        //// Create and initialize the FMOD instance
        instance_e = FMODUnity.RuntimeManager.CreateInstance("event:/ShellMusic");
        instance_e.start();

        ////Chace a handle to the intesity parameter
        FMOD.Studio.EventDescription ShellMusicDescription = FMODUnity.RuntimeManager.GetEventDescription("event:/ShellMusic");
        FMOD.Studio.PARAMETER_DESCRIPTION intensityParameterDescription;
        ShellMusicDescription.getParameterDescriptionByName("Intensity", out intensityParameterDescription);
        intensityParamID = intensityParameterDescription.id;

        ////Chace a handle to the state parameter
        FMOD.Studio.PARAMETER_DESCRIPTION stateParameterDescription;
        ShellMusicDescription.getParameterDescriptionByName("GameMusicState", out stateParameterDescription);
        stateParamID = stateParameterDescription.id;
    }

    // Update is called once per frame
    void Update()
    {
        int numOutOfShell = 0;
        float sumY = 0;

        foreach (var player in _players)
        {
            if (!player.IsInShell)
            {
                numOutOfShell++;
            }
            sumY += player.transform.position.y;
        }
        requestedState = (State)numOutOfShell;
        slopeIntensity = Mathf.Min(1, (sumY / _players.Length) / _maxY);
        instance_e.setParameterByID(intensityParamID, slopeIntensity);
        UnityEngine.Debug.Log($"slope intensity: {slopeIntensity}");
        if (requestedState != currState)
        {
            currState = requestedState;
            instance_e.setParameterByID(stateParamID, (uint)currState);
        }
    }
}
