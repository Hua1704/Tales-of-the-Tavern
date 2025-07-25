using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro; // If using TextMeshPro

public class RoomListItem : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text roomNameText;     // Assign in Inspector
    public TMP_Text playerCountText;  // Assign in Inspector
    public Button joinRoomButton;     // Assign in Inspector

    private RoomInfo _roomInfo;

    public void Setup(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;

        if (roomNameText != null)
            roomNameText.text = roomInfo.Name;
        else
            Debug.LogError("RoomNameText not assigned in RoomListItem for: " + roomInfo.Name);

        if (playerCountText != null)
            playerCountText.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        else
            Debug.LogError("PlayerCountText not assigned in RoomListItem for: " + roomInfo.Name);

        if (joinRoomButton != null)
        {
            joinRoomButton.onClick.RemoveAllListeners(); // Clear previous listeners
            joinRoomButton.onClick.AddListener(OnJoinRoomButtonClicked);
            // You might want to disable the button if the room is full,
            // though the manager script already filters these out.
            // joinRoomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers && roomInfo.IsOpen;
        }
        else
            Debug.LogError("JoinRoomButton not assigned in RoomListItem for: " + roomInfo.Name);
    }

    void OnJoinRoomButtonClicked()
    {
        if (_roomInfo == null)
        {
            Debug.LogError("RoomInfo is null. Cannot join room.");
            return;
        }

        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby(); // Important: Must leave lobby before joining a room
        }
        // After LeaveLobby, OnLeftLobby callback will be invoked.
        // We can then join the room.
        // A more robust way is to wait for OnLeftLobby callback and then join.
        // For simplicity here, we'll call JoinRoom directly.
        // Photon handles this sequence fairly well.

        Debug.Log($"Attempting to join room: {_roomInfo.Name}");
        PhotonNetwork.JoinRoom(_roomInfo.Name);

        // Optional: Disable button or show "Joining..."
        if (joinRoomButton != null)
        {
            joinRoomButton.interactable = false;
            // You might want to change button text to "Joining..."
        }
    }

    // It's good practice to clean up listeners when the object is destroyed
    void OnDestroy()
    {
        if (joinRoomButton != null)
        {
            joinRoomButton.onClick.RemoveAllListeners();
        }
    }
}