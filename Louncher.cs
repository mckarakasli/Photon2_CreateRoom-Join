using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Louncher : MonoBehaviourPunCallbacks
{

    public static Louncher Instance;
    [SerializeField] TMP_Text roomtext;
    [SerializeField] TMP_InputField roomNameınputFied;
    [SerializeField] TMP_Text errorText;
    [SerializeField] Transform RoomListContent;
    [SerializeField] Transform PlayerListContent;
    [SerializeField] GameObject Roomlistprefab;
    [SerializeField] GameObject PlayerlistItemprefab;
    [SerializeField] GameObject startGameButton;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameınputFied.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameınputFied.text);
        MenuManager.Instance.OpenMenu("loading");

    }
    public override void OnJoinedRoom()
    {

        foreach(Transform child in PlayerListContent)
        {
            Destroy(child.gameObject);
        }
        MenuManager.Instance.OpenMenu("room");
        roomtext.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerlistItemprefab, PlayerListContent).GetComponent<playerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = " RoomCreationFalide" + message;
        MenuManager.Instance.OpenMenu("error");
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading"); 
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");

       
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in RoomListContent)
        {
            Destroy(trans.gameObject);
        }
        
        for(int i =0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(Roomlistprefab, RoomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerlistItemprefab, PlayerListContent).GetComponent<playerListItem>().SetUp(newPlayer);
    }
}

