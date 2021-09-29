using Photon.Pun;
using UnityEngine;
using Photon.Voice.Unity;

namespace ChiliGames
{
    public class SetMicrophone : MonoBehaviourPun
    {
        //detects device microphone and sets it to "Recorder" component from Photon Voice
        private void Start()
        {
            string[] devices = Microphone.devices;
            if (devices.Length > 0)
            {
                GetComponent<Recorder>().UnityMicrophoneDevice = devices[0];
            }
        }

    }
}
