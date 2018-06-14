using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubSpace.Networking
{
    public class Launcher : Photon.PunBehaviour
    {
        #region Public Variables

        public PhotonLogLevel _logLevel = PhotonLogLevel.Informational;
        public bool useOffline = true;
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        public byte MaxPlayersPerRoom = 4;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        public GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        public GameObject progressLabel;

        public Text infoText;

        #endregion

        #region Private Variables

        string _gameVersion = "1";
        bool isConnecting;

        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.logLevel = _logLevel;
            PhotonNetwork.offlineMode = useOffline;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
        #endregion

        #region Public Methods

        public void Connect()
        {
            isConnecting = true;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        #endregion

        #region Photon.PunBehaviour CallBacks


        public override void OnConnectedToMaster()
        {
            Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");
            if (isConnecting)
                PhotonNetwork.JoinRandomRoom();
        }


        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarning("Launcher: OnDisconnectedFromPhoton() was called by PUN");
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            PhotonNetwork.LoadLevel(1);
        }

        public override void OnLobbyStatisticsUpdate(){
            infoText.text = "Rooms " + PhotonNetwork.countOfRooms + "\n" +
                        "Players not in Room: " + PhotonNetwork.countOfPlayersOnMaster + "\n" +
                        "Players in Room: " + PhotonNetwork.countOfPlayersInRooms + "\n" +
                        "Players: " + PhotonNetwork.countOfPlayers;
        }

        #endregion




    }
}

