using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SubSpace
{
    public class GameManager : Photon.PunBehaviour
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public Vector3 spawnPoint;

        bool enableSpawning = false;

        private void Start()
        {
            if (PhotonNetwork.isMasterClient)
            {
                // Nothing at the Moment
            }

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (SubSpace.Player.PlayerManager.LocalPlayerInstance == null)
                {
                    enableSpawning = true;
                    if (!PhotonNetwork.connected || (PhotonNetwork.InstantiateInRoomOnly && !PhotonNetwork.inRoom))
                    {
                        StartTestingLobby();
                        Debug.LogWarning("Player wasn't connected. You are in a Testing Room");
                    }
                    else
                    {
                        PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoint, Quaternion.identity, 0);
                    }
                }
            }
        }

        private void StartTestingLobby()
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.logLevel = PhotonLogLevel.Full;
            PhotonNetwork.offlineMode = false;
            PhotonNetwork.ConnectUsingSettings("testing");
            PhotonNetwork.player.NickName = "TestingPlayer";

        }

        #region Photon Messages

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.CreateRoom("Testing", new RoomOptions() { MaxPlayers = 3, IsVisible = true }, null);
            
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("GameManager: OnJoinedRoom() called by PUN. Now this client is in a room.");
            if(SubSpace.Player.PlayerManager.LocalPlayerInstance == null && enableSpawning)
                PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoint, Quaternion.identity, 0);
        }


        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public void LeaveRoom(GameObject player)
        {
            Destroy(player);
            PhotonNetwork.LeaveRoom();
        }

        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
        }
        
        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
                LoadArena();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                LoadArena();
            }
        }


        #endregion
    }
}
