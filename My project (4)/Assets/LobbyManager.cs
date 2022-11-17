using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField newRoomInputField;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] Button StartGameButton;
    [SerializeField] GameObject roomPanel;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] GameObject playerListObject;
    [SerializeField] GameObject RoomListObject;
    [SerializeField] PlayerItem playerItemPrefab;
    [SerializeField] RoomItem roomItemPrefab;
    List<RoomItem> roomItemList = new List<RoomItem>();
    List<PlayerItem> playerItemsList = new List<PlayerItem>();
    
    Dictionary<string,RoomInfo> roomInfoCache = new Dictionary<string, RoomInfo>();
    private void Start()
    {
        feedbackText.text = "Joining Lobby";
        PhotonNetwork.JoinLobby();
        roomPanel.SetActive(false);
    }

    public void ClickCreateRoom()
    {
        feedbackText.text = "";
        if (newRoomInputField.text.Length < 3)
        {
            feedbackText.text = "Room Name min 3 characters";
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        // roomOptions.IsVisivle = false;
        PhotonNetwork.CreateRoom(newRoomInputField.text);
    }

    public void ClickStartGame(string levelname)
    {
        

        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(levelname);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
        feedbackText.text = "Created room: " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        feedbackText.text = "Joined room: " + PhotonNetwork.CurrentRoom.Name;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);
        // update player list
        UpdatePlayerList();

        //atur start button
        SetStartGameButton();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

        // base.OnPlayerEnteredRoom(newPlayer);
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        //atur Start button
        SetStartGameButton();
    }

    private void SetStartGameButton()
    {
        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        StartGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount >= 2;
    }

    private void UpdatePlayerList()
    {
        foreach (var item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();
        // PhotonNetwork.PlayerList
        foreach (var(id, player) in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab,playerListObject.transform);
            newPlayerItem.Set(player);
            playerItemsList.Add(newPlayerItem);

            if(player == PhotonNetwork.LocalPlayer)
                newPlayerItem.transform.SetAsFirstSibling();

        }

        SetStartGameButton();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode + "," + message);
        feedbackText.text = returnCode.ToString() + ": " + message;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var roomInfo in roomList)
        {
            roomInfoCache[roomInfo.Name] = roomInfo;
        }

        Debug.Log("Room Updated");

        foreach (var item in roomItemList)
        {
            Destroy(item.gameObject);
        }

        this.roomItemList.Clear();

        foreach (var (roomName, roomInfo) in roomInfoCache)
        {
            if (roomInfo.IsVisible == false)
                continue;
            RoomItem newRoomItem = Instantiate(roomItemPrefab, RoomListObject.transform);
            newRoomItem.Set(this, roomInfo.Name);
            this.roomItemList.Add(newRoomItem);
        }
    }
}
