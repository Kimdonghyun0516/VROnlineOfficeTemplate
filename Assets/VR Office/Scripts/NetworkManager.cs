using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ChiliGames.VROffice
{
    //This script connects to PHOTON servers and creates a room if there is none, then automatically joins
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        bool triesToConnectToMaster = false;
        bool triesToConnectToRoom = false;

        private void Update()
        {
            if (!PhotonNetwork.IsConnected && !triesToConnectToMaster)
            {
                ConnectToMaster();
            }
            if (PhotonNetwork.IsConnected && !triesToConnectToMaster && !triesToConnectToRoom)
            {
                StartCoroutine(WaitFrameAndConnect());
            }
        }

        public void ConnectToMaster()
        {
            PhotonNetwork.OfflineMode = false; //true would "fake" an online connection
            PhotonNetwork.NickName = "PlayerName"; //we can use a input to change this 
            PhotonNetwork.AutomaticallySyncScene = true; //To call PhotonNetwork.LoadLevel()
            PhotonNetwork.GameVersion = "v1"; //only people with the same game version can play together

            triesToConnectToMaster = true;
            //PhotonNetwork.ConnectToMaster(ip, port, appid); //manual connection
            PhotonNetwork.ConnectUsingSettings(); //automatic connection based on the config file
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            triesToConnectToMaster = false;
            triesToConnectToRoom = false;
            Debug.Log(cause);
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            triesToConnectToMaster = false;
            Debug.Log("Connected to master!");
        }

        IEnumerator WaitFrameAndConnect()
        {
            triesToConnectToRoom = true;
            yield return new WaitForEndOfFrame();
            Debug.Log("Connecting");
            ConnectToRoom();
        }

        public void ConnectToRoom()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            triesToConnectToRoom = true;

            //We create a new room with the given options: 10 player max and to not delete cache when leaving, so we keep what's been already drawed on whiteboards.
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 10;
            PhotonNetwork.JoinOrCreateRoom("VROffice", options, null);

        }

        public override void OnJoinedRoom()
        {
            //Go to next scene after joining the room
            base.OnJoinedRoom();
            Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);
            
            SceneManager.LoadScene("Office"); //go to the room scene
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            //no room available
            //create a room (null as a name means "does not matter")
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 15 });
        }
    }
}