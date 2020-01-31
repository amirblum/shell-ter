using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        currState = requestedState;

        // Create and initialize the FMOD instance
        musicState = FMODUnity.RuntimeMAnager.CreateInstance(MusicStateEvent);
        musicState.Start();

        //Chace a handle to the intesity parameter
        FMOD.Studio.EventDescription intensityEventDescription = FMODUnity.RuntimeManager.GetEventDescription(HealEvent);
        FMOD.Studio.PARAMETER_DESCRIPTION intensityParameterDescription;
        fullHealEventDescription.getParameterDescriptionByName("intensity", out intensityEventDescription);
        intensityParamID = intensityParameterDescription.id;

        //Chace a handle to the state parameter
        FMOD.Studio.EventDescriptions stateEventDescription = FMODUnity.RuntimeManager.GetEventDescription(HealEvent);
        FMOD.Studio.PARAMETER_DESCRIPTION stateParameterDescription;
        fullHealEventDescription.getParameterDescriptionByName("state", out stateEventDescription);
        intensityParamID = stateParameterDescription.id;
    }

    // Update is called once per frame
    void Update()
    {
        musicState.SetParameterByID(intensityParamID, (float)slopeIntensity);
        if(requestedState != currState)
		{
            musicState.SetParameterByID(stateParamID, (uint)currState);
		}
    }
}
