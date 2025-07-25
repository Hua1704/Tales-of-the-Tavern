// CreateRoomTest.cs (attach to a temporary button or object)
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomTest : MonoBehaviourPunCallbacks
{
    public void CreateTestRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;
            PhotonNetwork.CreateRoom("TestRoom_" + Random.Range(0, 1000), roomOptions);
            Debug.Log("Created Room");
        }
        else if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Not connected, trying to connect...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnCreatedRoom() => Debug.Log("Room Created: " + PhotonNetwork.CurrentRoom.Name);
    public override void OnCreateRoomFailed(short returnCode, string message) => Debug.LogError($"Create Room Failed: {message} ({returnCode})");
    public override void OnJoinedRoom() => Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
    public override void OnJoinRoomFailed(short returnCode, string message) => Debug.LogError($"Join Room Failed: {message} ({returnCode})");
}