using System;
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
        public bool autoJoinLobby = true;

        #pragma warning disable IDE0044 // Add readonly modifier
        public string _gameVersion = "1";
         #pragma warning restore IDE0044 // Add readonly modifier
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        public byte MaxPlayersPerRoom = 3;
        
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        public GameObject progressLabel;

        [Header("Lobby")]
        public GameObject lobby;
        public GameObject playerInfo;
        public Button startGameBt;
        [Header("Join Lobby")]
        public GameObject joinLobby;
        public GameObject createOwnLobbyName;
        public InputField playerName;
        public GameObject lobbyPrefab;
        public GameObject allLobbys;

        public RoomOptions options;

        List<GameObject> roomList = new List<GameObject>();

        public Text infoText;

        #endregion

        #region Private Variables

       

        public bool isConnecting;

        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            PhotonNetwork.autoJoinLobby = autoJoinLobby;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.logLevel = _logLevel;
            PhotonNetwork.offlineMode = useOffline;
            
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
            isConnecting = true;
            playerName.text = PhotonNetwork.player.NickName;

        }

        private void FixedUpdate()
        {
            if(PhotonNetwork.room != null)
            {
                Text info = playerInfo.GetComponent<Text>();
                string infoText = null;
                infoText += "Room: " + PhotonNetwork.room.Name +
                            "\nPlayers: " + PhotonNetwork.room.PlayerCount + " / " + PhotonNetwork.room.MaxPlayers +
                            "\nAll Player:\n";

                int i = 1;
                foreach (PhotonPlayer p in PhotonNetwork.playerList)
                {
                    infoText += "\nPlayer " + i + ": " + p.NickName;
                    i++;
                }

                info.text = infoText;
            }
        }
        #endregion

        #region Public Methods

        public void CreateRoom()
        {
            string name = createOwnLobbyName.GetComponent<InputField>().text;
            if (name.Length > 2 && isConnecting == false)
            {
                progressLabel.SetActive(true);
                isConnecting = true;
                PhotonNetwork.CreateRoom(name, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom, IsVisible = true }, null);
            }
        }

        public void OnPlayerNameChanged()
        {
            PhotonNetwork.player.NickName = playerName.text;
            playerName.text = PhotonNetwork.player.NickName;
        }

        public void StartGame()
        {
            
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.room.IsOpen = false;
                PhotonNetwork.LoadLevel(1);
            }
            else
            {
                Debug.LogError("You are not allowed to start the game");
            }
                
        }

        #endregion

        #region Callbacks

        public void OnRoomJoin(object source, OnJoinLobbyArgs e)
        {
            if (PhotonNetwork.playerName.Length > 2)
            {
                progressLabel.SetActive(true);
                RoomInfo room = e.Room;
                isConnecting = true;
                PhotonNetwork.JoinRoom(room.Name);
            }
            else
            {
                Debug.LogError("Please Set a User Name (min 3 Chars)");
            }

        }

        #endregion

        #region Photon.PunBehaviour CallBacks


        public override void OnConnectedToMaster()
        {
            Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");
            isConnecting = false;
        }


        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(false);
            isConnecting = false;
            Debug.LogWarning("Launcher: OnDisconnectedFromPhoton() was called by PUN");
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.LogError("Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
            isConnecting = false;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            progressLabel.SetActive(false);
            joinLobby.SetActive(false);
            lobby.SetActive(true);
            isConnecting = false;
            if(!PhotonNetwork.isMasterClient)
                startGameBt.interactable = false;
        }

        public override void OnLobbyStatisticsUpdate(){
            Debug.Log("Update");
            infoText.text = "Rooms " + PhotonNetwork.countOfRooms + "\n" +
                        "Players not in Room: " + PhotonNetwork.countOfPlayersOnMaster + "\n" +
                        "Players in Room: " + PhotonNetwork.countOfPlayersInRooms + "\n" +
                        "Players: " + PhotonNetwork.countOfPlayers;
        }

        public override void OnConnectedToPhoton()
        {
            base.OnConnectedToPhoton();
            isConnecting = false;
        }


        public override void OnReceivedRoomListUpdate()
        {
            int height = 0;
            RoomInfo[] allRooms = PhotonNetwork.GetRoomList();

            foreach (GameObject room in roomList)
            {
                Destroy(room);
                }
            
            roomList.Clear();

            foreach (RoomInfo room in allRooms)
            {
                GameObject prefab = Instantiate(lobbyPrefab, allLobbys.transform);
                prefab.transform.Translate(0, height, 0);
                string status = "Error";
                if (room.IsOpen)
                    status = " Status: <i>Waiting</i>";
                else
                    status = " Status: <i>Running</i>";

                prefab.GetComponentInChildren<Text>().text = room.Name + ":  " + room.PlayerCount + " / " + room.MaxPlayers + status;
                prefab.GetComponent<Lobby>().room = room;
                prefab.GetComponent<Lobby>().OnJoinLobby += OnRoomJoin;
                roomList.Add(prefab);

                
                height -= 50;

                if(roomList.Count > 6)
                {
                    // TODO Solve that Problem
                    //allLobbys.transform.rotation += new Quaternion(0, 50, 0, 0);
                }
            }
        }
        #endregion




    }
}

