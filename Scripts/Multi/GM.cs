using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class GM : MonoBehaviourPunCallbacks
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    void Start()
    {

      
        switch (PhotonNetwork.PlayerList.Length)
        {
            case 1:
                Debug.Log("Instantiating Player 1");
                player1  = PhotonNetwork.Instantiate("FirstPlayer", new Vector3Int(0, 0, 0), Quaternion.identity);
                player1.SetActive(false);
                PhotonNetwork.Instantiate("Randomaizer", new Vector3(0, 0, 0), Quaternion.identity);
                GameObject.Find("1playerCamera").SetActive(false);
                break;
            case 2:
                Debug.Log("Instantiating Player 2");
                player2 = PhotonNetwork.Instantiate("SecondPlayer", new Vector3(30f, 0f, 0f), Quaternion.identity);
                player2.SetActive(false);
                GameObject.Find("2playerCamera").SetActive(false);
                break;
            case 3:
                Debug.Log("Instantiating Player 3");
                player3 = PhotonNetwork.Instantiate("ThirdPlayer", new Vector3Int(60, 0, 0), Quaternion.identity);
                player3.SetActive(false);
                GameObject.Find("3playerCamera").SetActive(true);
                break;
            case 4:
                Debug.Log("Instantiating Player 4");
                player4 = PhotonNetwork.Instantiate("FoursPlayer", new Vector3Int(90, 0, 0), Quaternion.identity);
                player4.SetActive(false);
                GameObject.Find("4playerCamera").SetActive(true);
                break;
        }

    }
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    
    }
    public override void OnLeftRoom()
    {
        Application.LoadLevel("Menu");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName +" left the room");
    }
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        switch (PhotonNetwork.PlayerList.Length)
        {

            case 4:
                player1.SetActive(true);
                player2.SetActive(true);
                player3.SetActive(true);
                player4.SetActive(true);
                
                break;

                //Debug.Log(otherPlayer.NickName + " entered the room");
        }
    }
}
