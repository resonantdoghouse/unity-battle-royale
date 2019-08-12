using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
  public int maxPlayers = 10;

  // instance
  public static NetworkManager instance;

  void Awake()
  {
    instance = this;
    DontDestroyOnLoad(gameObject);
  }

  void Start()
  {
    // connect to master server
    PhotonNetwork.ConnectUsingSettings();
  }

  public override void OnConnectedToMaster()
  {
    // Debug.Log("Connected to Master server");
    // CreateRoom("TestRoom");
    PhotonNetwork.JoinLobby();
  }

  //   public override void OnJoinedRoom(){
  //       Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
  //   }

  public void CreateRoom(string roomName)
  {
    RoomOptions options = new RoomOptions();
    options.MaxPlayers = (byte)maxPlayers;
    PhotonNetwork.CreateRoom(roomName, options);
  }

  public void JoinRoom(string roomName)
  {
    PhotonNetwork.JoinRoom(roomName);
  }

  [PunRPC]
  public void ChangeScene(string sceneName)
  {
    PhotonNetwork.LoadLevel(sceneName);
  }
}
