using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // For Path.Combine if using Resources folder

public class GameManager : MonoBehaviourPunCallbacks // Inherit for callbacks
{
    public string playerPrefabName = "Player"; // Name of your player prefab in a "Resources" folder
                                              // OR assign a public GameObject playerPrefab; and drag from Project
    public Transform[] spawnPoints; // Optional: Assign spawn points in the Inspector
    public Button startButton;
    void Start()
    {
        // Crucial for PhotonNetwork.LoadLevel to work correctly for all players
        PhotonNetwork.AutomaticallySyncScene = true;

        // If we are already in a room when this scene loads (e.g., after LoadLevel)
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
            
        }
    }

    // This callback is triggered when the local player joins a room.
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined room: " + PhotonNetwork.CurrentRoom.Name);
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // Check if this client has already instantiated its player (e.g. if Start() already ran)
        // A simple way is to check if player's TagObject is set.
        if (PhotonNetwork.LocalPlayer.TagObject != null)
        {
            Debug.Log("Player already spawned for this client.");
            return;
        }

        Transform spawnPoint = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        Vector3 spawnPosition = spawnPoint ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = spawnPoint ? spawnPoint.rotation : Quaternion.identity;

        // Instantiate the player prefab across the network.
        // Make sure "Player" prefab is in a "Resources" folder.
        GameObject playerGameObject = PhotonNetwork.Instantiate(playerPrefabName, spawnPosition, spawnRotation);

        // (Optional but good practice) Associate the instantiated GameObject with the Photon Player object
        // This can be useful for other scripts to find the GameObject for a specific Photon.Realtime.Player
        PhotonNetwork.LocalPlayer.TagObject = playerGameObject;

        Debug.Log("Player spawned: " + playerGameObject.name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entered room.");
        // Other players will automatically instantiate their own characters when they join and their OnJoinedRoom fires.
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " left room.");
        // Photon automatically cleans up objects instantiated by players who leave.
        // If you stored their TagObject, you might want to clear it:
        if (otherPlayer.TagObject is GameObject playerObj && playerObj != null)
        {
            // Destroy(playerObj); // Photon handles destruction of PhotonNetwork.Instantiate'd objects
        }
        SceneManager.LoadScene(0);
    }

    // Example method to call LoadLevel (usually from a Master Client check in a lobby or menu)
    public void StartGame()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master Client starting game");
            StartCoroutine(DelayedDeactivate());

        }
    }

    private IEnumerator DelayedDeactivate()
    {
        yield return null; // wait 1 frame
        startButton.gameObject.SetActive(false);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}