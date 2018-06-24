using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SubSpace.Player.Player
{
    public class PlayerMovement : MonoBehaviour
    {

        CharacterController character;
        Camera cam;

        [SerializeField]
        private readonly float movementSpeed = 10;

        [SerializeField]
        private readonly float gravity = 2;

        [SerializeField]
        private readonly float lookSpeed= 5;

        ShipController currentShip;
        Vector3 localPosition;



        // Use this for initialization
        void Start()
        {
            character = GetComponent<CharacterController>();
            cam = GetComponentInChildren<Camera>();
            FindShip();
        }

        #region Public Functions

        public void UpdateMovement()
        {
            if (currentShip == null)
            {
                FindShip();
            }
            
            if(!character.isGrounded)
                character.Move(-transform.up * Time.deltaTime * gravity);

            Walking();
            Rotation();

        }

        public void SyncPlayerToShip()
        {
            SyncPlayerRotationWithShip();
            SyncPlayerLocationWithShip();
        }

        #endregion

        #region Walking Functions

        void Rotation()
        {
            float h = lookSpeed * Input.GetAxis("Mouse X");
            float v = lookSpeed * Input.GetAxis("Mouse Y");

            character.transform.Rotate(0, h, 0);
            cam.transform.Rotate(-v, 0, 0);
        }

        void Walking()
        {

            if (Input.GetKey(KeyCode.W))
            {
                character.SimpleMove(transform.forward * movementSpeed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                character.SimpleMove(-transform.forward * movementSpeed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                character.SimpleMove(-transform.right * movementSpeed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                character.SimpleMove(transform.right * movementSpeed);
            }
        }

        
        #endregion

        #region Utilities Functions

        void FindShip()
        {
            currentShip = GameObject.FindGameObjectWithTag("Ship").GetComponent<ShipController>();
            lastShipPos = currentShip.transform.position;
            lastYRotation = currentShip.transform.rotation.y;
        }



        private float lastYRotation;
        void SyncPlayerRotationWithShip()
        {
            float curYRotation = currentShip.transform.rotation.eulerAngles.y;
            float yAngle = currentShip.transform.rotation.eulerAngles.y - lastYRotation;
            transform.RotateAround(currentShip.transform.position, Vector3.up, yAngle);

            lastYRotation = curYRotation;
        }

        private Vector3 lastShipPos;
        void SyncPlayerLocationWithShip()
        {
            Vector3 curShipPos = currentShip.transform.position;
            Vector3 moveDir = Vector3.zero;
            moveDir = curShipPos - lastShipPos;

            transform.position += moveDir;

            lastShipPos = curShipPos;
        }

        #endregion
    }

}
