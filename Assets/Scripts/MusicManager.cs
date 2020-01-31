using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Range(0, 1)]
    public float slopeIntensity = 0;


    public enum States
    {
        STOP = 0,
        ONE_IS_MOVING = 1,
        BOTH_MOVING = 2,
        WIN_STATE = 3
    }

    //[Serializeable]
    public States requestedState = States.STOP;
    States currState;

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
        instance_e.setParameterByID(intensityParamID, (float)slopeIntensity);
        if (requestedState != currState)
        {
            currState = requestedState;
            instance_e.setParameterByID(stateParamID, (uint)currState);
        }
    }
}
