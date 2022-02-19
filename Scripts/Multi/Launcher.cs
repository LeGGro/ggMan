using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        private string playerName;
        private string roomName;
        public GameObject roomJoinUI;
        public GameObject buttonLoadArena;

        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectUsingSettings();
        }
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
            
           
        }
        public void CreateRoom()
        {
           PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4});


        }
        public void JoinRandomRoom()
        {
             PhotonNetwork.JoinRandomRoom();


        }
        public override void OnJoinedRoom()
        {
            Debug.Log("We Connected to room");
            //Application.LoadLevel("TetrisMultiTest");
            PhotonNetwork.LoadLevel("AdditionalScene");
        }
        public void SetName(string s) { PhotonNetwork.NickName = s; Debug.Log("nick setted with "+ PhotonNetwork.NickName);}
    }
}
