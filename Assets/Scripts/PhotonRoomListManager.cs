using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement; // If using TextMeshPro

public class PhotonRoomListManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public GameObject roomItemPrefab; // Your UI prefab for a single room entry
    public Transform roomListParent;  // The parent object with VerticalLayoutGroup
    public TMP_Text statusText;       // Optional: Text to show connection status

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    void Start()
    {
        // Critical: Auto-sync Scene for all players in a room
        PhotonNetwork.AutomaticallySyncScene = true;

        // Connect to Photon Master Server if not already connected
        if (!PhotonNetwork.IsConnected)
        {
            UpdateStatus("Connecting to Master...");
            PhotonNetwork.ConnectUsingSettings();
        }
        else if (!PhotonNetwork.InLobby) // If connected but not in lobby
        {
            UpdateStatus("Joining Lobby...");
            PhotonNetwork.JoinLobby();
        }
    }

    void UpdateStatus(string message)
    {
        Debug.Log(message);
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        UpdateStatus("Connected to Master! Joining Lobby...");
        PhotonNetwork.JoinLobby(); // Default lobby is fine for room listing
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UpdateStatus($"Disconnected: {cause}");
        Debug.LogError($"PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {cause}");
    }

    public override void OnJoinedLobby()
    {
        UpdateStatus("Joined Lobby! Waiting for room list...");
        // Room list will be updated via OnRoomListUpdate callback
        cachedRoomList.Clear(); // Clear cache when joining lobby
    }

    public override void OnLeftLobby()
    {
        UpdateStatus("Left Lobby.");
        cachedRoomList.Clear();
        ClearRoomListView();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        PhotonNetwork.LoadLevel("Multiplayer PlayArea");

    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateStatus($"Room list updated. Found {roomList.Count} rooms.");
        Debug.Log($"OnRoomListUpdate: Received {roomList.Count} rooms.");

        // Update cached list
        // The roomList Photon provides contains rooms that have been updated.
        // Rooms that are no longer available will have their RemovedFromList flag set.
        foreach (RoomInfo info in roomList)
        {
            // Remove from cached list if it's marked as removed
            if (info.RemovedFromList)
            {
                cachedRoomList.RemoveAll(x => x.Name == info.Name);
            }
            else
            {
                // Add or update in cached list
                int index = cachedRoomList.FindIndex(x => x.Name == info.Name);
                if (index != -1)
                {
                    cachedRoomList[index] = info; // Update existing
                }
                else
                {
                    cachedRoomList.Add(info); // Add new
                }
            }
        }

        UpdateRoomListView();
    }

    #endregion

    void ClearRoomListView()
    {
        if (roomListParent == null)
        {
            Debug.LogError("RoomListParent is not assigned!");
            return;
        }

        foreach (Transform child in roomListParent)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateRoomListView()
    {
        ClearRoomListView();

        if (cachedRoomList.Count == 0)
        {
            UpdateStatus("No rooms available.");
            // Optionally, instantiate a "No rooms found" message prefab
            return;
        }

        foreach (RoomInfo info in cachedRoomList)
        {
            // Only display rooms that are open and visible
            if (info.PlayerCount >= info.MaxPlayers || !info.IsVisible || !info.IsOpen)
            {
                continue; // Skip full, invisible, or closed rooms
            }

            GameObject roomItemGO = Instantiate(roomItemPrefab, roomListParent);
            RoomListItem roomListItemUI = roomItemGO.GetComponent<RoomListItem>();

            if (roomListItemUI != null)
            {
                roomListItemUI.Setup(info);
            }
            else
            {
                Debug.LogError($"RoomItemPrefab is missing the RoomListItem script on its root object for room: {info.Name}");
                Destroy(roomItemGO); // Clean up if setup fails
            }
        }
    }

    // Example of how to refresh list manually (e.g., a refresh button)
    public void RefreshRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            UpdateStatus("Refreshing room list...");
            // OnRoomListUpdate will be called automatically by Photon after joining lobby.
            // If you want to force a more immediate (though potentially incomplete) list:
            // PhotonNetwork.GetCustomRoomList(TypedLobby.Default, ""); // This is usually not needed
            // The best way is to rejoin the lobby if you suspect stale data,
            // but OnRoomListUpdate generally keeps things current.
            // For now, simply re-evaluating the cached list is often enough for a UI refresh button.
            UpdateRoomListView();
        }
        else if (PhotonNetwork.IsConnected)
        {
            UpdateStatus("Not in lobby. Joining lobby to refresh...");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            UpdateStatus("Not connected. Connecting to refresh...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}