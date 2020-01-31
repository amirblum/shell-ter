using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Range(0, 1)]
    public float slopeIntensity = 0;


    public enum States{
        STOP = 0,
        ONE_IS_MOVING = 1,
        BOTH_MOVING = 2,
        WIN_STATE = 3
	}

    //[Serializeable]
    public States requestedState = States.STOP;
    States currState;


    [FMODUnity.EventRef]
    public string MusicStateEvent = "";
    public string intensityEvent = "";
    public string stateEvent = "";
    FMOD.Studio.EventInstance musicState;

    FMOD.Studio.PARAMETER_ID intensityParamID;
    FMOD.Studio.PARAMETER_ID stateParamID;

    // Start is called before the first frame update
    void Start()
    {
        currState = requestedState;

        // Create and initialize the FMOD instance
        musicState = FMODUnity.RuntimeManager.CreateInstance(MusicStateEvent);
        musicState.start();

        //Chace a handle to the intesity parameter
        FMOD.Studio.EventDescription intensityEventDescription = FMODUnity.RuntimeManager.GetEventDescription(stateEvent);
        FMOD.Studio.PARAMETER_DESCRIPTION intensityParameterDescription;
        intensityEventDescription.getParameterDescriptionByName("intensity", out intensityParameterDescription);
        intensityParamID = intensityParameterDescription.id;

        //Chace a handle to the state parameter
        FMOD.Studio.EventDescription stateEventDescription = FMODUnity.RuntimeManager.GetEventDescription(stateEvent);
        FMOD.Studio.PARAMETER_DESCRIPTION stateParameterDescription;
        stateEventDescription.getParameterDescriptionByName("state", out stateParameterDescription);
        stateParamID = stateParameterDescription.id;
    }

    // Update is called once per frame
    void Update()
    {
        musicState.setParameterByID(intensityParamID, (float)slopeIntensity);
        if(requestedState != currState)
		{
            musicState.setParameterByID(stateParamID, (uint)currState);
		}
    }
}
