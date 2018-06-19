﻿using System.Collections;
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
                        EnterShip(hit.collider.transform.parent.gameObject);
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

        void EnterShip(GameObject ship)
        {
            state = State.flying;
            GetComponentInChildren<Camera>().transform.rotation = Quaternion.identity;
            pilotSeat = GameObject.FindGameObjectWithTag("PilotSeat");
            this.transform.rotation = pilotSeat.transform.rotation;
            this.transform.position = pilotSeat.transform.position;
            shipView.TransferOwnership(PhotonNetwork.player.ID);
        }
        

        enum State
        {
            walking, flying, gunning, interact, engineering
        }

    }
}
