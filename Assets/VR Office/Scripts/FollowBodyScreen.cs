using UnityEngine;
using Photon.Pun;

namespace ChiliGames.VROffice
{
    //This script is attached to the VR body, to ensure each part is following the correct tracker. This is done only if the body is owned by the player
    //and replicated around the network with the Photon Transform View component
    public class FollowBodyScreen : MonoBehaviour
    {
        public Transform[] body;

        PhotonView pv;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!pv.IsMine) return;
            for (int i = 0; i < body.Length; i++)
            {
                body[i].position = PlatformManager.instance.screenRigParts[i].position;
                body[i].rotation = PlatformManager.instance.screenRigParts[i].rotation;
            }
        }
    }
}

