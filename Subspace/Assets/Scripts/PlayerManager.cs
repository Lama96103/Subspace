using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubSpace.Player
{
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {
        Player.PlayerMovement movePlayer;
        Ship.PlayerShipMovement moveShip;

        ShipController ship;
        PhotonView shipView;

        GameObject pilotSeat;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [SerializeField]
        State state = State.walking;



        public string playerName = "PLAYER";

        public Color nonInteractive;
        public Color interactive;
        Image dot;

        int health = 100;

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
            ship = GameObject.FindGameObjectWithTag("Ship").GetComponent<ShipController>();
            shipView = ship.GetComponent<PhotonView>();

            dot = GetComponentInChildren<Image>();
            dot.color = nonInteractive;



            gameObject.name = "Player " + PhotonNetwork.playerName;
            playerName = "Player " + PhotonNetwork.playerName;
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

            if (Input.GetKeyDown(KeyCode.O))
            {
                shipView.RPC("ActivateDoor", PhotonTargets.All, true);
                //ship.ActivateDoor(true);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                shipView.RPC("ActivateDoor", PhotonTargets.All, false);
                //ship.ActivateDoor(false);
            }

        }

        void RequestShipOwnerShip()
        {
            shipView.RequestOwnership();
        }

        void CheckInteractive()
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2f))
            {
                if (hit.collider.CompareTag("Interactive"))
                {
                    dot.color = interactive;
                    if (Input.GetKeyDown(KeyCode.E) && hit.collider.transform.parent.CompareTag("Ship"))
                    {
                        EnterPilotSeat(hit.collider.transform.parent.gameObject);
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
            GetComponentInChildren<Camera>().transform.rotation = new Quaternion(0, 0, 0, 0);
            this.transform.rotation = pilotSeat.transform.rotation;
            this.transform.position = pilotSeat.transform.position;
            shipView.TransferOwnership(PhotonNetwork.player.ID);
        }

        void ExitPilotSeat()
        {
            state = State.walking;
            GetComponentInChildren<Camera>().transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        

        enum State
        {
            walking, flying, gunning, interact, engineering
        }

    }
}
