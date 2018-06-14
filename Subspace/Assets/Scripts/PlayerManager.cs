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

            movePlayer.SyncPlayerToShip();
            switch (state)
            {
                case State.walking:
                    movePlayer.UpdateMovement();
                    break;
                case State.flying:
                    moveShip.UpdateMovement(ship);
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

        }

        void CheckInteractive()
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2f))
            {
                if (hit.collider.tag == "Interactive")
                {
                    dot.color = interactive;
                    if (Input.GetKeyDown(KeyCode.E) && hit.collider.transform.parent.CompareTag("Ship"))
                    {

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

        enum State
        {
            walking, flying, gunning, interact, engineering
        }

    }
}
