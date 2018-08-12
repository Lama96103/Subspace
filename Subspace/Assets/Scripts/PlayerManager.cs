using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubSpace.Player
{
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {
        private Player.PlayerMovement movePlayer;
        private Ship.PlayerShipMovement moveShip;

        private Camera playerCamera;

        private SubSpace.Ship.ShipController ship;
        private PhotonView shipView;

        private GameObject pilotSeat;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [SerializeField]
        private State state = State.walking;
        [SerializeField]
        private CameraState cameraState = CameraState.fly;


        public string playerName = "PLAYER";

        public Color nonInteractive;
        public Color interactive;
        private Image dot;

        private int health = 100;

        TMPro.TextMeshPro textMesh;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                //stream.SendNext(IsFiring);
                stream.SendNext(health);
            }
            else
            {
                //this.IsFiring = (bool)stream.ReceiveNext();
                this.health = (int)stream.ReceiveNext();
            }
        }

        void Awake()
        {
            if (photonView.isMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        // Use this for initialization
        void Start()
        {
            movePlayer = GetComponent<Player.PlayerMovement>();
            moveShip = GetComponent<Ship.PlayerShipMovement>();
            ship = GameObject.FindGameObjectWithTag("Ship").GetComponent<SubSpace.Ship.ShipController>();
            shipView = ship.GetComponent<PhotonView>();
            playerCamera = GetComponentInChildren<Camera>();
            dot = GetComponentInChildren<Image>();
            dot.color = nonInteractive;

            textMesh = GetComponentInChildren<TMPro.TextMeshPro>();

            gameObject.name = "Player_" + PhotonNetwork.playerName;
            playerName = "Player_" + PhotonNetwork.playerName;

            textMesh.text = PhotonNetwork.playerName;
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;
            }

           
            switch (state)
            {
                case State.walking:
                    movePlayer.UpdateMovement();
                    movePlayer.SyncPlayerToShip();
                    CheckInteractive();
                    break;
                case State.flying:
                    moveShip.UpdateMovement(ship, pilotSeat);
                    if (Input.GetKeyDown(KeyCode.E))
                        ExitPilotSeat();
                    break;
                case State.gunning:
                    break;
                case State.interact:
                    break;
                case State.engineering:
                    break;
                default:
                    break;
            }

            if(state == State.walking)
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    shipView.RPC("ActivateDoor", PhotonTargets.All, true);
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    shipView.RPC("ActivateDoor", PhotonTargets.All, false);
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                if( state == State.flying)
                {
                    ExitPilotSeat();
                }
                LeaveGame();
            }

            if(state == State.flying && Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if (cameraState == CameraState.fly) cameraState = CameraState.ineract;
                else cameraState = CameraState.fly;
            }

        }

        void RequestShipOwnerShip()
        {
            shipView.RequestOwnership();
        }

        void CheckInteractive()
        {
            RaycastHit hit;
            
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
            {
                if (hit.collider.CompareTag("Interactive"))
                {
                    dot.color = interactive;
                    if (Input.GetKeyDown(KeyCode.E) && hit.collider.transform.parent.CompareTag("Ship"))
                    {
                        EnterPilotSeat(hit.collider.transform.parent.gameObject);
                    }
                    if (Input.GetKeyDown(KeyCode.E) && hit.collider.transform.parent.CompareTag("Button"))
                    {
                        hit.collider.transform.parent.GetComponent<ButtonController>().Execute();
                    }
                }
                else
                {
                    dot.color = nonInteractive;
                }

            }
            else
            {
                dot.color = nonInteractive;
            }
        }

        void EnterPilotSeat(GameObject ship)
        {
            state = State.flying;
            pilotSeat = GameObject.FindGameObjectWithTag("PilotSeat");
            playerCamera.transform.rotation = new Quaternion(0, 0, 0, 0);
            this.transform.rotation = pilotSeat.transform.rotation;
            this.transform.position = pilotSeat.transform.position;
            shipView.TransferOwnership(PhotonNetwork.player.ID);
            ship.GetComponent<SubSpace.Ship.ShipController>().AssignPilot(this);
        }

        void ExitPilotSeat()
        {
            state = State.walking;
            playerCamera.transform.rotation = new Quaternion(0, 0, 0, 0);
            ship.GetComponent<SubSpace.Ship.ShipController>().UnAssignPilot(this);
            movePlayer.FindShip();
        }

        public void LeaveGame()
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().LeaveRoom(gameObject);
        }
        

        enum State
        {
            walking, flying, gunning, interact, engineering
        }

        enum CameraState
        {
            fly, ineract
        }

    }
}
